using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace GameProg.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxHealth;
        [Header("References")]
        [SerializeField] private GameObject player;
        [Header("Attack")]
        [SerializeField] private int damage;
        [SerializeField] [Range(0f,5f)] private float attackCooldown;
        [SerializeField] [Range(2f,8f)] private float attackRange;
        
        private Rigidbody2D _rb;
        private NavMeshAgent _navMeshAgent;
        
        [CanBeNull] private Coroutine _attackCoroutine;
        private bool _attackInProgress = false;
        private float _cooldownTimer = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            //get references
            _rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindWithTag("Player");
            _navMeshAgent = GetComponent<NavMeshAgent>();
            
            //set up agent for 2D
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            
            //set health
            currentHealth = maxHealth;
            
            //error handling
            //if (_rb == null) Debug.LogError("Rigidbody2D component not found");
            if (_navMeshAgent == null) Debug.LogError("NavMeshAgent component not found");
            if (player == null) Debug.LogError("Player not found");
        }

        private void FixedUpdate()
        {
            if (_cooldownTimer > 0) _cooldownTimer -= Time.fixedDeltaTime;
            
            if (_attackInProgress) return;
            
            //is player in attack range?
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if(distanceToPlayer <= attackRange)
            {
                //is cooldown over?
                if (_cooldownTimer <= 0)
                {
                    Debug.Log("Starting attack");
                    
                    //reset target
                    _navMeshAgent.ResetPath();
                    
                    //start attack
                    return;
                }
                
                Debug.Log("Moving towards player");
                
                //move towards player
                _navMeshAgent.SetDestination(player.transform.position);
            }
            else
            {
                Debug.Log("Moving towards player");
                
                //move towards player
                _navMeshAgent.SetDestination(player.transform.position);
            }
        }

        private void OnDrawGizmos()
        {
            //draw Attackrange as yellow circle
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        
        private IEnumerator AttackCoroutine()
        {
            _attackInProgress = true;
            
            yield return new WaitForSeconds(attackCooldown);
            Debug.Log("Attack performed!");
            
            _attackInProgress = false;
            
            _attackCoroutine = null;
        }
    }
}
