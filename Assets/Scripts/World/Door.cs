using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameProg.World
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Room roomA; //the room this door belongs to
        [SerializeField] [CanBeNull] private Room roomB; //the room this door leads to
        private Animator animator;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider2D;
        
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            //get references
            roomA = GetComponentInParent<Room>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            
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
                            
                            //set the room of the other door to this roomB
                            roomB = otherDoor.roomA;
                            
                            Debug.Log("Found overlapping door: "+collider.gameObject.name);

                            if (roomB == null)
                            {
                                Debug.LogError("Other door has no room component");
                                return;
                            }
                            
                            //delete any tile touching the door
                            Tilemap tilemapA = roomA.Walls;
                            tilemapA.SetTile(tilemapA.WorldToCell(transform.position), null);
                            
                            Tilemap tilemapB = roomB.Walls;
                            tilemapB.SetTile(tilemapB.WorldToCell(otherDoor.transform.position), null);
                            
                            //delete the other door
                            DestroyImmediate(collider.gameObject);
                            
                            //break the loop
                            break;
                        }
                    }
                }
                
                //if no other door was found, destroy this door
                if (!found)
                {
                    Debug.Log("No overlapping door found, destroying this door");
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
            //disable collider
            _boxCollider2D.enabled = false;
            
            //play animation
            animator.SetBool(IsOpen, true);
        }
        
        public void Close()
        {
            //enable collider
            _boxCollider2D.enabled = true;
            
            //play animation
            animator.SetBool(IsOpen, false);
        }
        
        public void CheckVisibility()
        {
            //check for null
            if (roomA == null || roomB == null)
            {
                Debug.LogError("RoomA or RoomB not set");
                return;
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
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log("Door started colliding with: "+other.gameObject.name);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            Debug.Log("Door stopped colliding with: "+other.gameObject.name);
        }
        
    }
}
