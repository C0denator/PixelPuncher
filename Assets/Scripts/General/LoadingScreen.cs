using UnityEngine;

namespace GameProg.General
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Camera _loadingCamera;
        [SerializeField] private World.World _world;

        private void Start()
        {
            //subscribe to the world generated event
            World.World.OnWorldGenerated += HandleOnWorldGenerated;
        }
        
        private void HandleOnWorldGenerated()
        {
            //disable the loading camera
            _loadingCamera.gameObject.SetActive(false);
            
            //unsubscribe from the world generated event
            World.World.OnWorldGenerated -= HandleOnWorldGenerated;
            
            //disable this gameobject
            gameObject.SetActive(false);
        }
    }
}
