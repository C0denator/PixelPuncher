using UnityEngine;
using UnityEngine.Tilemaps;

namespace General
{
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
