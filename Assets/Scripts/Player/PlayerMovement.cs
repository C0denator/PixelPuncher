using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GameProg.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] [Range(0.1f,10f)] private float moveSpeed = 5f;
        
        //Idle animation
        [SerializeField] private AnimatorController _idleAnimation;
        
        //Walk animation
        [SerializeField] private AnimatorController _walkAnimation;
        
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Rigidbody2D _rigidbody2D;
        
        private PlayerControls _playerControls;
        private Vector2 _movementInput;
        private Coroutine _moveCoroutine;

        private void Start()
        {
            //get References
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            if(_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found");
            if(_animator == null) Debug.LogError("Animator component not found");
            if(_rigidbody2D == null) Debug.LogError("Rigidbody2D component not found");
            if(_idleAnimation == null) Debug.LogError("Idle animation not set");
            if(_walkAnimation == null) Debug.LogError("Walk animation not set");
            
                
            //enable player controls
            _playerControls = new PlayerControls();
            _playerControls.Player.Movement.Enable();
            _playerControls.Player.Shoot.Enable();
            _playerControls.Player.Dash.Enable();
            
            //subscribe to movement input
            _playerControls.Player.Movement.performed += HandleOnMovementPerformed;
            _playerControls.Player.Movement.canceled += HandleOnMovementCanceled;
            
            //set idle animation
            _animator.runtimeAnimatorController = _idleAnimation;
        }
        
        private void HandleOnMovementPerformed(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
            
            //start MoveCoroutine
            if (_moveCoroutine == null)
            {
                _moveCoroutine = StartCoroutine(MoveCoroutine());
            }
        }
        
        private void HandleOnMovementCanceled(InputAction.CallbackContext context)
        {
            _movementInput = Vector2.zero;
            
            //stop MoveCoroutine
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }
            else
            {
                Debug.LogWarning("Move coroutine is not running");
            }
            
            //set idle animation
            _animator.runtimeAnimatorController = _idleAnimation;
        }
        
        private IEnumerator MoveCoroutine()
        {
            //set walk animation
            _animator.runtimeAnimatorController = _walkAnimation;
            
            while (true)
            {
                //move player
                Vector3 vel = new Vector3(
                    _movementInput.x * moveSpeed * Time.deltaTime, 
                    _movementInput.y * moveSpeed * Time.deltaTime, 
                    0f);
                
                _rigidbody2D.MovePosition(transform.position + vel);
                
                //flip sprite if moving left
                if (_movementInput.x < 0)
                {
                    _spriteRenderer.flipX = true;
                }
                else if (_movementInput.x > 0)
                {
                    _spriteRenderer.flipX = false;
                }
                
                yield return null;
            }
        }
    }
}
