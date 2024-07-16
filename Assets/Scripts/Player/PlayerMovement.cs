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
        
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Rigidbody2D _rigidbody2D;
        
        private PlayerControls _playerControls;
        private Vector2 _movementInput;
        private Coroutine _moveCoroutine;

        private float _velocity;
        private static readonly int Velocity = Animator.StringToHash("Velocity");

        private void Start()
        {
            //get References
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            if(_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found");
            if(_animator == null) Debug.LogError("Animator component not found");
            if(_rigidbody2D == null) Debug.LogError("Rigidbody2D component not found");
            
                
            //enable player controls
            _playerControls = new PlayerControls();
            _playerControls.Player.Movement.Enable();
            _playerControls.Player.Shoot.Enable();
            _playerControls.Player.Dash.Enable();
            
            //subscribe to movement input
            _playerControls.Player.Movement.performed += HandleOnMovementPerformed;
            _playerControls.Player.Movement.canceled += HandleOnMovementCanceled;
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
            
            //set velocity to 0
            _velocity = 0;
            _animator.SetFloat(Velocity, _velocity);
            
            
        }
        
        private IEnumerator MoveCoroutine()
        {
            
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
                
                //set velocity for animator
                _velocity = _movementInput.magnitude;
                _animator.SetFloat(Velocity, _velocity);
                
                yield return null;
            }
        }
    }
}
