using System;
using System.Collections;
using JetBrains.Annotations;
using Sound;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GameProg.Enemies.SpecificBehaviour
{   
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EnemyBaseClass))]
    public class Fighter1Controller : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private AudioClipWithVolume attackSound;
        [Header("Attack")]
        [SerializeField] private int damage;
        [SerializeField] [Range(0f,5f)] private float attackCooldown;
        [SerializeField] [Range(2f,8f)] private float attackRange;
        [SerializeField] [Range(5f,20f)] private float bulletSpeed;
        
        private Rigidbody2D _rb;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private AudioSource _audioSource;
        private GameMaster _gameMaster;
        
        
        [CanBeNull] private Coroutine _attackCoroutine;
        private bool _attackInProgress = false;
        private float _cooldownTimer;
        
        // Start is called before the first frame update
        private void Start()
        {
            //get references
            _rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindWithTag("Player");
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _audioSource = FindObjectOfType<GlobalSound>().globalAudioSource;
            _gameMaster = FindObjectOfType<GameMaster>();
            
            //error handling
            //if (_rb == null) Debug.LogError("Rigidbody2D component not found");
            if (_navMeshAgent == null) Debug.LogError("NavMeshAgent component not found");
            if (player == null) Debug.LogError("Player not found");
            if (_animator == null) Debug.LogError("Animator component not found");
            if (bulletPrefab == null) Debug.LogError("Bullet prefab not set");
            if (_audioSource == null) Debug.LogError("Audio source not found");
            if (_gameMaster == null) Debug.LogError("GameMaster not found");
            
            //set up agent for 2D
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            
            //set stuff
            _cooldownTimer = Random.Range(1, attackCooldown);
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

            int amountOfBullets = Random.Range(1, 4);

            for (int i = 0; i < amountOfBullets; i++)
            {
                SpawnBullet();
                yield return new WaitForSeconds(0.2f);
            }
            
            yield return new WaitForSeconds(0.3f);
            
            _attackInProgress = false;
            
            _animator.SetTrigger("AttackEnd");
            
            _attackCoroutine = null;
        }
        
        private void SpawnBullet()
        {
            //play attack sound
            _audioSource.PlayOneShot(attackSound.clip, attackSound.volume);
            
            //spawn bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            
            Vector3 launchDirection = Vector3.zero;

            if (_gameMaster.GigachadMode)
            {
                //calculate direction to future player position
                launchDirection = CalculateLaunchDirection(transform.position, bulletSpeed, player.transform.position, player.GetComponent<Rigidbody2D>().velocity);
            }
            else
            {
                //calculate wrong direction to future player position
                launchDirection = CalculateLaunchDirection(transform.position, bulletSpeed*3f, player.transform.position, player.GetComponent<Rigidbody2D>().velocity);
            }
            
            //rotate bullet
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(launchDirection.x, launchDirection.y, 0));
            bullet.transform.Rotate(Vector3.forward, 90);
            
            //set velocity
            bullet.GetComponent<Rigidbody2D>().velocity = launchDirection * bulletSpeed;
        }

        private Vector2 CalculateLaunchDirection(Vector2 projectilePosition, float projectileSpeed,
            Vector2 targetPosition, Vector2 targetVelocity)
        {
            Debug.Log("Calculating launch direction");
            Debug.Log("Projectile position: " + projectilePosition);
            Debug.Log("Projectile speed: " + projectileSpeed);
            Debug.Log("Target position: " + targetPosition);
            Debug.Log("Target velocity: " + targetVelocity);
            
            //calculate time to hit target
            Vector2 targetRelativePosition = targetPosition - projectilePosition;
            Vector2 targetRelativeVelocity = targetVelocity;
            
            //calculate coefficients for quadratic equation a*t^2 + b*t + c = 0
            float a = Vector2.Dot(targetRelativeVelocity, targetRelativeVelocity) - projectileSpeed * projectileSpeed;
            float b = 2 * Vector2.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = Vector2.Dot(targetRelativePosition, targetRelativePosition);
            
            //calculate discriminant
            float discriminant = b * b - 4 * a * c;
            
            //if discriminant is negative, target cannot be hit
            if (discriminant < 0)
            {
                Debug.LogWarning("Target cannot be hit");
                return Vector2.zero;
            }
            
            //calculate square root of discriminant
            float sqrtDiscriminant = Mathf.Sqrt(discriminant);
            
            //calculate two possible solutions for t
            float t1 = (-b + sqrtDiscriminant) / (2 * a);
            float t2 = (-b - sqrtDiscriminant) / (2 * a);
            
            //get bigger solution
            float t = t1 > t2 ? t1 : t2;
            
            //if t<0, get smaller solution
            if (t < 0) t = t1 < t2 ? t1 : t2;
            
            //check if t is positive
            if (t > 0)
            {
                //calculate future target position
                Vector2 futureTargetPosition = targetPosition + targetVelocity * t;
                
                //calculate direction to future target position and normalize it
                Vector2 launchDirection = (futureTargetPosition - projectilePosition).normalized;
                
                Debug.Log("Solution found: " + launchDirection);
                Vector2 normaleDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y).normalized;
                Debug.Log("Normal direction: " + normaleDirection);
                
                return launchDirection;
            }
            
            Debug.LogWarning("Target cannot be hit");
            return Vector2.zero;
            
            
        }

        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
    }
}
