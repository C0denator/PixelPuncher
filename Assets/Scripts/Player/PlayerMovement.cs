using GameProg.General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameProg.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] [Range(0.1f,10f)] private float moveSpeed = 5f;
        [SerializeField] [Range(20f,50f)] private float dashSpeed = 5f;
        [SerializeField] [Range(0.1f,0.5f)] private float dashDuration = 0.5f;
        [SerializeField] [Range(0f,3f)] private float dashCooldown = 1f;
        
        private Health.Health _health;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Rigidbody2D _rigidbody2D;
        private GameMaster _gameMaster;
        
        private PlayerControls _playerControls;
        private Vector2 _movementInput;
        
        private bool _isDashing;
        private float _elapsedDashTime;
        private float _elapsedDashCooldown;
        private Vector2 _dashDirection;
        
        private float _velocity;
        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int IsDashing = Animator.StringToHash("isDashing");

        private void Start()
        {
            //get References
            _health = GetComponent<Health.Health>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _gameMaster = FindObjectOfType<GameMaster>();
            
            //error handling
            if(_health == null) Debug.LogError("Health component not found");
            if(_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found");
            if(_animator == null) Debug.LogError("Animator component not found");
            if(_rigidbody2D == null) Debug.LogError("Rigidbody2D component not found");
            if(playerCamera == null) Debug.LogError("Player camera not set");
            if(_gameMaster == null) Debug.LogError("GameMaster not found");
            
                
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
            _elapsedDashTime = 0;
            _elapsedDashCooldown = 0;
            
            //switch to player camera
            playerCamera.gameObject.SetActive(true);
            _gameMaster.CurrentCamera = playerCamera;
        }

        private void Update()
        {
            //get mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            
            //get direction from player to mouse
            Vector3 direction = mousePos - transform.position;
            
            //flip sprite if mouse is on the left side
            if (direction.x < 0)
            {
                _spriteRenderer.flipX = true;
            }
            else
            {
                _spriteRenderer.flipX = false;
            }
        }

        private void FixedUpdate()
        {
            //dash logic
            if (_isDashing)
            {
                _elapsedDashTime += Time.fixedDeltaTime;
                
                if (_elapsedDashTime >= dashDuration) //dash is over
                {
                    _isDashing = false;
                    _animator.SetBool(IsDashing, false);
                    _elapsedDashTime = 0;
                    
                    //enable health component
                    _health.enabled = true;
                }
                else //dash still active
                {
                    //move player
                    Vector2 move = _dashDirection * (dashSpeed * Time.fixedDeltaTime);
                    _rigidbody2D.MovePosition((Vector2)transform.position + move);
                    
                    return;
                }
            }else if (_elapsedDashCooldown < dashCooldown)
            {
                //increase cooldown counter
                _elapsedDashCooldown += Time.fixedDeltaTime;
            }
            
            //move player
            Vector3 vel = new Vector3(
                _movementInput.x * moveSpeed * Time.fixedDeltaTime,
                _movementInput.y * moveSpeed * Time.fixedDeltaTime,
                0f);
            _rigidbody2D.MovePosition(transform.position + vel);
                
            //set velocity for animator
            _animator.SetFloat(Velocity, _movementInput.magnitude);
            
        }

        private void HandleOnMovementPerformed(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
        }
        
        private void HandleOnMovementCanceled(InputAction.CallbackContext context)
        {
            _movementInput = Vector2.zero;
            
            //set animator-velocity to 0
            _animator.SetFloat(Velocity, 0);
            
        }
        
        private void HandleOnDashPerformed(InputAction.CallbackContext context)
        {
            if (!_isDashing && _movementInput != Vector2.zero && _elapsedDashCooldown >= dashCooldown)
            {
                _isDashing = true;
                _elapsedDashTime = 0;
                _elapsedDashCooldown = 0;
                _animator.SetBool(IsDashing, true);
                
                //disable health component
                _health.enabled = false;
                
                _dashDirection = _movementInput.normalized;
            }
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
