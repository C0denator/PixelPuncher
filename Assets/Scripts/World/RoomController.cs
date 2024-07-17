using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameProg.World
{
    public class RoomController : MonoBehaviour
    {
        [SerializeField] private RoomType roomType;
        
    
        private TilemapRenderer _tilemapRenderer;
        
        private bool _wasVisited;
    
        // Start is called before the first frame update
        void Start()
        {
            //get references
            _tilemapRenderer = GetComponentInChildren<TilemapRenderer>();
        
            if(_tilemapRenderer == null) Debug.LogError("TilemapRenderer component not found");
        }

        private void OnValidate()
        {
            if (roomType == RoomType.Combat)
            {
                //does the gameobject already have a CombatSettings component? if not create one
                if (GetComponent<CombatSettings>() == null)
                {
                    gameObject.AddComponent<CombatSettings>();
                }
            }
        }
    }
}
