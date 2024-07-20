using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GameProg.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] [Range(0.1f,10f)] private float moveSpeed = 5f;
        [SerializeField] [Range(20f,50f)] private float dashSpeed = 5f;
        [SerializeField] [Range(0.1f,0.5f)] private float dashLength = 0.5f;
        
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Rigidbody2D _rigidbody2D;
        
        private PlayerControls _playerControls;
        private Vector2 _movementInput;
        private Coroutine _moveCoroutine;
        
        private bool _isDashing;
        private float _velocity;
        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int IsDashing = Animator.StringToHash("isDashing");

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
            
            //subscribe to dash input
            _playerControls.Player.Dash.performed += HandleOnDashPerformed;
            
            _isDashing = false;
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
            
            //set animator-velocity to 0
            _animator.SetFloat(Velocity, 0);
            
        }
        
        private void HandleOnDashPerformed(InputAction.CallbackContext context)
        {
            if (!_isDashing && _movementInput != Vector2.zero)
            {
                StartCoroutine(DashCoroutine());
            }
        }
        
        private IEnumerator MoveCoroutine()
        {
            
            while (true)
            {
                //do nothing when the player is dashing
                if (_isDashing) yield return null;
                
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
                _animator.SetFloat(Velocity, _movementInput.magnitude);
                
                yield return null;
            }
        }
        
        private IEnumerator DashCoroutine()
        {
            _isDashing = true;
            _animator.SetBool(IsDashing, true);
            
            Vector2 direction = _movementInput.normalized;
            
            //Debug.Log("Dashing started, Direction: "+direction);
            
            //dash until dashLength is reached
            float time = 0;
            
            while (time < dashLength)
            {
                time += Time.deltaTime;
                Vector2 move = direction * (dashSpeed * Time.deltaTime);
                _rigidbody2D.MovePosition((Vector2)transform.position + move);
                
                yield return null;
            }
            
            _isDashing = false;
            _animator.SetBool(IsDashing, false);
            
            
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            //Debug.Log("Player started colliding with: "+other.gameObject.name);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            //Debug.Log("Player stopped colliding with: "+other.gameObject.name);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //Debug.Log("Player started trigger with: "+other.gameObject.name);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            //Debug.Log("Player stopped trigger with: "+other.gameObject.name);
        }
    }
}
