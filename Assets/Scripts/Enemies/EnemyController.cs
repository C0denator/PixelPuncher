using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace GameProg.Enemies
{   
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
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
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        
        [CanBeNull] private Coroutine _attackCoroutine;
        private bool _attackInProgress = false;
        private float _cooldownTimer;
        
        // Start is called before the first frame update
        void Start()
        {
            //get references
            _rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindWithTag("Player");
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            
            //set up agent for 2D
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            
            //set stuff
            currentHealth = maxHealth;
            _cooldownTimer = attackCooldown;
            
            //error handling
            //if (_rb == null) Debug.LogError("Rigidbody2D component not found");
            if (_navMeshAgent == null) Debug.LogError("NavMeshAgent component not found");
            if (player == null) Debug.LogError("Player not found");
            if (_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found");
            if (_animator == null) Debug.LogError("Animator component not found");
        }

        private void FixedUpdate()
        {
            //flip sprite if player is on the left side
            if (player.transform.position.x < transform.position.x)
            {
                _spriteRenderer.flipX = true;
            }
            else
            {
                _spriteRenderer.flipX = false;
            }
            
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
                    _animator.SetTrigger("AttackStart");
                    
                    //set cooldown
                    _cooldownTimer = attackCooldown;
                    
                    //start attack coroutine
                    _attackCoroutine = StartCoroutine(AttackCoroutine());
                    
                    return;
                }
                
                //move towards player
                _navMeshAgent.SetDestination(player.transform.position);
            }
            else
            {
                
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
            
            yield return new WaitForSeconds(1f);
            Debug.Log("Attack performed!");
            
            _attackInProgress = false;
            
            _animator.SetTrigger("AttackEnd");
            
            _attackCoroutine = null;
        }
    }
}
