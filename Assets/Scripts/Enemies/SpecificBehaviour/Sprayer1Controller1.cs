using System;
using System.Collections;
using JetBrains.Annotations;
using Sound;
using UnityEngine;
using UnityEngine.AI;

namespace GameProg.Enemies.SpecificBehaviour
{   
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EnemyBaseClass))]
    public class Sprayer1Controller : MonoBehaviour
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
        [SerializeField] [Range(15f, 90f)] private float sprayAngle;
        [SerializeField] [Range(2f, 10f)] private int bulletsPerShot;
        
        private Rigidbody2D _rb;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private AudioSource _audioSource;
        
        
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
            _animator = GetComponent<Animator>();
            _audioSource = FindObjectOfType<GlobalSound>().globalAudioSource;
            
            //error handling
            //if (_rb == null) Debug.LogError("Rigidbody2D component not found");
            if (_navMeshAgent == null) Debug.LogError("NavMeshAgent component not found");
            if (player == null) Debug.LogError("Player not found");
            if (_animator == null) Debug.LogError("Animator component not found");
            if (bulletPrefab == null) Debug.LogError("Bullet prefab not set");
            if (_audioSource == null) Debug.LogError("Audio source not found");
            
            //set up agent for 2D
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            
            //set stuff
            _cooldownTimer = attackCooldown;
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
            //start attack animation
            _animator.SetTrigger("AttackStart");
            
            _attackInProgress = true;
            
            //set player target
            Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, 0);
            
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < bulletsPerShot; i++)
            {
                //calculate angle difference; difference is 0 for the middle bullet and max for first and last

                float angleDiff = 0;
                
                if (i < (bulletsPerShot-1f) / 2f)
                {
                    //diverge to the left
                    float t = i / ((bulletsPerShot-1f) / 2f);
                    //Debug.Log("i = " + i + " t = " + t);
                    angleDiff = Mathf.Lerp(sprayAngle, 0, t);
                    
                }else if (i > (bulletsPerShot-1f) / 2f)
                {
                    //diverge to the right
                    float t = i / ((bulletsPerShot-1f) / 2f);
                    t -= 1;
                    //Debug.Log("i = " + i + " t = " + t);
                    angleDiff = Mathf.Lerp(0, -sprayAngle, t);
                    
                }
                
                //Debug.Log("i = " + i + " angleDiff = " + angleDiff);
                
                SpawnBullet(angleDiff, target);
                
                //yield return new WaitForSeconds(0.1f);
                
            }
            
            _attackInProgress = false;
            
            //end attack animation
            _animator.SetTrigger("AttackEnd");
            
            _attackCoroutine = null;
        }
        
        private void SpawnBullet(float angleDiff, Vector3 target)
        {
            //play attack sound
            _audioSource.PlayOneShot(attackSound.clip, attackSound.volume);
            
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Vector2 direction = (target - transform.position).normalized;
            bullet.transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            
            //apply the angle Difference
            bullet.transform.Rotate(0,0,angleDiff);
            
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed;
            
            bullet.GetComponent<EnemyBullet>().Damage = damage;
        }

        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
    }
}
