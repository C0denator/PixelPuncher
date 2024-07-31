using System;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Helper class to change the scene
    /// </summary>
    public class ChangeSzeneHelper : MonoBehaviour
    {
        
        private GameMaster _gameMaster;
        
        // Start is called before the first frame update
        private void Start()
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
