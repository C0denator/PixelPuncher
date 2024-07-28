using System.Collections;
using UnityEngine;

namespace GameProg.Enemies.SpecificBehaviour.BossAttacks
{
    public class MinigunAttack : BossAttack
    {
        [SerializeField] [Range(3f,10f)] private float attackDuration;
        
        private float _elapsedTime;
        
        private void Awake()
        {
            _elapsedTime = 0;
        }

        public override void StartAttack(BossController ctx)
        {
            if(_attackCoroutine == null)
            {
                _attackCoroutine = StartCoroutine(MinigunAttackCoroutine());
            }
            else
            {
                Debug.LogWarning("Attack already in progress");
            }
        }
        
        private IEnumerator MinigunAttackCoroutine()
        {
            Debug.Log("Minigun attack started");
            
            //TODO: Implement attack
            yield return new WaitForSeconds(attackDuration);
            
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
