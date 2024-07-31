using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace World
{
    /// <summary>
    /// Component to control a room in the game. Holds references to doors, walls and the world it is in.
    /// </summary>
    public class Room : MonoBehaviour
    {
        [SerializeField] private RoomType roomType;
        [SerializeField] private List<Door> doors; //all doors in the room
        public TilemapRenderer wallsRenderer;
        public Tilemap Walls; //the walls of the room
        [SerializeField] private CompositeCollider2D spaceCollider; //the inside of the room (trigger)
        
        public World World { get; private set; }
        public RoomType RoomType => roomType;
        public List<Door> Doors => doors;

        public bool WasVisited;
        
        
        public System.Action OnRoomEnter;
        public System.Action OnRoomExit;

        public System.Action OnRoomCleared; 
        
        /// <summary>
        /// Called during world generation, after all rooms have been placed
        /// </summary>
        public void InitializeRoom()
        {
            //get references
            World = GetComponentInParent<World>();
            
            //check for a CombatController component and initialize it
            if (roomType == RoomType.Combat || roomType == RoomType.Boss)
            {
                CombatController combatController = GetComponent<CombatController>();
                
                if (combatController == null)
                {
                    Debug.LogError("Room "+name+" is of type Combat but has no CombatController component");
                }
                else
                {
                    combatController.Init();
                }
            }
            
            if(World == null) Debug.LogError("World component not found in parent");
            if(wallsRenderer == null) Debug.LogError("TilemapRenderer component not found");
            if(spaceCollider == null) Debug.LogError("SpaceCollider component not found");
            if(doors.Count == 0) Debug.LogWarning("No doors set in room "+name);
            
        }
        
        /// <summary>
        /// Called during world generation, after all rooms have been initialized
        /// </summary>
        public void InitializeDoors()
        {
            Debug.Log("Initializing doors for room "+name);
            
            if (doors.Count == 0)
            {
                Debug.LogWarning("No doors set in room "+name);
                return;
            }
            
            //initialize doors
            for(int i = 0; i < doors.Count; i++)
            {
                Debug.Log("Initializing door "+doors[i].name);
                doors[i].Initialize();
                Debug.Log("Door "+doors[i].name+" initialized");
            }
        }

        public bool IsPlayerInRoom(Vector3 playerPosition)
        {
            return spaceCollider.OverlapPoint(playerPosition);
        }
        
        public void LockDoors()
        {
            for(int i = 0; i < doors.Count; i++)
            {
                doors[i].Close();
            }
        }
        
        public void OpenDoors()
        {
            for(int i = 0; i < doors.Count; i++)
            {
                doors[i].Open();
            }
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
        
        public bool IsCollidingWithRoom(Room otherRoom)
        {
            return spaceCollider.bounds.Intersects(otherRoom.spaceCollider.bounds);
        }

        private void OnValidate()
        {
            if (roomType == RoomType.Combat)
            {
                //does the gameobject already have a CombatSettings component? if not create one
                if (GetComponent<CombatController>() == null)
                {
                    gameObject.AddComponent<CombatController>();
                }
            }
        }
    }
}
