using UnityEngine;

namespace GameProg.General
{
    public class GameMaster : MonoBehaviour
    {
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
            
            //switch scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
