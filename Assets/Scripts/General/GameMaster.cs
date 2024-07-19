using UnityEngine;

namespace GameProg.General
{
    public class GameMaster : MonoBehaviour
    {
        [SerializeField] private Music music;
        
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        public void SwitchScene(string sceneName)
        {
            //does the scene exist?
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is empty");
                return;
            }
            
            if (sceneName == "World1")
            {
                music.PlayClip("world1");
            }
            
            //switch scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
