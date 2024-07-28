using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameProg.Enemies.SpecificBehaviour.BossAttacks
{
    public class MinigunAttack : BossAttack
    {
        [SerializeField] [Range(3f,20f)] private float attackDuration;
        [SerializeField] [Range(2f,10f)] private float moveSpeed;
        [SerializeField] private AudioClipWithVolume _attackSound;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] [Range(5,20f)] private float _bulletSpeed;
        [SerializeField] [Range(0.1f, 1f)] private float secondsBetweenShots;
        [SerializeField] [Range(0f, 45f)] private float maxAngleDeviation;
        [FormerlySerializedAs("_bulletSpawnPoint")] [SerializeField] private GameObject minigun;
        
        
        private float _elapsedTime;
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = FindObjectOfType<Sound.GlobalSound>().globalAudioSource;
            
            //error handling
            if (_audioSource == null) Debug.LogError("Global audio source not found in MinigunAttack");
            if (_bulletPrefab == null) Debug.LogError("Bullet prefab not found in MinigunAttack");
        }

        public override void StartAttack(BossController ctx)
        {
            if (ctx.GameMaster.GigachadMode)
            {
                secondsBetweenShots *= 0.75f;
            }
            
            if(_attackCoroutine == null)
            {
                _elapsedTime = 0;
                _attackCoroutine = StartCoroutine(MinigunAttackCoroutine(ctx));
            }
            else
            {
                Debug.LogWarning("Attack already in progress");
            }
        }
        
        private IEnumerator MinigunAttackCoroutine(BossController ctx)
        {
            Debug.Log("Minigun attack started");
            
            ctx.NavMeshAgent.stoppingDistance = 2;
            ctx.NavMeshAgent.speed = moveSpeed;
            
            //start minigun animation
            ctx.MiniGunAnimator.SetTrigger("Fire");
            
            float secondsTillLastShot = secondsBetweenShots;

            while (_elapsedTime < attackDuration)
            {
                _elapsedTime += Time.deltaTime;

                //move towards player
                ctx.NavMeshAgent.SetDestination(ctx.Player.position);
                
                //rotate mini gun towards player
                Vector2 lookDir = ctx.Player.position - minigun.transform.position;
                float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
                minigun.transform.rotation = Quaternion.Euler(0, 0, angle);
                
                
                if (_elapsedTime > 1f)
                {
                    secondsTillLastShot += Time.deltaTime;
                    
                    if(secondsTillLastShot >= secondsBetweenShots)
                    {
                        //shoot
                        GameObject bullet = Instantiate(_bulletPrefab, minigun.transform.position, Quaternion.identity);
                        
                        //look at player
                        lookDir = ctx.Player.position - minigun.transform.position;
                        angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
                        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

                        if (ctx.GameMaster.GigachadMode)
                        {
                            //rotate randomly
                            bullet.transform.Rotate(Vector3.forward, Random.Range(-maxAngleDeviation*2, maxAngleDeviation*2));
                            
                            //set velocity
                            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * _bulletSpeed;
                        }
                        else
                        {
                            //rotate randomly
                            bullet.transform.Rotate(Vector3.forward, Random.Range(-maxAngleDeviation, maxAngleDeviation));
                        
                            //set velocity
                            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * _bulletSpeed;
                        }
                        
                        //play sound
                        _audioSource.PlayOneShot(_attackSound.clip, _attackSound.volume);
                        
                        secondsTillLastShot = 0;
                    }
                }

                yield return null;
            }

            //stop minigun animation
            ctx.MiniGunAnimator.SetTrigger("Stop");
            
            //reset path
            ctx.NavMeshAgent.ResetPath();
            
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
