using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameProg.World
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private RoomType roomType;
        [SerializeField] private List<Door> doors;
    
        private World _world;
        private TilemapRenderer _tilemapRenderer;
        
        public World World => _world;
        
        private bool _wasVisited;
    
        // Start is called before the first frame update
        void Start()
        {
            //get references
            _world = GetComponentInParent<World>();
            _tilemapRenderer = GetComponentInChildren<TilemapRenderer>();
            
            if(_world == null) Debug.LogError("World component not found in parent");
            if(_tilemapRenderer == null) Debug.LogError("TilemapRenderer component not found");
            
            //is starting room? then set current room
            if (roomType == RoomType.Start)
            {
                _world.CurrentRoom = this;
            }
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
