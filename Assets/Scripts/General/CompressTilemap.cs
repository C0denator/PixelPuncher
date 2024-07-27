using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameProg.General
{
    [ExecuteInEditMode]
    public class CompressTilemap : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        // Update is called once per frame
        private void Update()
        {
            if(tilemap != null)
            {
                tilemap.CompressBounds();
            }
        }
    }
}
