using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameProg.General
{
    [ExecuteInEditMode]
    public class CompressTilemap : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        // Update is called once per frame
        void Update()
        {
            if(tilemap != null)
            {
                tilemap.CompressBounds();
            }
        }
    }
}
