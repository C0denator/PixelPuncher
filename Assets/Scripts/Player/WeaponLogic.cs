using System.Collections;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace GameProg.Player
{
    public class WeaponLogic : MonoBehaviour, IWeapon
    {
        [Header("References")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private AudioClip shootSound;
        [SerializeField] [Range(0f,1f)] private float shootVolume = 1f;
        [Header("Settings")]
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float secondsBetweenShots = 0.5f;
        [SerializeField] private float damage = 1f;
        [SerializeField] private bool hasMagazine = false;
        [SerializeField] private float reloadTime = 2f;
        [SerializeField] private int magazineAmount = 10;
        [SerializeField] private int currentMagazine;
        
        private AudioSource _audioSource;
        private bool _isBulletInMagazine = true;
        private bool _cooldownActive = false;
        
        private Coroutine _reloadCoroutine;
        private Coroutine _cooldownCoroutine;
        
        //getters
        public float ReloadTime => reloadTime;
        
        public event Action OnReloadStart;
        public event Action OnReloadFinished;
        
        // Start is called before the first frame update
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            
            if(shootSound == null)
            {
                Debug.LogWarning("Shoot sound not set");
            }else if (_audioSource == null)
            {
                Debug.LogWarning("Audio source not found");
            }
            
            if (bulletPrefab == null)
            {
                Debug.LogError("Bullet prefab not set");
                return;
            }
            
            //set the current maganize
            currentMagazine = magazineAmount;
            
        }
        
        public void HandleShoot()
        {
            if(!_isBulletInMagazine || _cooldownActive) return;
            
            Debug.Log("Shooting");
            //play the shoot once
            _audioSource.PlayOneShot(shootSound, shootVolume);
            
            //create a bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            
            //set the rotation of the bullet to the rotation of the weapon
            bullet.transform.rotation = transform.rotation;
            
            //set the bullet speed
            bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;

            if (hasMagazine)
            {
                currentMagazine--;
            
                //check if the maganize is empty
                if (currentMagazine <= 0)
                {
                    //invoke the reload event
                    OnReloadStart?.Invoke();
                
                    //start the reload coroutine

                    if (_reloadCoroutine == null)
                    {
                        _reloadCoroutine = StartCoroutine(Reload());
                    }
                }
            }
            
            //start the cooldown coroutine
            if (_cooldownCoroutine == null)
            {
                _cooldownCoroutine = StartCoroutine(Cooldown());
            }
        }
        
        private IEnumerator Reload()
        {
            _isBulletInMagazine = false;
            
            //wait for the reload time
            yield return new WaitForSeconds(reloadTime);
            
            //reset the maganize
            currentMagazine = magazineAmount;
            
            _isBulletInMagazine = true;
            
            //trigger the event
            OnReloadFinished?.Invoke();
            
            _reloadCoroutine = null;
        }
        
        private IEnumerator Cooldown()
        {
            _cooldownActive = true;
            
            //wait for the cooldown time
            yield return new WaitForSeconds(secondsBetweenShots);
            
            _cooldownActive = false;
            
            _cooldownCoroutine = null;
        }
        
    }
}
