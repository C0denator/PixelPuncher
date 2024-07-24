using UnityEngine;

namespace Sound
{
    public class GlobalSound : MonoBehaviour
    {
    
        private static GlobalSound instance;
    
        public static AudioSource globalAudioSource;

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning("GlobalSound already exists. Destroying new instance.");
                Destroy(gameObject);
            }
        
            globalAudioSource = GetComponent<AudioSource>();
        
            if (globalAudioSource == null)
            {
                Debug.LogError("Global AudioSource not set");
            }
        }
    }
}
