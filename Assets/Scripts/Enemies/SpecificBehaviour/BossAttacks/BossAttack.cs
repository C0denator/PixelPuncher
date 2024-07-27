using System;
using UnityEngine;

namespace GameProg.Enemies.SpecificBehaviour.BossAttacks
{
    public abstract class BossAttack : MonoBehaviour
    {
        public Action OnAttackFinished;
        
        public void StartAttack(BossController ctx){}
    }
}
