using System;
using System.Collections;
using UnityEngine;

namespace GameProg.Enemies.SpecificBehaviour.BossAttacks
{
    public class FlyAttack : BossAttack
    {
        [SerializeField] [Range(0.1f,5f)] private float amplitude;
        [SerializeField] [Range(0.1f,5f)] private float frequency;
        [SerializeField] [Range(0.1f,5f)] private float horizontalSpeed;
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
                _attackCoroutine = StartCoroutine(FlyAttackCoroutine());
            }
            else
            {
                Debug.LogWarning("Attack already in progress");
            }
        }
        
        private IEnumerator FlyAttackCoroutine()
        {
            Debug.Log("Fly attack started");
            
            //TODO: Implement attack
            
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
