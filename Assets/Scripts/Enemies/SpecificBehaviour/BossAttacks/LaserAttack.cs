using System.Collections;
using UnityEngine;

namespace Enemies.SpecificBehaviour.BossAttacks
{
    /// <summary>
    /// Boss attack, which was not implemented yet
    /// </summary>
    public class LaserAttack : BossAttack
    {
        [SerializeField] [Range(3f,10f)] private float attackDuration;
        
        private float _elapsedTime;
        
        private void Awake()
        {
            
        }

        public override void StartAttack(BossController ctx)
        {
            if(_attackCoroutine == null)
            {
                _elapsedTime = 0;
                _attackCoroutine = StartCoroutine(LaserAttackCoroutine());
            }
            else
            {
                Debug.LogWarning("Attack already in progress");
            }
        }
        
        private IEnumerator LaserAttackCoroutine()
        {
            Debug.Log("Laser attack started");
            
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
