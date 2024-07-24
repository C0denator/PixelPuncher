using System.Collections;
using System.Collections.Generic;
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
        [Header("References")]
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private List<GameObject> debrisObjects;
        [Header("Attack")]
        [SerializeField] private int damage;
        [SerializeField] [Range(0f,5f)] private float attackCooldown;
        [SerializeField] [Range(2f,8f)] private float attackRange;
        [SerializeField] [Range(3f,10f)] private float bulletSpeed;
        
        private Health.Health _health;
        private Rigidbody2D _rb;
        private NavMeshAgent _navMeshAgent;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Transform _debrisParent;
        
        
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
            _health = GetComponent<Health.Health>();
            
            //find debris parent: look at parent and search for a gameobject with the name "Debris"
            foreach (Transform child in transform.parent)
            {
                if (child.name == "Debris")
                {
                    _debrisParent = child;
                    break;
                }
            }
            
            //error handling
            //if (_rb == null) Debug.LogError("Rigidbody2D component not found");
            if (_navMeshAgent == null) Debug.LogError("NavMeshAgent component not found");
            if (player == null) Debug.LogError("Player not found");
            if (_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found");
            if (_animator == null) Debug.LogError("Animator component not found");
            if (bulletPrefab == null) Debug.LogError("Bullet prefab not set");
            if (_debrisParent == null) Debug.LogError("Debris parent not set");
            if (_health == null) Debug.LogError("Health component not found");
            
            //set up agent for 2D
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            
            //set stuff
            _cooldownTimer = attackCooldown;
            
            //subscribe to death event
            _health.OnDeath += HandleOnDeath;
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
            
            yield return new WaitForSeconds(0.2f);
            
            SpawnBullet();
            

            yield return new WaitForSeconds(0.3f);
            
            //shoot another bullet
            
            SpawnBullet();
            
            
            yield return new WaitForSeconds(0.3f);
            
            _attackInProgress = false;
            
            _animator.SetTrigger("AttackEnd");
            
            _attackCoroutine = null;
        }
        
        private void SpawnBullet()
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Vector2 direction = (player.transform.position - transform.position).normalized;
            bullet.transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
            
            bullet.GetComponent<EnemyBullet>().Damage = damage;
        }
        
        private void HandleOnDeath()
        {
            //spawn debris
            foreach (GameObject debrisObject in debrisObjects)
            {
                debrisObject.SetActive(true);
                
                //set parent
                debrisObject.transform.SetParent(_debrisParent);
                
                //get rigidbody
                Rigidbody2D debrisRb = debrisObject.GetComponent<Rigidbody2D>();

                if (debrisRb == null)
                {
                    Debug.LogError("Rigidbody2D component not found on debris object");
                }
                else
                {
                    //apply slight random force
                    debrisRb.AddForce(new Vector2(Random.Range(-2f,2f), Random.Range(-2f,2f)), ForceMode2D.Impulse);
                }
                
                //get sprite renderer
                SpriteRenderer debrisSpriteRenderer = debrisObject.GetComponent<SpriteRenderer>();
                
                debrisSpriteRenderer.flipX = _spriteRenderer.flipX;
                
                
            }
            
            //destroy enemy
            Destroy(gameObject);
        }
    }
}
