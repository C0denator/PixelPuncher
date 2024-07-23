using System.Collections.Generic;
using UnityEngine;

namespace GameProg.UI
{
    public class BorderCanvas : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _menuElements;
        [SerializeField] private List<GameObject> _loadingElements;
        [SerializeField] private Canvas _borderCanvas;
        private World.World currentWorld;
        
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            //subscribe to the scene loaded event
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            
            //show all menu elements
            foreach (var element in _menuElements)
            {
                element.SetActive(true);
            }
            
            //hide all loading elements
            foreach (var element in _loadingElements)
            {
                element.SetActive(false);
            }
        }
        
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            //reset canvas
            _borderCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _borderCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            
            //was the main menu loaded?
            if (scene.name == "MainMenu")
            {
                //show all menu elements
                foreach (var element in _menuElements)
                {
                    element.SetActive(true);
                }
            }
            else
            {
                //hide all menu elements
                foreach (var element in _menuElements)
                {
                    element.SetActive(false);
                }
                
                //show all loading elements
                foreach (var element in _loadingElements)
                {
                    element.SetActive(true);
                }
                
                //find world
                currentWorld = FindObjectOfType<World.World>();
            
                //error handling
                if (currentWorld == null)
                {
                    Debug.LogError("World not found");
                    return;
                }
                else
                {
                    //subscribe to the world generated event
                    World.World.OnWorldGenerated += HandleOnWorldGenerated;
                }
            }
        }
        
        private void HandleOnWorldGenerated()
        {
            //hide all loading elements
            foreach (var element in _loadingElements)
            {
                element.SetActive(false);
            }
        }
    }
}
