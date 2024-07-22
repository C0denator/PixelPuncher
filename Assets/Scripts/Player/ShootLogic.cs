using UnityEngine;

namespace GameProg.Player
{
    public class ShootLogic : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float damage = 1f;
        [SerializeField] private bool hasMaganize = false;
        [SerializeField] private int maganizeAmount = 10;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
