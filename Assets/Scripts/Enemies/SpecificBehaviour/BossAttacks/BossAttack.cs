using System;
using UnityEngine;

namespace Enemies.SpecificBehaviour.BossAttacks
{
    public abstract class BossAttack : MonoBehaviour
    {
        public Action OnAttackFinished;
        
        protected Coroutine _attackCoroutine;
        
        public virtual void StartAttack(BossController ctx){
            throw new NotImplementedException();
        }
        
        public virtual void StopAttack(){
            if(_attackCoroutine != null){
                StopCoroutine(_attackCoroutine);
            }
        }

        protected void OnDisable()
        {
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }
        }
    }
}
