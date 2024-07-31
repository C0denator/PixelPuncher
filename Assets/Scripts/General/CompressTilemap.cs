using UnityEngine;
using UnityEngine.Tilemaps;

namespace General
{
    /// <summary>
    /// Compresses the bounds of a tilemap in edit mode, because the bounds are otherwise not updated
    /// </summary>
    [ExecuteInEditMode]
    public class CompressTilemap : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        
        #if UNITY_EDITOR

        // Update is called once per frame
        private void Update()
        {
            if(tilemap != null)
            {
                tilemap.CompressBounds();
            }
        }
        
        #endif
    }
}
