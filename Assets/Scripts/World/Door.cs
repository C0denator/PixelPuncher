using System;
using JetBrains.Annotations;
using Sound;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameProg.World
{
    public class Door : MonoBehaviour
    {
        public Room roomA; //the room this door belongs to
        [CanBeNull] public Room roomB; //the room this door leads to
        private Animator animator;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private AudioClipWithVolume deniedSound;

        private bool _markedForDeletion = false; //if true, the door will be deleted in the next frame
        //(fixed a bug where doors would still be used after being deleted)
        
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");
        
        public bool WasUsedInGeneration;
        
        private AudioSource _audioSource;
        

        public void Initialize()
        {
            //return if the door is marked for deletion
            if (_markedForDeletion) return;
            
            //get references
            roomA = GetComponentInParent<Room>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _audioSource = GlobalSound.globalAudioSource;
            
            //check if references are set
            if(roomA == null) Debug.LogError("Room component not found in parent");
            if(_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found");
            if(animator == null) Debug.LogError("Animator component not found");
            if(_boxCollider2D == null) Debug.LogError("BoxCollider2D component not found");
            
            //look, if another door touches this door
            if (roomB == null)
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, _boxCollider2D.size, 0);
                bool found = false;
                
                //look if one gameobject has the tag "Door"
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Door"))
                    {
                        //is the gameobject not this gameobject?
                        if (collider.gameObject != gameObject)
                        {
                            found = true;
                            
                            //get the door component
                            Door otherDoor = collider.GetComponent<Door>();
                            
                            //set the roomB of the other door to this roomB
                            roomB = otherDoor.roomA;
                            
                            Debug.Log("Found overlapping door: "+collider.gameObject.name);

                            if (roomB == null)
                            {
                                Debug.LogError("Other door has no room component");
                                return;
                            }
                            
                            //delete all tiles touching the collider of this door
                            Tilemap tilemapA = roomA.Walls;
                            Vector2 colliderSize = _boxCollider2D.size;
                            Vector2 bottomLeft = (Vector2)transform.position - colliderSize/2;
                            
                            Debug.Log("Looking for tiles to delete on Room A...");
                            
                            for(int i=0; i<_boxCollider2D.size.y; i++)
                            {
                                for(int j=0; j<_boxCollider2D.size.x; j++)
                                {
                                    tilemapA.SetTile(tilemapA.WorldToCell(bottomLeft + new Vector2(j, i)), null);
                                    
                                    Debug.Log("Deleted tile at: "+(bottomLeft + new Vector2(j, i)));
                                    Debug.Log("i: "+i+" j: "+j + " size: "+_boxCollider2D.size);
                                }
                            }
                            
                            Tilemap tilemapB = roomB.Walls;
                            colliderSize = otherDoor._boxCollider2D.size;
                            bottomLeft = (Vector2)otherDoor.transform.position - colliderSize/2;
                            
                            Debug.Log("Looking for tiles to delete on Room B...");
                            
                            for(int i=0; i<otherDoor._boxCollider2D.size.y; i++)
                            {
                                for(int j=0; j<otherDoor._boxCollider2D.size.x; j++)
                                {
                                    tilemapB.SetTile(tilemapB.WorldToCell(bottomLeft + new Vector2(j, i)), null);
                                    
                                    Debug.Log("Deleted tile at: "+(bottomLeft + new Vector2(j, i)));
                                    Debug.Log("i: "+i+" j: "+j + " size: "+otherDoor._boxCollider2D.size);
                                }
                            }
                            
                            //add this door to the list of other room
                            otherDoor.roomA.Doors.Add(this);
                            
                            //remove the other door from the list of the other room
                            otherDoor.roomA.Doors.Remove(otherDoor);
                            
                            //delete the other door
                            otherDoor._markedForDeletion = true;
                            Destroy(collider.gameObject);
                            
                            //break the loop
                            break;
                        }
                    }
                }
                
                //if no other door was found, destroy this door
                if (!found)
                {
                    Debug.Log("No overlapping door found, destroying this door");
                    //roomA.Doors.Remove(this);
                    _markedForDeletion = true;
                    Destroy(gameObject);
                }
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

        public void Open()
        {
            //return if the door is marked for deletion
            if (_markedForDeletion) return;
            
            //disable collider
            _boxCollider2D.enabled = false;
            
            //play animation
            animator.SetBool(IsOpen, true);
        }
        
        public void Close()
        {
            //return if the door is marked for deletion
            if (_markedForDeletion) return;
            
            //enable collider
            _boxCollider2D.enabled = true;
            
            //play animation
            animator.SetBool(IsOpen, false);
        }
        
        public void CheckVisibility()
        {
            //return if the door is marked for deletion
            if (_markedForDeletion) return;
            
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
            if (_markedForDeletion) return;
            
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
            if (_markedForDeletion) return;
            
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
        
        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
        
    }
}
