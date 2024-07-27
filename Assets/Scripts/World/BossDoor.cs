using UnityEngine;
namespace GameProg.World
{
    public class BossDoor : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _boxCollider2D;
        
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");
        private GameMaster _gameMaster;
        
        // Start is called before the first frame update
        void Awake()
        {
            _gameMaster = FindObjectOfType<GameMaster>();
            
            //error handling
            if (_gameMaster == null) Debug.LogError("GameMaster not found in BossDoor");
            if (animator == null) Debug.LogError("Animator not set in BossDoor");
            if (_spriteRenderer == null) Debug.LogError("SpriteRenderer not set in BossDoor");
            if (_boxCollider2D == null) Debug.LogError("BoxCollider2D not set in BossDoor");
        }

        public void Open()
        {
            Debug.Log("Opening Boss Door");
            
            //disable collider
            _boxCollider2D.enabled = false;
            
            //play animation
            animator.SetBool(IsOpen, true);
        }

        void OnEnable()
        {
            _gameMaster.OnAllRoomsCleared += Open;    
        }
        
        void OnDisable()
        {
            _gameMaster.OnAllRoomsCleared -= Open;
        }
    }
}
