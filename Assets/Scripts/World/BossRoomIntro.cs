using System;
using UnityEngine;

namespace GameProg.World
{
    public class BossRoomIntro : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            Debug.Log("Boss spawned");  
        }

        private void OnEnable()
        {
        }
        
        private void OnDisable()
        {
        }
    }
}
