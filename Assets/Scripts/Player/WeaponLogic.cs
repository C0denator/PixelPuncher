using System;
using System.Collections;
using Sound;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Component for a specific weapon. Holds all data for the weapon
    /// </summary>
    public class WeaponLogic : MonoBehaviour, IWeapon
    {
        [Header("References")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private AudioClipWithVolume shootSound;
        [Header("Settings")]
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float secondsBetweenShots = 0.5f;
        [SerializeField] private int damage = 1;
        [SerializeField] private bool hasMagazine = false;
        [SerializeField] private float reloadTime = 2f;
        [SerializeField] private int magazineAmount = 10;
        [SerializeField] private int currentMagazine;
        
        private bool _isBulletInMagazine = true;
        private bool _cooldownActive = false;
        
        private AudioSource _audioSource;
        
        private Coroutine _reloadCoroutine;
        private Coroutine _cooldownCoroutine;
        
        //getters
        public float ReloadTime => reloadTime;
        
        public event Action OnReloadStart;
        public event Action OnReloadFinished;
        
        // Start is called before the first frame update
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            
            if(shootSound.clip == null)
            {
                Debug.LogWarning("Shoot sound not set");
            }else if (_audioSource == null)
            {
                Debug.LogWarning("Audio source not found");
            }
            
            if (bulletSpawnPoint == null)
            {
                Debug.LogError("Bullet spawn point not set");
                return;
            }
            
            if (bulletPrefab == null)
            {
                Debug.LogError("Bullet prefab not set");
                return;
            }
            
            //set the current maganize
            currentMagazine = magazineAmount;
            
            //get the audio source
            _audioSource = FindObjectOfType<GlobalSound>().globalAudioSource;

        }
        
        public void HandleShoot()
        {
            if(!_isBulletInMagazine || _cooldownActive) return;
            
            //play the shoot once
            _audioSource.PlayOneShot(shootSound.clip, shootSound.volume);
            
            //create a bullet
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            
            //set the rotation of the bullet to the rotation of the weapon
            bullet.transform.rotation = transform.rotation;
            
            //set the bullet speed
            bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;
            
            //set the damage
            bullet.GetComponent<PlayerBullet>().Damage = damage;

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
        
        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
        
    }
}
