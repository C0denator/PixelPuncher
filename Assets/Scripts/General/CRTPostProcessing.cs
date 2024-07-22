using UnityEngine;

namespace GameProg.General
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]

    public class CRTPostProcessing : MonoBehaviour
    {
        public Material crtMaterial;
        [Range(0, 1)]
        public float curvature = 0.5f;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (crtMaterial != null)
            {
                crtMaterial.SetFloat("_Curvature", curvature);
                Graphics.Blit(source, destination, crtMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}