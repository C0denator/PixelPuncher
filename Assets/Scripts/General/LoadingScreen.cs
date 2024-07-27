using System;
using GameProg.World;
using UnityEngine;

namespace GameProg.General
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private World.World _world;
        
        [SerializeField] private GameObject _loadingScreen;
        
        private GameMaster _gameMaster;

        private void Awake()
        {
            _gameMaster = FindObjectOfType<GameMaster>();
            
            //error checking
            if (_world == null) Debug.LogError("World reference not found in LoadingScreen");
            if (_loadingScreen == null) Debug.LogError("LoadingScreen reference not set in LoadingScreen");
        }

        private void Start()
        {
            _loadingScreen.SetActive(true);
        }

        private void HandleOnWorldGenerated()
        {
            _loadingScreen.SetActive(false);
            
            //unsubscribe from the world generated event
            _world.OnWorldGenerated -= HandleOnWorldGenerated;
        }

        private void OnEnable()
        {
            //subscribe to the world generated event
            _world.OnWorldGenerated += HandleOnWorldGenerated;
        }
        
        private void OnDisable()
        {
            //unsubscribe from the world generated event
            _world.OnWorldGenerated -= HandleOnWorldGenerated;
        }
    }
}
