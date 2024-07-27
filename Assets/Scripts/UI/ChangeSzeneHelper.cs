using System;
using GameProg.General;
using GameProg.World;
using UnityEngine;

namespace GameProg.UI
{
    public class ChangeSzeneHelper : MonoBehaviour
    {
        
        private GameMaster _gameMaster;
        
        // Start is called before the first frame update
        void Start()
        {
            _gameMaster = FindObjectOfType<GameMaster>();
            if (_gameMaster == null) Debug.LogError("GameMaster not found");
        }

        public void ChangeSzene(String sceneName)
        {
            _gameMaster.SwitchScene(sceneName);
        }
    }
}
