using System.Collections;
using System.Collections.Generic;
using GameProg.Enemies.SpecificBehaviour.BossAttacks;
using UnityEngine;
using UnityEngine.AI;

namespace GameProg.Enemies.SpecificBehaviour
{
    public class BossController : MonoBehaviour
    {
        [SerializeField] [Range(0f,3f)] private float _secondsBetweenAttacks;
        [SerializeField] private List<BossAttack> _attacks;
        
        private Transform _player;
        private Health.Health _health;
        private BossAttack _lastAttack;
        private NavMeshAgent _navMeshAgent;
        private Coroutine _waitCoroutine;
        
        private float elapsedTimeBetweenAttacks;

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

        private void HandleOnAttackFinished()
        {
            
        }
        
        private IEnumerator WaitCoroutine()
        {
            yield return new WaitForSeconds(_secondsBetweenAttacks);
            //Attack();
            
            _waitCoroutine = null;
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
                attack.OnAttackFinished += HandleOnAttackFinished;
            }
        }
        
        private void OnDisable()
        {
            _health.OnHealthChanged -= HandleOnHealthChanged;
            _health.OnDeath -= HandleOnDeath;
        }
    }
}
