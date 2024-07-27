using UnityEngine;

namespace Sound
{
    public class GlobalSound : MonoBehaviour
    {
    
        private static GlobalSound instance;
    
        public AudioSource globalAudioSource;

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        
            globalAudioSource = GetComponent<AudioSource>();
        
            if (globalAudioSource == null)
            {
                Debug.LogError("Global AudioSource not set");
            }
        }
    }
}
