using System.Collections;
using UnityEngine;

namespace General
{
    /// <summary>
    /// Post processing effect that simulates a CRT screen
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]

    public class ShaderController : MonoBehaviour
    {
        public Material crtImageMat;
        public Material crtCurvatureMat;
        [SerializeField] private ShaderSettings _shaderSettings;
        private static readonly int ScanlineTime = Shader.PropertyToID("_ScanlineTime");
        private static readonly int VignetteFactor = Shader.PropertyToID("_VignetteFactor");
        private static readonly int VignetteExponent = Shader.PropertyToID("_VignetteExponent");
        private static readonly int ScanlinePeriod = Shader.PropertyToID("_ScanlinePeriod");
        
        private Coroutine _interlaceCoroutine;
        private bool _interlacingBool = false;
        private static readonly int InterlacingBool = Shader.PropertyToID("_InterlacingBool");
        private static readonly int ScanlineMinValue = Shader.PropertyToID("_ScanlineMinValue");

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (crtImageMat != null && crtCurvatureMat != null)
            {
                crtImageMat.SetFloat(ScanlineTime, Time.unscaledTime);

                if (_shaderSettings.InterlaceEffect)
                {
                    //set interlaceBool
                    crtImageMat.SetFloat(InterlacingBool, _interlacingBool ? 1 : 0);
                }
                
                //create temporary render texture
                RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
                
                //apply the image effects
                Graphics.Blit(source, temp, crtImageMat);
                
                //apply the curvature effect
                Graphics.Blit(temp, destination, crtCurvatureMat);
                
                //release the temporary render texture
                RenderTexture.ReleaseTemporary(temp);
            }
            else
            {
                Graphics.Blit(source, destination);
                Debug.LogError("CRT shader materials not set in ShaderController");
            }
        }
        
        public void UpdateShaderProperties()
        {
            Debug.Log("Updating shader properties");
            if (crtImageMat != null)
            {
                crtImageMat.SetFloat(VignetteFactor, _shaderSettings.VignetteFactor);
                crtImageMat.SetFloat(VignetteExponent, _shaderSettings.VignetteExponent);

                if (_shaderSettings.ScanlineEffect)
                {
                    //set the interlacing effect
                    if (_shaderSettings.InterlaceEffect)
                    {
                        if (_interlaceCoroutine == null)
                        {
                            _interlaceCoroutine = StartCoroutine(InterlaceCoroutine());
                            Debug.Log("Starting interlace coroutine");
                        }
                    }
                    else
                    {
                        if (_interlaceCoroutine != null)
                        {
                            StopCoroutine(_interlaceCoroutine);
                            _interlaceCoroutine = null;
                            Debug.Log("Stopping interlace coroutine");
                        }
                    }
                
                    crtImageMat.SetFloat(InterlacingBool, _interlacingBool ? 1 : 0);
                
                    crtImageMat.SetFloat(ScanlineMinValue, 1 - _shaderSettings.ScanlineIntensity);
                }
                else
                {
                    crtImageMat.SetFloat(ScanlineMinValue, 1);
                    
                    if (_interlaceCoroutine != null)
                    {
                        StopCoroutine(_interlaceCoroutine);
                        _interlaceCoroutine = null;
                        Debug.Log("Stopping interlace coroutine");
                    }
                }
                
            }
        }

        private void Start()
        {
            if (_shaderSettings == null)
            {
                Debug.LogError("ShaderSettings reference not set in CRTPostProcessing");
            }
        }

        private void OnEnable()
        {
            //subscribe to the settings changed event
            ShaderSettings.OnSettingsChanged += UpdateShaderProperties;
            
            UpdateShaderProperties();
        }
        
        private void OnDisable()
        {
            //unsubscribe from the settings changed event
            ShaderSettings.OnSettingsChanged -= UpdateShaderProperties;
        }
        
        private IEnumerator InterlaceCoroutine()
        {
            while (true)
            {
                _interlacingBool = !_interlacingBool;
                yield return new WaitForSecondsRealtime(1f / _shaderSettings.InterlacingPerSecond);
            }
        }
    }
}