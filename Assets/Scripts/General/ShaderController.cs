using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace General
{
    /// <summary>
    /// Post processing effect that simulates a CRT screen
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]

    public class ShaderController : MonoBehaviour
    {
        [FormerlySerializedAs("crtImageMat")] public Material crtScanlinesMat;
        public Material crtCurvatureMat;
        public Material crtGlowMat;
        public Material crtChromMat;
        [SerializeField] private ShaderSettings _shaderSettings;
        private static readonly int ScanlineTime = Shader.PropertyToID("_ScanlineTime");
        private static readonly int VignetteFactor = Shader.PropertyToID("_VignetteFactor");
        private static readonly int VignetteExponent = Shader.PropertyToID("_VignetteExponent");
        private static readonly int ScanlinePeriod = Shader.PropertyToID("_ScanlinePeriod");
        
        private Coroutine _interlaceCoroutine;
        private bool _interlacingBool = false;
        private static readonly int InterlacingBool = Shader.PropertyToID("_InterlacingBool");
        private static readonly int ScanlineMinValue = Shader.PropertyToID("_ScanlineMinValue");

        public float InterlaceFrequency = 50;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (crtScanlinesMat != null && crtCurvatureMat != null && crtGlowMat != null)
            {
                crtScanlinesMat.SetFloat(ScanlineTime, Time.unscaledTime);

                if (_shaderSettings.InterlaceEffect)
                {
                    //set interlaceBool
                    crtScanlinesMat.SetFloat(InterlacingBool, _interlacingBool ? 1 : 0);
                }
                
                //create temporary render textures
                RenderTexture temp1 = RenderTexture.GetTemporary(source.width, source.height);
                RenderTexture temp2 = RenderTexture.GetTemporary(source.width, source.height);
                
                //apply the scanlines effect
                Graphics.Blit(source, temp1, crtScanlinesMat);
                
                //apply the chromatic aberration effect
                Graphics.Blit(temp1, temp2, crtChromMat);
                
                //apply the glow effect
                Graphics.Blit(temp2, temp1, crtGlowMat);
                
                //apply the curvature effect
                Graphics.Blit(temp1, destination, crtCurvatureMat);
                
                //release the temporary render textures
                RenderTexture.ReleaseTemporary(temp1);
                RenderTexture.ReleaseTemporary(temp2);
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
            if (crtScanlinesMat != null)
            {
                crtScanlinesMat.SetFloat(VignetteFactor, _shaderSettings.VignetteFactor);
                crtScanlinesMat.SetFloat(VignetteExponent, _shaderSettings.VignetteExponent);

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
                
                    crtScanlinesMat.SetFloat(InterlacingBool, _interlacingBool ? 1 : 0);
                
                    crtScanlinesMat.SetFloat(ScanlineMinValue, 1 - _shaderSettings.ScanlineIntensity);
                }
                else
                {
                    crtScanlinesMat.SetFloat(ScanlineMinValue, 1);
                    
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
                if(InterlaceFrequency!=0) _interlacingBool = !_interlacingBool;

                if (InterlaceFrequency == 0)
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                }
                else
                {
                    yield return new WaitForSecondsRealtime(1f / InterlaceFrequency);
                }
            }
        }
    }
}