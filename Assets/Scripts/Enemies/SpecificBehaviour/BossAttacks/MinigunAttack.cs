using System.Collections;
using UnityEngine;

namespace GameProg.Enemies.SpecificBehaviour.BossAttacks
{
    public class MinigunAttack : BossAttack
    {
        [SerializeField] [Range(3f,10f)] private float attackDuration;
        [SerializeField] [Range(2f,10f)] private float moveSpeed;
        
        private float _elapsedTime;
        
        private void Awake()
        {
            
        }

        public override void StartAttack(BossController ctx)
        {
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

            while (_elapsedTime < attackDuration)
            {
                _elapsedTime += Time.deltaTime;

                //move towards player
                ctx.NavMeshAgent.SetDestination(ctx.Player.position);

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
    }
}
