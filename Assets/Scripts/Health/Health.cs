using System;
using Sound;
using UnityEngine;

namespace GameProg.Health
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxHealth;
        [Header("HitSounds")] 
        [SerializeField] private AudioClipWithVolume hitSound;
        [SerializeField] private AudioClipWithVolume deathSound;

        private AudioSource _audioSource;
        
        public event Action OnHealthChanged;
        public event Action<GameObject> OnDeath;
        
        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                int tmp = currentHealth;
                currentHealth = value;
                
                if(tmp != currentHealth)
                {
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
        void Start()
        {
            currentHealth = maxHealth;
            
            //get audio source
            _audioSource = GlobalSound.globalAudioSource;
        }
        
        public void DoDamage(int damage)
        {
            CurrentHealth -= damage;
            
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
                
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
