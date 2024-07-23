using System;
using UnityEngine;

namespace GameProg.Health
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxHealth;
        
        public event Action OnHealthChanged;
        public event Action OnDeath;
        
        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                currentHealth = value;
                OnHealthChanged?.Invoke();
            }
        }
        
        // Start is called before the first frame update
        void Start()
        {
            currentHealth = maxHealth;
        }
        
        public void DoDamage(int damage)
        {
            CurrentHealth -= damage;
            
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
                
                OnDeath?.Invoke();
            }
        }
    }
}
