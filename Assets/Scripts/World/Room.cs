using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace GameProg.World
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private RoomType roomType;
        [SerializeField] private List<Door> doors;
        [FormerlySerializedAs("tilemapRenderer")] [SerializeField] private TilemapRenderer wallsRenderer;
        public Tilemap Walls;
        [SerializeField] private CompositeCollider2D spaceCollider; //the inside of the room

        public World World { get; private set; }

        private bool _wasVisited;
    
        // Start is called before the first frame update
        void Start()
        {
            //get references
            World = GetComponentInParent<World>();
            
            //find doors in children
            doors = new List<Door>();
            foreach (Transform child in transform)
            {
                Door door = child.GetComponent<Door>();
                if (door != null)
                {
                    doors.Add(door);
                }
            }
            
            if(World == null) Debug.LogError("World component not found in parent");
            if(wallsRenderer == null) Debug.LogError("TilemapRenderer component not found");
            if(spaceCollider == null) Debug.LogError("SpaceCollider component not found");
            
            //is starting room? then set current room
            if (roomType == RoomType.Start)
            {
                World.CurrentRoom = this;
                Show();
            }
            else
            {
                Hide();
            }
        }

        public bool IsPlayerInRoom(Vector3 playerPosition)
        {
            return spaceCollider.OverlapPoint(playerPosition);
        }

        public void Hide()
        {
            wallsRenderer.enabled = false;
            
            //hide doors
            for(int i = 0; i < doors.Count; i++)
            {
                //check for null
                if(doors[i] == null)
                {
                    doors.RemoveAt(i);
                    i--;
                    continue;
                }
                
                doors[i].CheckVisibility();
            }
        }
        
        public void Show()
        {
            wallsRenderer.enabled = true;
            
            //show doors
            for(int i = 0; i < doors.Count; i++)
            {
                //check for null
                if(doors[i] == null)
                {
                    doors.RemoveAt(i);
                    i--;
                    continue;
                }
                
                doors[i].CheckVisibility();
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
