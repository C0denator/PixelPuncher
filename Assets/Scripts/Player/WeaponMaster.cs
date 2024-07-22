using UnityEngine;
using UnityEngine.InputSystem;

namespace GameProg.Player
{
    public class WeaponMaster : MonoBehaviour
    {
        [SerializeField] private Reload _reloadBar;
        [SerializeField] private GameObject _currentWeapon;
        private IWeapon _currentWeaponScript;
        private PlayerControls _playerControls;
        
        // Start is called before the first frame update
        void Start()
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
            
            //subscribe to the shoot event
            _playerControls = new PlayerControls();
            _playerControls.Player.Shoot.performed += HandleShoot;
            _playerControls.Player.Shoot.Enable();
            
        }
        
        private void HandleShoot(InputAction.CallbackContext context)
        {
            _currentWeaponScript.HandleShoot();
        }
    }
}
