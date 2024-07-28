using System;
using System.Collections;
using System.Collections.Generic;
using GameProg.Enemies.SpecificBehaviour.BossAttacks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace GameProg.Enemies.SpecificBehaviour
{
    public class BossController : MonoBehaviour
    {
        [SerializeField] private GameObject eye;
        [SerializeField] private GameObject core;
        [SerializeField] [Range(0f,3f)] private float _secondsBetweenAttacks;
        [SerializeField] private List<BossAttack> _firstPhaseAttacks;
        [SerializeField] private List<BossAttack> _secondPhaseAttacks;
        [SerializeField] private AudioClipWithVolume _destroySound;
        [SerializeField] private List<GameObject> _destroyObjects; //objects to destroy on second phase
        [SerializeField] private Animator miniGunAnimator;
        [SerializeField] private Animator mortarAnimator;
        
        private Transform _player;
        private Health.Health _health;
        [SerializeField] private BossAttack _currentAttack;
        [SerializeField] private BossAttack _lastAttack;
        private NavMeshAgent _navMeshAgent;
        private Coroutine _waitCoroutine;
        private AudioSource _audioSource;
        private CircleCollider2D _collider;
        
        private float elapsedTimeBetweenAttacks;
        private bool _secondPhase;
        
        public Transform Player => _player;
        
        public GameObject Core => core;
        public Health.Health Health => _health;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        public CircleCollider2D Collider => _collider;
        public Animator MiniGunAnimator => miniGunAnimator;
        public Animator MortarAnimator => mortarAnimator;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _health = GetComponent<Health.Health>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _audioSource = FindObjectOfType<Sound.GlobalSound>().globalAudioSource;
            _collider = GetComponent<CircleCollider2D>();
            
            //error handling
            if (_player == null) Debug.LogError("Player not found in BossController");
            if (_health == null) Debug.LogError("Player health not found in BossController");
            if(_firstPhaseAttacks.Count == 0) Debug.LogError("No first phase attacks found in BossController");
            if(_secondPhaseAttacks.Count == 0) Debug.LogError("No second phase attacks found in BossController");
            if (_navMeshAgent == null) Debug.LogError("NavMeshAgent not found in BossController");
            if (_audioSource == null) Debug.LogError("Global audio source not found in BossController");
            if (_collider == null) Debug.LogError("Collider not found in BossController");
            if (miniGunAnimator == null) Debug.LogError("MiniGun animator not set in BossController");
            if (mortarAnimator == null) Debug.LogError("Mortar animator not set in BossController");
            
            //set up agent for 2D
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
        }

        private void Start()
        {
            _secondPhase = false;
            
            //start waiting coroutine
            _waitCoroutine = StartCoroutine(WaitCoroutine());
        }

        private void FixedUpdate()
        {
            //look at player
            Vector2 lookDir = _player.position - eye.transform.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            eye.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (_secondPhase)
            {
                //set rotation speed depending on agent velocity
                float rotationSpeed = _navMeshAgent.velocity.magnitude * 2;
                
                //moving left or right
                if (_navMeshAgent.velocity.x > 0.1f)
                {
                    core.transform.Rotate(Vector3.forward, rotationSpeed);
                }
                else if (_navMeshAgent.velocity.x < -0.1f)
                {
                    core.transform.Rotate(Vector3.forward, -rotationSpeed);
                }
            }
        }

        private IEnumerator WaitCoroutine()
        {
            _currentAttack = null;
            
            //set random target for boss in close range
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 30;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 30, 1);
            Vector3 finalPosition = hit.position;
            _navMeshAgent.SetDestination(finalPosition);
            
            yield return new WaitForSeconds(_secondsBetweenAttacks);
            
            _waitCoroutine = null;
            
            StartRandomAttack();
        }
        
        private IEnumerator SecondPhaseCoroutine()
        {
            Debug.Log("Second phase started");
            
            //make invulnerable
            _health.invincible = true;
            
            _secondPhase = true;
                
            //stop current attack
            if (_currentAttack != null)
            {
                _currentAttack.StopAttack();
            }
                
            //stop coroutine
            if (_waitCoroutine != null) StopCoroutine(_waitCoroutine);
            
            //destroy objects
            foreach (var obj in _destroyObjects)
            {
                Destroy(obj);
                //play destroy sound
                _audioSource.PlayOneShot(_destroySound.clip, _destroySound.volume);
                
                yield return new WaitForSeconds(0.25f);
            }
            
            //make vulnerable
            _health.invincible = false;
            
            //start waiting coroutine
            _waitCoroutine = StartCoroutine(WaitCoroutine());
            
            yield return null;
        }

        private void StartRandomAttack()
        {
            if (!_secondPhase)
            {
                //get available attacks; cant be last attack
                List<BossAttack> availableAttacks = _firstPhaseAttacks.FindAll(x => x != _lastAttack);
                
                if (availableAttacks.Count == 0)
                {
                    Debug.LogError("No available attacks found");
                    return;
                }
                
                int randomIndex = UnityEngine.Random.Range(0, availableAttacks.Count);
                _currentAttack = availableAttacks[randomIndex];

            }
            else
            {
                //get random attack
                List<BossAttack> availableAttacks = _secondPhaseAttacks.FindAll(x => x != _lastAttack);
                
                if (availableAttacks.Count == 0)
                {
                    Debug.LogError("No available attacks found");
                    return;
                }
                
                int randomIndex = UnityEngine.Random.Range(0, availableAttacks.Count);
                _currentAttack = availableAttacks[randomIndex];
            }
            
            //start attack
            _currentAttack.StartAttack(this);
            
        }
        
        private void HandleOnAttackFinished()
        {
            
            //set last attack
            _lastAttack = _currentAttack;
            
            _currentAttack = null;
            
            Debug.Log("Attack finished");
            
            //start waiting coroutine
            if (_waitCoroutine == null)
            {
                _waitCoroutine = StartCoroutine(WaitCoroutine());
            }
            else
            {
                Debug.LogWarning("Wait coroutine already running");
            }
        }

        private void HandleOnHealthChanged()
        {
            Debug.Log("Boss health changed");
            
            if(_health.CurrentHealth <= _health.MaxHealth / 2 && !_secondPhase)
            {
                //start second phase
                StartCoroutine(SecondPhaseCoroutine());
            }
        }
        
        private void HandleOnDeath(GameObject obj)
        {
            Debug.Log("Boss died");
            
            //stop all coroutines
            if (_waitCoroutine != null) StopCoroutine(_waitCoroutine);
        }

        private void OnEnable()
        {
            _health.OnHealthChanged += HandleOnHealthChanged;
            _health.OnDeath += HandleOnDeath;
            
            //subscribe to attack finished event
            foreach (var attack in _firstPhaseAttacks)
            {
                attack.OnAttackFinished += HandleOnAttackFinished;
            }
            
            foreach (var attack in _secondPhaseAttacks)
            {
                attack.OnAttackFinished += HandleOnAttackFinished;
            }
        }
        
        private void OnDisable()
        {
            _health.OnHealthChanged -= HandleOnHealthChanged;
            _health.OnDeath -= HandleOnDeath;
            
            //unsubscribe from attack finished event
            foreach (var attack in _firstPhaseAttacks)
            {
                attack.OnAttackFinished -= HandleOnAttackFinished;
            }
            
            foreach (var attack in _secondPhaseAttacks)
            {
                attack.OnAttackFinished -= HandleOnAttackFinished;
            }
            
            //stop all coroutines
            if (_waitCoroutine != null) StopCoroutine(_waitCoroutine);
        }
        
        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
    }
}
