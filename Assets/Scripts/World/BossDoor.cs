using System.Collections;
using UnityEngine;
namespace GameProg.World
{
    public class BossDoor : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private float _openDistance;
        
        private Door _door;
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");
        private GameMaster _gameMaster;
        
        private Coroutine _openCoroutine;
        
        // Start is called before the first frame update
        private void Awake()
        {
            _gameMaster = FindObjectOfType<GameMaster>();
            player = GameObject.FindWithTag("Player").transform;
            _door = GetComponent<Door>();
            
            //error handling
            if (player == null) Debug.LogError("Player not set in BossDoor");
            if (_gameMaster == null) Debug.LogError("GameMaster not found in BossDoor");
            if (animator == null) Debug.LogError("Animator not set in BossDoor");
            if (_spriteRenderer == null) Debug.LogError("SpriteRenderer not set in BossDoor");
            if (_boxCollider2D == null) Debug.LogError("BoxCollider2D not set in BossDoor");
            if (_door == null) Debug.LogError("Door not found in BossDoor");
        }
        
        private void HandleOnAllRoomsCleared()
        {
            _openCoroutine = StartCoroutine(OpenCoroutine());
            
            //unsubscribe from the event
            _gameMaster.OnAllRoomsCleared -= HandleOnAllRoomsCleared;
        }
        
        private IEnumerator OpenCoroutine()
        {
            //is the player close enough?
            while (Vector2.Distance(player.position, transform.position) > _openDistance || 
                   (_door.roomA.World.CurrentRoom != _door.roomA && _door.roomB.World.CurrentRoom != _door.roomB))
            {
                yield return null;
            }
            
            Open();
        }

        public void Open()
        {
            Debug.Log("Opening Boss Door");
            
            //disable collider
            _boxCollider2D.enabled = false;
            
            //play animation
            animator.SetBool(IsOpen, true);
        }

        private void OnEnable()
        {
            _gameMaster.OnAllRoomsCleared += HandleOnAllRoomsCleared;
        }

        private void OnDisable()
        {
            _gameMaster.OnAllRoomsCleared -= HandleOnAllRoomsCleared;
            
            //stop the coroutine
            if(_openCoroutine != null) StopCoroutine(_openCoroutine);
        }
    }
}
