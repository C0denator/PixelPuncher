using GameProg.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace General
{
    public class PauseMenu : MonoBehaviour
    {
        private PlayerControls _controls;
        [SerializeField] private GameObject _pauseMenu;
        
        private bool _isPaused = false;
        private Canvas _canvas;
        
        private void Awake()
        {
            _controls = new PlayerControls();
            
            if(_pauseMenu == null) Debug.LogError("PauseMenu reference not set");
            
            //set render camera of the canvas
            _canvas = _pauseMenu.GetComponent<Canvas>();
            
            if(_canvas == null) Debug.LogError("Canvas component not found on PauseMenu");
            
            _canvas.worldCamera = Camera.main;
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
        
        private void Pause(InputAction.CallbackContext context)
        {
            if(!_isPaused && Time.timeScale != 0) //prevent pausing when in cutscenes
            {
                Time.timeScale = 0;
                _isPaused = true;
                _pauseMenu.SetActive(true);
                
            }else if(_isPaused && Time.timeScale == 0)
            {
                Time.timeScale = 1;
                _isPaused = false;
                _pauseMenu.SetActive(false);
            }
            else
            {
                Debug.Log("Cant pause right now");
            }
        }
        
        public void Unpause()
        {
            Time.timeScale = 1;
            _isPaused = false;
            _pauseMenu.SetActive(false);
        }
    }
}
