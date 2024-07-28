using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GameProg.Enemies.SpecificBehaviour.BossAttacks
{
    public class RollAttack : BossAttack
    {
        [SerializeField] [Range(5,20f)] private float rollSpeed;
        [SerializeField] [Range(3f,10f)] private float attackDuration;
        [SerializeField] Vector3 vel;
        
        private void Awake()
        {
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

            // Set random diagonal direction (northwest, northeast, southwest, southeast)
            int direction = Random.Range(0, 4);
            
            //set velocity based on direction (x,y)
            switch (direction)
            {
                case 0:
                    vel = new Vector3(1, 1, 0).normalized * rollSpeed;
                    break;
                case 1:
                    vel = new Vector3(1, -1, 0).normalized * rollSpeed;
                    break;
                case 2:
                    vel = new Vector3(-1, 1, 0).normalized * rollSpeed;
                    break;
                case 3:
                    vel = new Vector3(-1, -1, 0).normalized * rollSpeed;
                    break;
            }

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
                }
        
                yield return null;
            }

            ctx.NavMeshAgent.ResetPath();
            ctx.NavMeshAgent.velocity = Vector3.zero;
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
