using GameProg.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Sets the current weapon and handles the shooting input and reloading events
    /// </summary>
    public class WeaponMaster : MonoBehaviour
    {
        [SerializeField] private Reload _reloadBar;
        [SerializeField] private GameObject _currentWeapon;
        private IWeapon _currentWeaponScript;
        private PlayerControls _playerControls;
        
        private void Awake()
        {
            _playerControls = new PlayerControls();
            _currentWeaponScript = _currentWeapon.GetComponent<IWeapon>();
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            //try to get missing references
            if (_reloadBar == null)
            {
                _reloadBar = GetComponentInChildren<Reload>();
                
                if (_reloadBar == null)
                {
                    Debug.LogError("Reload bar not set");
                    return;
                }
            }
            
            if(_currentWeapon == null)
            {
                //look for a gameobject with the tag "Weapon"
                _currentWeapon = GameObject.FindGameObjectWithTag("Weapon");
                
                if (_currentWeapon == null)
                {
                    Debug.LogError("Weapon not set");
                    return;
                }
                else
                {
                    _currentWeaponScript = _currentWeapon.GetComponent<IWeapon>();

                    if (_currentWeaponScript == null)
                    {
                        Debug.LogError("Weapon script not found on Weapon");
                    }
                    
                }
            }
            else
            {
                _currentWeaponScript = _currentWeapon.GetComponent<IWeapon>();
                
                if (_currentWeaponScript == null)
                {
                    Debug.LogError("Weapon script not found on Weapon");
                }
            }
        }
        
        private void HandleShoot(InputAction.CallbackContext context)
        {
            if(Time.timeScale == 0) return;
            _currentWeaponScript.HandleShoot();
        }

        private void HandleOnReloadStart()
        {
            _reloadBar.StartReload(_currentWeaponScript.ReloadTime);
        }
        
        private void HandleOnReloadFinished()
        {
            _reloadBar.FinishReload();
        }

        private void OnEnable()
        {
            //subscribe to the shoot event
            _playerControls.Player.Shoot.performed += HandleShoot;
            _playerControls.Player.Shoot.Enable();
            
            //subscribe to the reload event
            if (_currentWeaponScript != null)
            {
                _currentWeaponScript.OnReloadStart += HandleOnReloadStart;
                _currentWeaponScript.OnReloadFinished += HandleOnReloadFinished;
            }
        }

        private void OnDisable()
        {
            Debug.Log("Scene unloaded");
            
            //unsubscribe from the shoot event
            _playerControls.Player.Shoot.performed -= HandleShoot;
            _playerControls.Player.Shoot.Disable();
            
            _currentWeaponScript.OnReloadStart -= HandleOnReloadStart;
            _currentWeaponScript.OnReloadFinished -= HandleOnReloadFinished;
        }
    }
}
