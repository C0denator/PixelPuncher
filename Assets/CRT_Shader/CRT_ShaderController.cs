using System.Collections;
using UnityEngine;

namespace CRT_Shader
{
    /// <summary>
    /// Post-processing effect that simulates a CRT screen
    /// </summary>
    /// <remarks>
    /// This script applies a series of post processing effects to the camera to simulate a CRT screen. It must be attached to the camera that will render the scene.
    /// </remarks>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]

    public class ShaderController : MonoBehaviour
    {
        public Material crtScanlinesMat;
        public Material crtCurvatureMat;
        public Material crtGlowMat;
        public Material crtChromMat;
        
        private static readonly int InterlaceFrequency = Shader.PropertyToID("_InterlaceFrequency");
        private static readonly int Interlace = Shader.PropertyToID("_Interlace");
        
        private Coroutine _interlaceCoroutine;
        private bool _interlacingBool;

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
        
        private void Awake()
        {
            //error checking
            if (crtScanlinesMat == null)
            {
                Debug.LogError("CRT Scanlines material not set in ShaderController");
            }
            
            if (crtCurvatureMat == null)
            {
                Debug.LogError("CRT Curvature material not set in ShaderController");
            }
            
            if (crtGlowMat == null)
            {
                Debug.LogError("CRT Glow material not set in ShaderController");
            }
            
            if (crtChromMat == null)
            {
                Debug.LogError("CRT Chromatic Aberration material not set in ShaderController");
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
            
            // ReSharper disable once IteratorNeverReturns
        }
    }
}