using System;
using System.Collections;
using UnityEngine;

namespace GameProg.Enemies.SpecificBehaviour.BossAttacks
{
    public class FlyAttack : BossAttack
    {
        [SerializeField] [Range(0.1f,40f)] private float amplitude;
        [SerializeField] [Range(0.1f,15f)] private float frequency;
        [SerializeField] [Range(0.1f,15f)] private float horizontalSpeed;
        [SerializeField] [Range(3f,10f)] private float attackDuration;
        [SerializeField] private GameObject _debugSphere;
        
        private float _elapsedTime;
        
        private void Awake()
        {
        }

        public override void StartAttack(BossController ctx)
        {
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
                hit = Physics2D.Raycast(target, Vector3.up, 5f, LayerMask.GetMask("Wall"));
            } while (hit.collider == null);
            
            Debug.Log("Target found with " + i + " iterations");
            Debug.Log("Target: " + target);
            Debug.Log("Boss position: " + ctx.transform.position);
            
            //spawn debug sphere
            Instantiate(_debugSphere, target, Quaternion.identity);
            
            ctx.NavMeshAgent.SetDestination(target);
            
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
            
            //TODO: Implement attack
            while (_elapsedTime < attackDuration)
            {
                float ySin = Mathf.Sin(Time.time * frequency) * amplitude;
                sinVector = new Vector3(0, ySin, 0);
                
                //has the agent reached its destination?
                if (!reachedTop && ctx.NavMeshAgent.remainingDistance < 0.5f)
                {
                    Debug.Log("Reached destination");
                    reachedTop = true;
                    ctx.NavMeshAgent.ResetPath();
                    vel = new Vector3(horizontalSpeed, 0, 0);
                }

                if (!reachedTop)
                {
                    yield return null;
                }
                else
                {
                    _elapsedTime += Time.deltaTime;

                    Debug.Log("Sin vector: " + sinVector * Time.deltaTime);
                    
                    ctx.NavMeshAgent.Move((vel * Time.deltaTime)+sinVector);
        
                    // Check for collisions with the walls using RaycastHit2D
                    Vector3 origin = new Vector3(ctx.transform.position.x, ctx.transform.position.y, 0);
                    hit = Physics2D.Raycast(origin, vel, 20, LayerMask.GetMask("Wall"));
                    if (hit.collider != null)
                    {
                        // Reflect velocity 
                        //vel *= -1;
                    }

                    yield return null;

                }
                
                yield return null;
            }
            
            OnAttackFinished?.Invoke();
            
            _attackCoroutine = null;
            
            yield return null;
        }
        
        private new void OnDisable()
        {
            base.OnDisable();
        }
    }
}
