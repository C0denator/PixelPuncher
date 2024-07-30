using UnityEngine;

namespace UI
{
    public class Quit : MonoBehaviour
    {
        public void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
