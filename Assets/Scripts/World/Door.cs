using System;
using JetBrains.Annotations;
using Sound;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace World
{
    /// <summary>
    /// Component to handle doors between rooms. Each door has a reference to the room it belongs to and the room it leads to.
    /// </summary>
    public class Door : MonoBehaviour
    {
        public Room roomA; //the room this door originally belongs to
        [CanBeNull] public Room roomB; //the room this door leads to; is null before world generation
        private Animator animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private AudioClipWithVolume deniedSound;
        [SerializeField] private bool isBossDoor;
        
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        
        //is the door marked for deletion? Fixes the issue of Destroy() not being instant
        public bool MarkedForDeletion = false; 
        
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");
        
        private static int roomCount = 0;
        
        public bool WasUsedInGeneration;
        
        private AudioSource _audioSource;

        private void Awake()
        {
            //error handling
            if (_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found");
            if (_boxCollider2D == null) Debug.LogError("BoxCollider2D component not found");
            if (deniedSound.clip == null) Debug.LogError("Denied sound not set");
            if(roomA == null) Debug.LogError("Room A not set");
            
            //set name
            gameObject.name = "Door " + roomCount;
            roomCount++;
        }

        /// <summary>
        /// Called during world generation to initialize the door. The door looks for other doors or walls to connect and reference them.
        /// </summary>
        public void Initialize()
        {
            //return if the door is marked for deletion
            if (MarkedForDeletion) return;
            
            //return if roomB was already set
            if (roomB) return;
            
            //get references
            animator = GetComponent<Animator>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _audioSource = FindObjectOfType<GlobalSound>().globalAudioSource;
            
            //check if references are set
            if(roomA == null) Debug.LogError("Room component not found in parent");
            if(_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found");
            if(animator == null) Debug.LogError("Animator component not found");
            if(_boxCollider2D == null) Debug.LogError("BoxCollider2D component not found");
            
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, _boxCollider2D.size, 0);
            
            String names = "";
            foreach (Collider2D collider in colliders)
            {
                names += collider.gameObject.name + ", ";
            }
            Debug.Log("Door "+gameObject.name+" found colliders: "+names);

            bool found = false;
            
            //look for other wall
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject == gameObject) continue;

                /*if (collider.CompareTag("Wall") && 
                    collider.gameObject.transform.parent.gameObject != roomA.gameObject &&
                    collider.gameObject.transform.parent.GetComponent<Room>().RoomType != RoomType.Boss)
                {
                    //get the room component
                    Room otherRoom = collider.GetComponentInParent<Room>();

                    if (!otherRoom)
                    {
                        Debug.LogError("Room component not found in parent");
                    }
                    else
                    {
                        //set roomB
                        found = true;
                        roomB = otherRoom;
                        
                        //add itself to the list of the other room
                        roomB.Doors.Add(this);
                        
                        Debug.Log("Door "+gameObject.name+" found room "+roomB.name+" through wall "+collider.gameObject.name);
                        break;
                    }
                }else */
                if (collider.CompareTag("Door") && collider.gameObject != gameObject)
                {
                    //get the door component
                    Door otherDoor = collider.GetComponent<Door>();
                    
                    if (!otherDoor)
                    {
                        Debug.LogError("Door component not found");
                    }
                    else
                    {
                        found = true;
                        
                        if (_spriteRenderer.sortingOrder >= otherDoor.SpriteRenderer.sortingOrder)
                        {
                            //save hte reference to the other room
                            roomB = otherDoor.roomA;
                            
                            //add itself to the list of the other room
                            roomB?.Doors.Add(this);
                            
                            //mark other door for deletion
                            otherDoor.MarkedForDeletion = true;
                        }
                        else
                        {
                            Debug.Log("Sorting order of door "+gameObject.name+" is lower than "+otherDoor.name);
                            break;
                            
                            
                        }
                        
                        Debug.Log("Door "+gameObject.name+" found door "+otherDoor.name+" through door "+collider.gameObject.name);

                        break;
                    }
                }
            }

            if (!found)
            {

                if (WasUsedInGeneration)
                {
                    Debug.Log("Door "+gameObject.name+" was used in generation but no other door was found");
                }
                
                MarkedForDeletion = true;
                Debug.Log("Door "+gameObject.name+" was marked for deletion because no other door was found");
            }
            
            
            //open or close depending on the animator state
            if (animator.GetBool(IsOpen))
            {
                Open();
            }
            else
            {
                Close();
            }
        }
        
        /// <summary>
        /// Deletes all tiles of the room walls that are touching the collider of this door.
        /// </summary>
        public void DeleteOverlappingTiles()
        {
            if(MarkedForDeletion) return;
            
            //delete all tiles touching the collider of this door
            Tilemap tilemapA = roomA.Walls;
            Tilemap tilemapB = roomB?.Walls;
            
            Debug.Log("Looking for tiles to delete on position "+transform.position);
            
            //Update the tilemaps manually (because the collision would otherwise not be detected)
            tilemapA.RefreshAllTiles();
            tilemapB?.RefreshAllTiles();
            
            //delete tile by setting it to null
            tilemapA.SetTile(tilemapA.WorldToCell(transform.position), null);
            tilemapB?.SetTile(tilemapB.WorldToCell(transform.position), null);
        }

        public void Open()
        {
            if (isBossDoor) return;
            
            //return if the door is marked for deletion
            if (MarkedForDeletion) return;
            
            //disable collider
            _boxCollider2D.enabled = false;
            
            //play animation
            animator.SetBool(IsOpen, true);
        }
        
        public void Close()
        {
            //return if the door is marked for deletion
            if (MarkedForDeletion) return;
            
            //enable collider
            _boxCollider2D.enabled = true;
            
            //play animation
            animator.SetBool(IsOpen, false);
        }
        
        public void CheckVisibility()
        {
            //return if the door is marked for deletion
            if (MarkedForDeletion) return;
            
            //check for null
            if (roomA == null || roomB == null)
            {
                Debug.Log("RoomA or RoomB not set. Hiding this door now");
                _spriteRenderer.enabled = false;
            }
            
            //check if one of the rooms is the current room
            if(roomA.World.CurrentRoom == roomA || roomA.World.CurrentRoom == roomB)
            {
                _spriteRenderer.enabled = true;
            }
            else
            {
                _spriteRenderer.enabled = false;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Debug.Log("Door started colliding with: "+other.gameObject.name);
            //return if the door is marked for deletion
            if (MarkedForDeletion) return;
            
            //has the other gameobject the tag "Player"?
            if (other.gameObject.CompareTag("Player"))
            {
                //is the door currently locked?
                if (!animator.GetBool(IsOpen))
                {
                    //play denied sound
                    _audioSource.PlayOneShot(deniedSound.clip, deniedSound.volume);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            //Debug.Log("Door stopped colliding with: "+other.gameObject.name);
            //return if the door is marked for deletion
            if (MarkedForDeletion) return;
            
            //has the other gameobject the tag "Player"?
            if (other.gameObject.CompareTag("Player"))
            {
                //is the player in a room, that is not the current room?
                if (!roomA.World.CurrentRoom.IsPlayerInRoom(other.transform.position))
                {
                    Room formerRoom = null;
                    Room newRoom = null;
                    
                    if (roomA == roomA.World.CurrentRoom) //if A was the former current room
                    {
                        formerRoom = roomA;
                        newRoom = roomB;
                    }
                    else if(roomB == roomA.World.CurrentRoom) //if B was the former current room
                    {
                        formerRoom = roomB;
                        newRoom = roomA;
                    }
                    else
                    {
                        Debug.LogError("Current room is neither A nor B");
                    }
                    
                    formerRoom!.WasVisited = true;
                    
                    //set the new room as current room
                    roomA.World.CurrentRoom = newRoom;
                    
                    //show new room
                    newRoom!.Show();
                    
                    //fire events
                    formerRoom.OnRoomExit?.Invoke();
                    newRoom.OnRoomEnter?.Invoke();
                }
            }
        }

        public void Delete()
        {
            if (!MarkedForDeletion)
            {
                Debug.LogError("Door "+gameObject.name+" was not marked for deletion");
            }else 
            {
                Debug.Log("Deleting door "+gameObject.name);
            }
            
            //remove from lists
            roomA.Doors.Remove(this);
            roomB?.Doors.Remove(this);
            
            //delete the door
            Destroy(gameObject);
        }
        
        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
        
    }
}
