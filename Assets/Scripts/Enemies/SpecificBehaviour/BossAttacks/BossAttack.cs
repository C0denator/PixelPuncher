using System;
using UnityEngine;

namespace GameProg.Enemies.SpecificBehaviour.BossAttacks
{
    public abstract class BossAttack : MonoBehaviour
    {
        public Action OnAttackFinished;
        
        protected Coroutine _attackCoroutine;
        
        public virtual void StartAttack(BossController ctx){
            throw new NotImplementedException();
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
