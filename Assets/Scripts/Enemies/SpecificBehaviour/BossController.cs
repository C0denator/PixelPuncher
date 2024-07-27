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
        [SerializeField] [Range(0f,3f)] private float _secondsBetweenAttacks;
        [SerializeField] private List<BossAttackListItem> _attacks;
        
        private Transform _player;
        private Health.Health _health;
        private int _lastAttackIndex;
        private NavMeshAgent _navMeshAgent;
        private Coroutine _waitCoroutine;
        
        private float elapsedTimeBetweenAttacks;
        
        public Transform Player => _player;
        public Health.Health Health => _health;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _health = _player.GetComponent<Health.Health>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            
            //error handling
            if (_player == null) Debug.LogError("Player not found in BossController");
            if (_health == null) Debug.LogError("Player health not found in BossController");
            if(_attacks.Count == 0) Debug.LogError("No attacks set in BossController");
            if (_navMeshAgent == null) Debug.LogError("NavMeshAgent not found in BossController");
            
            //set up agent for 2D
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
        }

        private void Start()
        {
            //start waiting coroutine
            _waitCoroutine = StartCoroutine(WaitCoroutine());
            
            //set last attack index to -1
            _lastAttackIndex = -1;
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
            
            StartRandomAttack();
            
            _waitCoroutine = null;
        }

        private void StartRandomAttack()
        {
            int randomIndex;
            BossAttack randomAttack;
            
            if (_attacks.Count > 1)
            {
                //get list of available attacks
                List<BossAttackListItem> availableAttacks = new List<BossAttackListItem>();

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
            
            //start attack
            randomAttack.StartAttack(this);
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
        public struct BossAttackListItem
        {
            public BossAttack Attack;
            public bool IsAvailable;
        }
    }
}
