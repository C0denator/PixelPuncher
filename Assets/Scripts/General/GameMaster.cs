using System;
using JetBrains.Annotations;
using UnityEngine;

namespace GameProg.General
{
    public class GameMaster : MonoBehaviour
    {
        [SerializeField] private Music music;
        [SerializeField] private GameProg.World.World currentWorld;
        [SerializeField] [CanBeNull] private Camera currentCamera;
        public AudioSource globalAudioSource;
        
        public Action<Camera> OnCameraChanged;
        
        public Camera CurrentCamera
        {
            get => currentCamera;
            set
            {
                currentCamera = value;
                OnCameraChanged?.Invoke(currentCamera);
            }
        }
        
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            //subscribe to the scene loaded event
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            
            //subscribe to the world generated event
            GameProg.World.World.OnWorldGenerated += OnWorldGenerated;
        }
        
        public void SwitchScene(string sceneName)
        {
            //does the scene exist?
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is empty");
                return;
            }
            
            //switch scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            //find world
            currentWorld = FindObjectOfType<GameProg.World.World>();
            
            //error handling
            if (currentWorld == null)
            {
                Debug.LogError("World not found");
                return;
            }
        }
        
        private void OnWorldGenerated()
        {
            //error handling
            if (currentWorld == null)
            {
                Debug.LogError("World not found");
                return;
            }
            
            //if world1
            if (currentWorld.name == "World1")
            {
                music.PlayClip("world1");
            }
        }
    }
}