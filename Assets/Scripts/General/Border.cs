using UnityEngine;

namespace GameProg.General
{
    [RequireComponent(typeof(Canvas))]
    public class Border : MonoBehaviour
    {
        [SerializeField] private GameMaster gameMaster;
        private Canvas _canvas;

        private void Start()
        {
            //get the game master
            if (gameMaster == null)
            {
                gameMaster = FindObjectOfType<GameMaster>();
                
                //error handling
                if (gameMaster == null)
                {
                    Debug.LogError("Game master not found");
                    return;
                }
            }
            
            //get the canvas
            _canvas = GetComponent<Canvas>();
            
            //error handling
            if (_canvas == null)
            {
                Debug.LogError("Canvas not found");
                return;
            }
            
            //subscribe to the camera changed event
            gameMaster.OnCameraChanged += HandleOnCameraChanged;
            
            //subscribe to the scene unloaded event
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        
        private void HandleOnCameraChanged(Camera newCamera)
        {
            //set the new camera
            _canvas.worldCamera = newCamera;
        }
        
        private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
        {
            //unsubscribe from the camera changed event
            gameMaster.OnCameraChanged -= HandleOnCameraChanged;
        }
    }
}
