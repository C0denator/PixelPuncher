using UnityEngine;

namespace GameProg.General
{
    public class FrameLimit : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 60;
        
        private void Awake()
        {
            //disable v-sync
            QualitySettings.vSyncCount = 0;
            
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
