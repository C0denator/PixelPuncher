using System;
using System.Collections;
using Sound;
using UnityEngine;

namespace Enemies
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float timeToActivate = 1f;
        [SerializeField] private float timeToAim = 1f;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private AudioClipWithVolume explosionSound;
        [SerializeField] private ParticleSystem _particleSystem;
        
        private Transform _player;
        private Rigidbody2D _rb;
        private GameMaster _gameMaster;
        private AudioSource _audioSource;
        private float _elapsedTime = 0f;
        private bool _isActivated = false;
        
        // Start is called before the first frame update
        void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _rb = GetComponent<Rigidbody2D>();
            _gameMaster = FindObjectOfType<GameMaster>();
            _audioSource = FindObjectOfType<GlobalSound>().globalAudioSource;
            
            if (_player == null) Debug.LogError("Player not found");
            if (_rb == null) Debug.LogError("Rigidbody2D not found");
            if (_gameMaster == null) Debug.LogError("GameMaster not found");
            if (explosionPrefab == null) Debug.LogError("Explosion prefab not set");
            if (explosionSound.clip == null) Debug.LogError("Explosion sound not set");
            if (_audioSource == null) Debug.LogError("Audio source not found");
            if (_particleSystem == null) Debug.LogError("Particle system not found");
            
        }

        private void Start()
        {
            _isActivated = false;
            StartCoroutine(RocketCoroutine());
        }

        private IEnumerator RocketCoroutine()
        {
            Debug.Log("Rocket spawned");
            
            //wait for the rocket to activate
            yield return new WaitForSeconds(timeToActivate);
            
            Debug.Log("Rocket activated");
            
            //activate the rocket
            _isActivated = true;
            
            //enable ParticleSystem
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }
            
            //reset velocity
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            
            //get direction to player
            Vector3 direction = _player.position - transform.position;
            
            //get angle to player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            //aim at player in timeToAim seconds
            while (_elapsedTime < timeToAim)
            {
                _elapsedTime += Time.deltaTime;
                
                //rotate towards player
                float rotationZ = Mathf.Lerp(0, angle, _elapsedTime / timeToAim);
                
                transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
            }
            
            //shoot rocket
            _rb.velocity = transform.right * speed;
            
            yield return null;
            
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Player") || (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Door") && _isActivated))
            {
                //explode
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                
                //play explosion sound
                _audioSource.PlayOneShot(explosionSound.clip, explosionSound.volume);
                
                //destroy rocket
                Destroy(gameObject);
            }
            
        }

        private void OnEnable()
        {
            _elapsedTime = 0f;
            _isActivated = false;
            
            //disable ParticleSystem
            if (_particleSystem != null)
            {
                _particleSystem.Stop();
            }
        }
        
        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
    }
}
