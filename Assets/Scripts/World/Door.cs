using JetBrains.Annotations;
using UnityEngine;

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
                
                //look if one gameobject has the tag "Door"
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Door"))
                    {
                        //is the gameobject not this gameobject?
                        if (collider.gameObject != gameObject)
                        {
                            Debug.Log("Found overlapping door: "+collider.gameObject.name);
                            
                            //get the door component
                            Door otherDoor = collider.GetComponent<Door>();
                            
                            //set the room of the other door to this roomB
                            roomB = otherDoor.roomA;
                            
                            if(roomB == null) Debug.LogError("Other door has no room component");
                            
                            //delete the other door
                            Destroy(collider.gameObject);
                            
                            //break the loop
                            break;
                        }
                    }
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
