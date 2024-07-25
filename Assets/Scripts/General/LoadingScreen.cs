using System;
using UnityEngine;

namespace GameProg.General
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private World.World _world;
        
        [SerializeField] private GameObject _loadingScreen;
        
        private GameMaster _gameMaster;

        private void Start()
        {
            _gameMaster = FindObjectOfType<GameMaster>();
            
            _loadingScreen.SetActive(true);
            
            //subscribe to the world generated event
            World.World.OnWorldGenerated += HandleOnWorldGenerated;
        }
        
        private void HandleOnWorldGenerated()
        {
            _loadingScreen.SetActive(false);
            
            //unsubscribe from the world generated event
            World.World.OnWorldGenerated -= HandleOnWorldGenerated;
        }

        private void OnValidate()
        {
            //unsubscribe from the world generated event
            World.World.OnWorldGenerated -= HandleOnWorldGenerated;
        }
    }
}
