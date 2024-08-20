using GameProg.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace General
{
    public class PauseMenu : MonoBehaviour
    {
        private PlayerControls _controls;
        
        public static event System.Action OnPauseStart;
        public static event System.Action OnPauseEnd;
        
        private bool _isPaused = false;
        
        private void Awake()
        {
            _controls = new PlayerControls();
        }
        
        private void OnEnable()
        {
            _controls.Enable();
            _controls.Player.Pause.performed += Pause;
        }
        
        private void OnDisable()
        {
            _controls.Disable();
            _controls.Player.Pause.performed -= Pause;
        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        private void Pause(InputAction.CallbackContext context)
        {
            if(!_isPaused && Time.timeScale != 0) //prevent pausing when in cutscenes
            {
                Time.timeScale = 0;
                _isPaused = true;
                OnPauseStart?.Invoke();
            }else if(_isPaused && Time.timeScale == 0)
            {
                Time.timeScale = 1;
                _isPaused = false;
                OnPauseEnd?.Invoke();
            }
            else
            {
                Debug.Log("Cant pause right now");
            }
        }
    }
}
