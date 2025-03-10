using System;
using Sound;
using UnityEngine;

namespace Health
{
    /// <summary>
    /// Health component for objects that can take damage. Also handles sound effects for taking damage and dying
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxHealth;
        public bool invincible;
        [Header("HitSounds")] 
        [SerializeField] private AudioClipWithVolume hitSound;
        [SerializeField] private AudioClipWithVolume deathSound;

        private AudioSource _audioSource;
        
        public AudioSource AudioSource => _audioSource;
        
        public event Action OnHealthChanged;
        public event Action<GameObject> OnDeath;
        
        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                
                if(invincible) return;
                
                int tmp = currentHealth;
                currentHealth = value;
                
                if(tmp != currentHealth)
                {
                    Debug.Log("Health changed to " + currentHealth);
                    
                    OnHealthChanged?.Invoke();
                    
                    if(tmp > currentHealth)
                    {
                        _audioSource.PlayOneShot(hitSound.clip, hitSound.volume);
                    }
                }
            }
        }
        
        public int MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            currentHealth = maxHealth;
            
            //get audio source
            _audioSource = FindObjectOfType<GlobalSound>().globalAudioSource;
        }
        
        public void DoDamage(int damage)
        {
            CurrentHealth -= damage;
            
            if (CurrentHealth < 0) CurrentHealth = 0;

            if (CurrentHealth == 0)
            {
                _audioSource.PlayOneShot(deathSound.clip, deathSound.volume);
                
                OnDeath?.Invoke(gameObject);
            }
        }

        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
    }
}
