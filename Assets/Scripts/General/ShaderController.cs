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
        public Material crtScanlinesMat;
        public Material crtCurvatureMat;
        public Material crtGlowMat;
        public Material crtChromMat;
        private static readonly int VignetteFactor = Shader.PropertyToID("_VignetteFactor");
        private static readonly int VignetteExponent = Shader.PropertyToID("_VignetteExponent");
        internal static readonly int InterlaceFrequency = Shader.PropertyToID("_InterlaceFrequency");
        
        private Coroutine _interlaceCoroutine;
        private bool _interlacingBool = false;
        private static readonly int Interlace = Shader.PropertyToID("_Interlace");

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (crtScanlinesMat != null && crtCurvatureMat != null && crtGlowMat != null)
            {
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

        private void OnEnable()
        {
            //start the interlace coroutine
            
            if (_interlaceCoroutine != null) StopCoroutine(_interlaceCoroutine);
            _interlaceCoroutine = StartCoroutine(InterlaceCoroutine());
        }
        
        private void OnDisable()
        {
            //stop the interlace coroutine
            if (_interlaceCoroutine != null)
            {
                StopCoroutine(_interlaceCoroutine);
                _interlaceCoroutine = null;
            }
        }
        
        private IEnumerator InterlaceCoroutine()
        {
            Debug.Log("InterlaceCoroutine started");
            
            while (true)
            {
                float frequency = crtScanlinesMat.GetFloat(InterlaceFrequency);
                
                if(frequency!=0) _interlacingBool = !_interlacingBool;
                
                crtScanlinesMat.SetFloat(Interlace, _interlacingBool ? 1 : 0);

                if (frequency == 0)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                }
                else
                {
                    yield return new WaitForSecondsRealtime(1f / frequency);
                }
            }
        }
    }
}