using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GameProg.Enemies.SpecificBehaviour.BossAttacks
{
    public class RollAttack : BossAttack
    {
        [SerializeField] [Range(5,20f)] private float rollSpeed;
        [SerializeField] [Range(3f,10f)] private float attackDuration;
        [SerializeField] Vector3 vel;
        [SerializeField] private AudioClipWithVolume _attackSound;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] [Range(5,10f)] private float _bulletSpeed;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = FindObjectOfType<Sound.GlobalSound>().globalAudioSource;
            
            //error handling
            if (_audioSource == null) Debug.LogError("Global audio source not found in RollAttack");
            if (_bulletPrefab == null) Debug.LogError("Bullet prefab not found in RollAttack");
        }

        public override void StartAttack(BossController ctx)
        {
            if(_attackCoroutine == null)
            {
                _attackCoroutine = StartCoroutine(RollAttackCoroutine(ctx));
            }
            else
            {
                Debug.LogWarning("Attack already in progress");
            }
        }
        
        private IEnumerator RollAttackCoroutine(BossController ctx)
        {
            float elapsedTime = 0;
            Debug.Log("Roll attack started");

            ctx.NavMeshAgent.ResetPath();

            // Set first direction to player
            Vector3 playerPos = ctx.Player.position;
            vel = (playerPos - ctx.transform.position).normalized * rollSpeed;
            vel = new Vector3(vel.x, vel.y, 0);

            while (elapsedTime < attackDuration)
            {
                elapsedTime += Time.deltaTime;
        
                // Move the boss
                ctx.NavMeshAgent.Move(vel * Time.deltaTime);
        
                // Check for collisions with the walls using RaycastHit2D
                Vector3 origin = new Vector3(ctx.transform.position.x, ctx.transform.position.y, 0);
                RaycastHit2D hit = Physics2D.Raycast(origin, vel, 10*rollSpeed * Time.deltaTime, LayerMask.GetMask("Wall"));
                if (hit.collider != null)
                {
                    // Reflect velocity based on the hit normal in 2D
                    vel = Vector2.Reflect(vel, hit.normal);
                    
                    //play sound
                    _audioSource.PlayOneShot(_attackSound.clip, _attackSound.volume);
                    
                    //spawn bullet ring
                    SpawnBulletRing();
                }
                
                //rotate the core depending on the velocity
                float rotationSpeed = vel.magnitude;
                ctx.Core.transform.Rotate(Vector3.forward, rotationSpeed);
        
                yield return null;
            }

            ctx.NavMeshAgent.ResetPath();
            ctx.NavMeshAgent.velocity = Vector3.zero;
            OnAttackFinished?.Invoke();
            _attackCoroutine = null;
            
            yield return null;
        }

        private void SpawnBulletRing()
        {
            int BulletAmount = 36;
            
            //spawn a ring around the boss  
            for (int i = 0; i < BulletAmount; i++)
            {
                float angle = i * Mathf.PI * 2 / BulletAmount;
                Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 0.5f + transform.position;
                GameObject bullet = Instantiate(_bulletPrefab, pos, Quaternion.identity);
                
                //set velocity
                bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _bulletSpeed;
                
                Debug.Log("Bullet spawned. Velocity: " + bullet.GetComponent<Rigidbody2D>().velocity);
            }
        }
        
        private new void OnDisable()
        {
            base.OnDisable();
        }
        
        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
    }
}
