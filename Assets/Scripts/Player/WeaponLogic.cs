using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace GameProg.Player
{
    public class WeaponLogic : MonoBehaviour, IWeapon
    {
        [Header("References")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Reload reload;
        [Header("Settings")]
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float damage = 1f;
        [SerializeField] private bool hasMaganize = false;
        [SerializeField] private int maganizeAmount = 10;
        
        public event Action OnReloadStart;
        
        // Start is called before the first frame update
        void Start()
        {
            //get references
            reload = transform.parent.GetComponentInChildren<Reload>();
            
            //error handling
            if (reload == null)
            {
                Debug.LogWarning("Reload not set");
                return;
            }
            
            if (bulletPrefab == null)
            {
                Debug.LogWarning("Bullet prefab not set");
                return;
            }
        }
        
        public void HandleShoot()
        {
            Debug.Log("Shooting");
        }
    }
}
