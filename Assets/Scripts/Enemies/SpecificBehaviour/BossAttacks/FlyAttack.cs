using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Enemies.SpecificBehaviour.BossAttacks
{
    public class FlyAttack : BossAttack
    {
        [SerializeField] [Range(0.01f,1f)] private float amplitude;
        [SerializeField] [Range(0.1f,15f)] private float frequency;
        [SerializeField] [Range(0.1f,15f)] private float horizontalSpeed;
        [SerializeField] [Range(3f,20f)] private float attackDuration;
        [SerializeField] [Range(0.1f, 1f)] private float secondsBetweenShots;
        [SerializeField] [Range(0.1f, 5f)] private float secondsBetweenRockets;
        [SerializeField] [Range(1f, 3f)] private float bulletSpeed;
        [FormerlySerializedAs("_bulletSpawnPoint")] [SerializeField] private GameObject minigun;
        [SerializeField] private GameObject mortar;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject rocketPrefab;
        [SerializeField] private AudioClipWithVolume _attackSound;
        [SerializeField] private GameObject _debugSphere;
        
        private float _elapsedTime;
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = FindObjectOfType<Sound.GlobalSound>().globalAudioSource;
            
            if (_audioSource == null) Debug.LogError("Global audio source not found in FlyAttack");
            if (bulletPrefab == null) Debug.LogError("Bullet prefab not found in FlyAttack");
            if (minigun == null) Debug.LogError("Bullet spawn point not found in FlyAttack");
        }

        public override void StartAttack(BossController ctx)
        {
            if (ctx.GameMaster.GigachadMode)
            {
                secondsBetweenShots *= 0.75f;
                secondsBetweenRockets *= 0.75f;
            }
            
            if(_attackCoroutine == null)
            {
                _elapsedTime = 0;
                _attackCoroutine = StartCoroutine(FlyAttackCoroutine(ctx));
            }
            else
            {
                Debug.LogWarning("Attack already in progress");
            }
        }
        
        private IEnumerator FlyAttackCoroutine(BossController ctx)
        {
            Debug.Log("Fly attack started");

            ctx.NavMeshAgent.ResetPath();
            ctx.NavMeshAgent.stoppingDistance = 0;
            
            //reset minigun rotation
            minigun.transform.rotation = Quaternion.Euler(0, 0, -90);
            
            bool reachedTop = false;
            
            //set point above boss
            Vector3 target = new Vector3(ctx.transform.position.x, ctx.transform.position.y, 0);

            RaycastHit2D hit;
            int i = 0;
            int maxIterations = 100;

            do
            {
                i++;
                
                if (i > maxIterations)
                {
                    Debug.LogError("Max iterations reached");
                    break;
                }
                
                target += Vector3.up;
                hit = Physics2D.Raycast(target, Vector3.up, 2f, LayerMask.GetMask("Wall"));
            } while (hit.collider == null);
            
            Debug.Log("Target found with " + i + " iterations");
            Debug.Log("Target: " + target);
            Debug.Log("Boss position: " + ctx.transform.position);
            
            //spawn debug sphere
            //Instantiate(_debugSphere, target, Quaternion.identity);
            
            ctx.NavMeshAgent.SetDestination(target);
            ctx.NavMeshAgent.speed = 10f;
            
            //wait for agent find path
            while(ctx.NavMeshAgent.pathPending)
            {
                Debug.Log("Path pending");
                yield return null;
            }
            
            Debug.Log("Path found");
            Debug.Log("Remaining distance: " + ctx.NavMeshAgent.remainingDistance);
            
            Vector3 vel = Vector3.zero;
            Vector3 sinVector = Vector3.zero;

            while (ctx.NavMeshAgent.remainingDistance > 0.5f)
            {
                Debug.Log("Remaining distance: " + ctx.NavMeshAgent.remainingDistance);
                yield return null;
            }
            
            Debug.Log("Reached destination");
            ctx.NavMeshAgent.ResetPath();
            vel = new Vector3(horizontalSpeed, 0, 0);
            
            //start mini gun animation
            ctx.MiniGunAnimator.SetTrigger("Fire");

            float secondsTillLastShot = secondsBetweenShots;
            float secondsTillLastRocket = secondsBetweenRockets;
            
            while (_elapsedTime < attackDuration)
            {
                _elapsedTime += Time.deltaTime;
                
                float ySin = Mathf.Sin(Time.time * frequency) * amplitude;
                sinVector = new Vector3(0, ySin, 0);

                if (_elapsedTime > 1f)
                {
                    //shoot bullet

                    if (secondsTillLastShot >= secondsBetweenShots)
                    {
                        GameObject bullet = Instantiate(bulletPrefab, minigun.transform.position, Quaternion.identity);

                        //rotate downwards
                        bullet.transform.rotation = Quaternion.Euler(0, 0, -90);
                        
                        

                        if (ctx.GameMaster.GigachadMode)
                        {
                            //set velocity
                            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(ySin*50f, -2f) * bulletSpeed;
                        }
                        else
                        {
                            //set velocity
                            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(ySin*10f, -2f) * bulletSpeed;
                        }
                        
                        //play sound
                        _audioSource.PlayOneShot(_attackSound.clip, _attackSound.volume);
                        
                        secondsTillLastShot = 0;
                    }
                    else
                    {
                        secondsTillLastShot += Time.deltaTime;
                    
                    }
                    
                    //shoot rocket
                    if (secondsTillLastRocket >= secondsBetweenRockets)
                    {
                        GameObject rocket = Instantiate(rocketPrefab, mortar.transform.position, Quaternion.identity);

                        //rotate downwards
                        rocket.transform.rotation = Quaternion.Euler(0, 0, -90);
                        
                        //set velocity
                        rocket.GetComponent<Rigidbody2D>().velocity = new Vector2(ySin*10f, -2f);
                        rocket.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-90f, 90f);
                        
                        //play sound
                        _audioSource.PlayOneShot(_attackSound.clip, _attackSound.volume);
                        
                        //play mortar animation
                        ctx.MortarAnimator.SetTrigger("Fire");
                        
                        secondsTillLastRocket = 0;
                    }
                    else
                    {
                        secondsTillLastRocket += Time.deltaTime;
                    }
                }

                Debug.Log("Sin vector: " + sinVector * Time.deltaTime);
                    
                ctx.NavMeshAgent.Move((vel * Time.deltaTime)+sinVector);
        
                // Check for collisions with the walls using RaycastHit2D
                Vector3 origin = new Vector3(ctx.transform.position.x, ctx.transform.position.y, 0);
                hit = Physics2D.Raycast(origin, vel, 2, LayerMask.GetMask("Wall"));
                if (hit.collider != null)
                {
                    // Reflect velocity 
                    vel = new Vector3(-vel.x, vel.y, 0);
                }

                yield return null;
            }
            
            //stop minigun animation
            ctx.MiniGunAnimator.SetTrigger("Stop");
            
            OnAttackFinished?.Invoke();
            
            _attackCoroutine = null;
            
            yield return null;
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
