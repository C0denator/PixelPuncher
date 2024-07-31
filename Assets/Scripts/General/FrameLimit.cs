using UnityEngine;

namespace General
{
    /// <summary>
    /// Limits the frame rate of the game
    /// </summary>
    public class FrameLimit : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private float DeltaTime;
        
        private void Awake()
        {
            //disable v-sync
            QualitySettings.vSyncCount = 0;
            
            Application.targetFrameRate = targetFrameRate;
        }
        
        private void OnValidate()
        {
            //apply the target frame rate
            Application.targetFrameRate = targetFrameRate;
        }
        
        private void Update()
        {
            //calculate the delta time
            DeltaTime = Time.deltaTime;
        }
    }
    
    
}
