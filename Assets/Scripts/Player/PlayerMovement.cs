using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameProg.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] [Range(0.1f,3f)] private float moveSpeed = 1f;
        
        private PlayerControls _playerControls;
        private Vector2 _movementInput;
        private Coroutine _moveCoroutine;

        private void Start()
        {
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
        }
        
        private IEnumerator MoveCoroutine()
        {
            while (true)
            {
                //move player
                transform.position += new Vector3(
                    _movementInput.x * moveSpeed * Time.deltaTime, 
                    _movementInput.y * moveSpeed * Time.deltaTime, 
                    0f);
                
                yield return null;
            }
        }
    }
}
