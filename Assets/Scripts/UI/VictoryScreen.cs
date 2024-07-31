using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Handles the life cycle of the victory screen
    /// </summary>
    public class VictoryScreen : MonoBehaviour
    {
        [SerializeField] private Image fadeOutImage;
        [SerializeField] private float fadeOutTime = 1f;
        [SerializeField] private Image gigachad;
        [SerializeField] private float fadeInTime = 10f;
        private float quitTime = 79.685f;
        
        private GameMaster _gameMaster;
        private float _elapsedTime;

        private void Awake()
        {
            _gameMaster = FindObjectOfType<GameMaster>();
            
            if (_gameMaster == null) Debug.LogError("GameMaster not found");
        }

        // Update is called once per frame
        private void Start()
        {
            StartCoroutine(FadeOut());

            if (_gameMaster.GigachadMode) StartCoroutine(FadeIn());
        }

        private void FixedUpdate()
        {
            _elapsedTime += Time.fixedDeltaTime;
            
            if (_elapsedTime > quitTime)
            {
                Application.Quit();
                Debug.Log("Quit");
            }
        }

        private IEnumerator FadeOut()
        {
            float elapsedTime = 0;
            Color color = fadeOutImage.color;
        
            while (elapsedTime < fadeOutTime)
            {
                color.a = Mathf.Lerp(1, 0, elapsedTime / fadeOutTime);
                fadeOutImage.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            color.a = 0;
            fadeOutImage.color = color;
        }
    
        private IEnumerator FadeIn()
        {
            yield return new WaitForSeconds(5f);
            
            float elapsedTime = 0;
            Color color = gigachad.color;
        
            while (elapsedTime < fadeInTime)
            {
                color.a = Mathf.Lerp(0, 1, elapsedTime / fadeInTime);
                gigachad.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            color.a = 1;
            gigachad.color = color;
        }

        private void OnEnable()
        {
            //set the alpha of the images
            Color tmp = fadeOutImage.color;
            tmp.a = 255;
            fadeOutImage.color = tmp;
        
            tmp = gigachad.color;
            tmp.a = 0;
            gigachad.color = tmp;
            
            _elapsedTime = 0;
        }
    }
}
