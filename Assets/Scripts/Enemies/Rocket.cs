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
        private LineRenderer _lineRenderer;
        private float _elapsedTime = 0f;
        private bool _isActivated = false;
        
        // Start is called before the first frame update
        void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _rb = GetComponent<Rigidbody2D>();
            _gameMaster = FindObjectOfType<GameMaster>();
            _audioSource = FindObjectOfType<GlobalSound>().globalAudioSource;
            _lineRenderer = GetComponent<LineRenderer>();
            
            if (_player == null) Debug.LogError("Player not found");
            if (_rb == null) Debug.LogError("Rigidbody2D not found");
            if (_gameMaster == null) Debug.LogError("GameMaster not found");
            if (explosionPrefab == null) Debug.LogError("Explosion prefab not set");
            if (explosionSound.clip == null) Debug.LogError("Explosion sound not set");
            if (_audioSource == null) Debug.LogError("Audio source not found");
            if (_particleSystem == null) Debug.LogError("Particle system not found");
            if (_lineRenderer == null) Debug.LogError("LineRenderer not found");
            
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
                
                direction = _player.position - transform.position;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
                //rotate towards player
                float rotationZ = Mathf.Lerp(0, angle, _elapsedTime / timeToAim);
                
                transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
                
                //draw line out of rocket till wall
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, LayerMask.GetMask("Wall"));
                
                if (hit.collider != null)
                {
                    _lineRenderer.SetPosition(0, transform.position);
                    _lineRenderer.SetPosition(1, hit.point);
                }
                else
                {
                   Debug.LogWarning("No wall found");
                }
                
                yield return null;
            }
            
            //aim at player
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
            
            //delete line
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.zero);

            if (_gameMaster.GigachadMode)
            {
                //shoot rocket
                _rb.velocity = transform.right * speed * 2;
            }
            else
            {
                //shoot rocket
                _rb.velocity = transform.right * speed;
            }
            
            
            
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
                
                //if player is hit deal damage
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<Health.Health>().DoDamage(1);
                }

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
