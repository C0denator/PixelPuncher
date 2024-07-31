using UnityEngine;

namespace UI
{ 
    /// <summary>
    /// Handles the UI element that toggles the difficulty
    /// </summary>
    public class GigachadButton : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text buttonText;
        private GameMaster _gameMaster;
        
        // Start is called before the first frame update
        void Awake()
        {
            //get GameMaster instance
            _gameMaster = FindObjectOfType<GameMaster>();
            
            //error handling
            if(_gameMaster == null) Debug.LogError("GameMaster not found");
        }

        public void ToggleGigachadMode()
        {
            _gameMaster.GigachadMode = !_gameMaster.GigachadMode;
            
            if (_gameMaster.GigachadMode)
            {
                buttonText.text = "Difficulty: Gigachad";
                buttonText.color = Color.red;
            }
            else
            {
                buttonText.text = "Difficulty: Easy";
                buttonText.color = Color.white;
            }
        }

        private void OnEnable()
        {
            if (_gameMaster.GigachadMode)
            {
                buttonText.text = "Difficulty: Gigachad";
                buttonText.color = Color.red;
            }
            else
            {
                buttonText.text = "Difficulty: Easy";
                buttonText.color = Color.white;
            }
        }
    }
}
