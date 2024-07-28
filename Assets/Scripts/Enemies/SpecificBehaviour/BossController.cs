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
        [SerializeField] private List<BossAttackListItem> _attacks;
        [SerializeField] private AudioClipWithVolume _destroySound;
        
        private Transform _player;
        private Health.Health _health;
        private int _lastAttackIndex;
        [SerializeField] private BossAttack _currentAttack;
        private NavMeshAgent _navMeshAgent;
        private Coroutine _waitCoroutine;
        private AudioSource _audioSource;
        private CircleCollider2D _collider;
        
        private float elapsedTimeBetweenAttacks;
        private bool _secondPhase;
        
        public Transform Player => _player;
        public Health.Health Health => _health;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        public CircleCollider2D Collider => _collider;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _health = _player.GetComponent<Health.Health>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _audioSource = FindObjectOfType<Sound.GlobalSound>().globalAudioSource;
            _collider = GetComponent<CircleCollider2D>();
            
            //error handling
            if (_player == null) Debug.LogError("Player not found in BossController");
            if (_health == null) Debug.LogError("Player health not found in BossController");
            if(_attacks.Count == 0) Debug.LogError("No attacks set in BossController");
            if (_navMeshAgent == null) Debug.LogError("NavMeshAgent not found in BossController");
            if (_audioSource == null) Debug.LogError("Global audio source not found in BossController");
            if (_collider == null) Debug.LogError("Collider not found in BossController");
            
            //set up agent for 2D
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
        }

        private void Start()
        {
            _secondPhase = false;
            
            //start waiting coroutine
            _waitCoroutine = StartCoroutine(WaitCoroutine());
            
            //set last attack index to -1
            _lastAttackIndex = -1;
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
                float rotationSpeed = _navMeshAgent.velocity.magnitude * 10;
                
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
            //set random target for boss in close range
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 10;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10, 1);
            Vector3 finalPosition = hit.position;
            _navMeshAgent.SetDestination(finalPosition);
            
            yield return new WaitForSeconds(_secondsBetweenAttacks);
            
            _waitCoroutine = null;
            
            StartRandomAttack();
        }

        private void StartRandomAttack()
        {
            int randomIndex;
            BossAttack randomAttack;
            
            if (_attacks.Count > 1)
            {
                //get list of available attacks
                List<BossAttackListItem> availableAttacks = new List<BossAttackListItem>();
                
                if(availableAttacks.Count == 0)
                {
                    Debug.LogWarning("No attacks available");
                    return;
                }

                foreach (var attack in _attacks)
                {
                    if (attack.IsAvailable)
                    {
                        availableAttacks.Add(attack);
                    }
                }

                //pick random attack
                randomIndex = UnityEngine.Random.Range(0, availableAttacks.Count);
                randomAttack = availableAttacks[randomIndex].Attack;
                
                //find index of attack in _attacks list
                for (int i = 0; i < _attacks.Count; i++)
                {
                    if (_attacks[i].Attack == randomAttack)
                    {
                        randomIndex = i;
                        break;
                    }
                }
                
            }
            else if(_attacks.Count == 1)
            {
                randomIndex = 0;
                randomAttack = _attacks[0].Attack;
            }
            else
            {
                Debug.LogWarning("No attacks available");
                return;
            }

            //set last attack as available again
            
            if (_lastAttackIndex != -1)
            {
                var lastAttack = _attacks[_lastAttackIndex];
                lastAttack.IsAvailable = true;
                _attacks[_lastAttackIndex] = lastAttack;
            }
            
            //set new attack as unavailable
            var newAttack = _attacks[randomIndex];
            newAttack.IsAvailable = false;
            _attacks[randomIndex] = newAttack;
            
            //set new attack as last attack
            _lastAttackIndex = randomIndex;
            
            //set current attack
            _currentAttack = randomAttack;
            
            //start attack
            _currentAttack.StartAttack(this);
        }
        
        private void HandleOnAttackFinished()
        {
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
                _secondPhase = true;
                
                //disable flying attack
                for(int i = 0; i < _attacks.Count; i++)
                {
                    if (_attacks[i].Attack is FlyAttack)
                    {
                        var attack = _attacks[i];
                        attack.IsAvailable = false;
                        _attacks[i] = attack;
                        
                        break;
                    }
                }
                
                //enable roll attack
                for(int i = 0; i < _attacks.Count; i++)
                {
                    if (_attacks[i].Attack is RollAttack)
                    {
                        var attack = _attacks[i];
                        attack.IsAvailable = true;
                        _attacks[i] = attack;
                        
                        break;
                    }
                }

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
            foreach (var attack in _attacks)
            {
                attack.Attack.OnAttackFinished += HandleOnAttackFinished;
            }
        }
        
        private void OnDisable()
        {
            _health.OnHealthChanged -= HandleOnHealthChanged;
            _health.OnDeath -= HandleOnDeath;
            
            //unsubscribe from attack finished event
            foreach (var attack in _attacks)
            {
                attack.Attack.OnAttackFinished -= HandleOnAttackFinished;
            }
            
            //stop all coroutines
            if (_waitCoroutine != null) StopCoroutine(_waitCoroutine);
        }
        
        [Serializable]
        private struct BossAttackListItem
        {
            public BossAttack Attack;
            public bool IsAvailable;
        }
        
        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
    }
}
