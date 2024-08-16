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
        public Material crtMaterial;
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
            if (crtMaterial != null)
            {
                crtMaterial.SetFloat(ScanlineTime, Time.unscaledTime);

                if (_shaderSettings.InterlaceEffect)
                {
                    //set interlaceBool
                    crtMaterial.SetFloat(InterlacingBool, _interlacingBool ? 1 : 0);
                }
                
                Graphics.Blit(source, destination, crtMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
        
        public void UpdateShaderProperties()
        {
            Debug.Log("Updating shader properties");
            if (crtMaterial != null)
            {
                crtMaterial.SetFloat(VignetteFactor, _shaderSettings.VignetteFactor);
                crtMaterial.SetFloat(VignetteExponent, _shaderSettings.VignetteExponent);

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
                
                    crtMaterial.SetFloat(InterlacingBool, _interlacingBool ? 1 : 0);
                
                    crtMaterial.SetFloat(ScanlineMinValue, 1 - _shaderSettings.ScanlineIntensity);
                }
                else
                {
                    crtMaterial.SetFloat(ScanlineMinValue, 1);
                    
                    if (_interlaceCoroutine != null)
                    {
                        StopCoroutine(_interlaceCoroutine);
                        _interlaceCoroutine = null;
                        Debug.Log("Stopping interlace coroutine");
                    }
                }
                
            }
        }

        private void OnStart()
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