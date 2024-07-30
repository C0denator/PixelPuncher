using System;
using System.Collections;
using GameProg.World;
using JetBrains.Annotations;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private Music music;
    [SerializeField] private World currentWorld;
    [SerializeField] [CanBeNull] private Camera currentCamera;
        
    private static GameMaster _instance;
        
    public Action<Camera> OnCameraChanged;
    public Action OnAllRoomsCleared;
        
    private int _clearedRooms;
    [SerializeField] private bool _gigachadMode = false;
        
    public bool GigachadMode
    {
        get => _gigachadMode;
        set => _gigachadMode = value;
    }
        
    public Camera CurrentCamera
    {
        get => currentCamera;
        set
        {
            currentCamera = value;
            OnCameraChanged?.Invoke(currentCamera);
        }
    }

    private void Awake()
    {
        //singleton pattern
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
            
        DontDestroyOnLoad(gameObject);
            
        //subscribe to the scene events
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
        
    public void SwitchScene(string sceneName)
    {
        //does the scene exist?
        /*if (SceneManager.GetSceneByName(sceneName).buildIndex == -1)
            {
                Debug.LogError("Scene "+sceneName+" not found");
                return;
            }*/
            
        //switch scene
        SceneManager.LoadScene(sceneName);
    }
        
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: "+scene.name);
            
        _clearedRooms = 0;
            
        //find world
        currentWorld = FindObjectOfType<World>();
            
        if(currentWorld != null)
        {
            //subscribe to the world generated event
            currentWorld.OnWorldGenerated += OnWorldGenerated;
        }
        else
        {
            Debug.Log("World not found");
        }

        if (SceneManager.GetActiveScene().name=="MainMenu")
        {
            music.PlayClip("MainMenu");
        }else if(SceneManager.GetActiveScene().name=="VictoryScreen")
        {
            music.PlayClip("Victory");
        }
    }
        
    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("Scene unloaded: "+scene.name);
            
        //unsubscribe from the world generated event
        if (currentWorld != null)
        {
            currentWorld.OnWorldGenerated -= OnWorldGenerated;
        }
            
        //unsubscribe from the room cleared event
        if (currentWorld != null)
        {
            for(int i=0; i<currentWorld.GeneratedRooms.Count; i++)
            {
                currentWorld.GeneratedRooms[i].OnRoomCleared -= HandleOnRoomCleared;
            }
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
        /*if (currentWorld.name == "World1")
        {
            music.PlayClip("World1");
        }*/
            
        //subscribe to the room cleared event
        for(int i=0; i<currentWorld.GeneratedRooms.Count; i++)
        {
            currentWorld.GeneratedRooms[i].OnRoomCleared += HandleOnRoomCleared;
        }
            
        //unsub from the world generated event
        currentWorld.OnWorldGenerated -= OnWorldGenerated;
    }
        
    private void HandleOnRoomCleared()
    {
        _clearedRooms++;
            
        if (_clearedRooms >= currentWorld.GeneratedRooms.Count - 2)
        {
            OnAllRoomsCleared?.Invoke();
        }
    }
    
    public void OnVictory()
    {
        StartCoroutine(VictorySequence());
    }
    
    private IEnumerator VictorySequence()
    {
        //stop all music
        music.StopAll();
        
        //set timescale to 0
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(2f);
        
        //get current camera
        currentCamera = Camera.main;
        
        float sizeChange = 0.1f;

        while (currentCamera.orthographicSize < 100000)
        {
            currentCamera.orthographicSize += sizeChange;

            sizeChange *= 1.1f;
            
            yield return new WaitForSecondsRealtime(0.01f);
        }
        
        //set timescale back to 1
        Time.timeScale = 1;
        
        //change scene to victory screen
        SwitchScene("VictoryScreen");
    }
}