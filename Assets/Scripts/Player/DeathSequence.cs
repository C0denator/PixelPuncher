using System;
using System.Collections;
using GameProg.General;
using GameProg.World;
using Sound;
using UnityEngine;

namespace GameProg.Player
{
    public class DeathSequence : MonoBehaviour
    {
        [SerializeField] private Camera _deathCamera;
        [SerializeField] private AudioClipWithVolume _deathSound;
        
        private Health.Health _playerHealth;
        
        private Coroutine _deathSequenceCoroutine;
        private Music _music;
        
        // Start is called before the first frame update
        private void Start()
        {
            
            //get references
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player not found");
                return;
            }
            _playerHealth = player.GetComponent<Health.Health>();
            _music = FindObjectOfType<Music>();
            
            //error handling
            if (_playerHealth == null) Debug.LogError("Health component not found on player");
            if (_deathCamera == null) Debug.LogError("Death camera not set");
            if (_music == null) Debug.LogError("Music not found");
            
            //subscribe to the death event
            _playerHealth.OnDeath += HandleOnDeath;
        }
        
        private void HandleOnDeath(GameObject obj)
        {
            if(_deathSequenceCoroutine == null) StartCoroutine(DeathSequenceCoroutine());
        }
        
        private IEnumerator DeathSequenceCoroutine()
        {
            Debug.Log("Player died");
            
            //stop music
            _music.StopAll();
            
            //stop time
            Time.timeScale = 0;
            
            yield return new WaitForSecondsRealtime(1f);

            while (_deathCamera.orthographicSize > 5)
            {
                _deathCamera.orthographicSize -= 10 * Time.unscaledDeltaTime;
                yield return null;
            }
            _deathCamera.orthographicSize = 5;

            yield return new WaitForSecondsRealtime(1f);
            
            //play death sound
            _playerHealth.AudioSource.PlayOneShot(_deathSound.clip, _deathSound.volume);

            while (transform.localScale.y > 0)
            {
                transform.localScale -= new Vector3(0, 1, 0) * Time.unscaledDeltaTime;
                yield return null;
            }
            transform.localScale = new Vector3(1, 0, 1);
            
            Debug.Log("Death sequence finished");
            
            //change to main menu
            FindObjectOfType<GameMaster>().SwitchScene("MainMenu");
            
            //reset time
            Time.timeScale = 1;

            yield return null;
        }

        private void OnDisable()
        {
            //unsubscribe from the death event
            _playerHealth.OnDeath -= HandleOnDeath;
        }

        [Serializable]
        private struct AudioClipWithVolume
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }
    }
}
