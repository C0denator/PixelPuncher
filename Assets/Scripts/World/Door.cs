using JetBrains.Annotations;
using UnityEngine;

namespace GameProg.World
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Room roomA;
        [SerializeField] [CanBeNull] private Room roomB;
        private Animator animator;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider2D;
        
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");

        private void Start()
        {
            roomA = GetComponentInParent<Room>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            
            if(roomA == null) Debug.LogError("Room component not found in parent");
            if(_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found");
            if(animator == null) Debug.LogError("Animator component not found");
            if(_boxCollider2D == null) Debug.LogError("BoxCollider2D component not found");
            
            //is the animator bool parameter "IsOpen" true?
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
        
    }
}
