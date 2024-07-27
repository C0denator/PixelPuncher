using System.Collections;
using Sound;
using UnityEngine;

namespace GameProg.World
{
    public class BossRoomIntro : MonoBehaviour
    {
        
        private Music _music;
        private Camera _camera;
        private Coroutine _introCoroutine;
        private GameObject _player;
        
        // Start is called before the first frame update
        void Awake()
        {
            Debug.Log("Boss spawned");  
            
            _music = FindObjectOfType<Music>();
            _player = GameObject.FindWithTag("Player");
            _camera = _player.GetComponentInChildren<Camera>();
            
            
            //error handling
            if (_music == null) Debug.LogError("Music not found in BossRoomIntro");
            if (_player == null) Debug.LogError("Player not found in BossRoomIntro");
            if (_camera == null) Debug.LogError("Camera not found in BossRoomIntro");
            
            
        }

        void Start()
        {
            //start the intro coroutine
            _introCoroutine = StartCoroutine(IntroCoroutine());
        }

        private IEnumerator IntroCoroutine()
        {
            //stop music
            _music.StopAll();
            
            //set time scale to 0
            Time.timeScale = 0f;
            
            yield return new WaitForSecondsRealtime(1f);
            
            Vector3 oldPos = new Vector3(_camera.transform.position.x, _camera.transform.position.y, _camera.transform.position.z);
            
            //move camera slowly to this position
            float timeToMove = 2f;
            float elapsedTime = 0f;
            
            while (elapsedTime < timeToMove)
            {
                Vector2 lerpPos = Vector2.Lerp(oldPos, transform.position, elapsedTime / timeToMove);
                _camera.transform.position = new Vector3(lerpPos.x, lerpPos.y, oldPos.z);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            
            _camera.transform.position = new Vector3(transform.position.x, transform.position.y, oldPos.z);
            
            yield return new WaitForSecondsRealtime(2f);
            
            //move camera slowly to old position
            Vector3 newPos = new Vector3(_camera.transform.position.x, _camera.transform.position.y, _camera.transform.position.z);
            timeToMove = 1f;
            elapsedTime = 0f;
            
            while (elapsedTime < timeToMove)
            {
                _camera.transform.position = Vector3.Lerp(newPos, oldPos, elapsedTime / timeToMove);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(1f);
            
            //play boss music
            _music.PlayClip("Boss");
            
            //set time scale to 1
            Time.timeScale = 1f;
        }
        
        private void OnDisable()
        {
            //stop the coroutine
            if(_introCoroutine != null) StopCoroutine(_introCoroutine);
        }
    }
}
