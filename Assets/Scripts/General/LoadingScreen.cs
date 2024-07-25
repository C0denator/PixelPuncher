using UnityEngine;

namespace GameProg.General
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Camera _loadingCamera;
        [SerializeField] private Camera _newCamera;
        [SerializeField] private World.World _world;
        
        private GameMaster _gameMaster;

        private void Start()
        {
            _gameMaster = FindObjectOfType<GameMaster>();
            
            //subscribe to the world generated event
            World.World.OnWorldGenerated += HandleOnWorldGenerated;
        }
        
        private void HandleOnWorldGenerated()
        {
            //disable the loading camera
            _loadingCamera.gameObject.SetActive(false);
            
            //enable the new camera
            _newCamera.gameObject.SetActive(true);
            
            //call camera changed event
            _gameMaster.OnCameraChanged?.Invoke(_newCamera);
            
            //unsubscribe from the world generated event
            World.World.OnWorldGenerated -= HandleOnWorldGenerated;
            
            //disable this gameobject
            gameObject.SetActive(false);
        }
    }
}
