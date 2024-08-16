using UnityEngine;

namespace General
{
    /// <summary>
    /// Post processing effect that simulates a CRT screen
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]

    public class CRTPostProcessing : MonoBehaviour
    {
        public Material crtMaterial;
        private static readonly int ScanlineTime = Shader.PropertyToID("_ScanlineTime");

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (crtMaterial != null)
            {
                crtMaterial.SetFloat(ScanlineTime, Time.unscaledTime);
                Graphics.Blit(source, destination, crtMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}