using UnityEngine;

namespace UI
{
    /// <summary>
    /// Component to quit the game
    /// </summary>
    public class Quit : MonoBehaviour
    {
        public void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
