
using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace GameProg.General
{
    public class Music : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSourceA;
        [SerializeField] private AudioSource audioSourceB;
        [SerializeField] private PlayableAudioClip startAudioClip;
        [SerializeField] private float fadeTime = 1f;
        [SerializeField] private bool fadeOnStart = true;
        
        [CanBeNull] private AudioSource _currentAudioSource;
        
        [SerializeField] private PlayableAudioClip[] playableAudioClips;
        
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            //start with "startAudioClip"
            PlayClip("menu");
        }

        public void PlayClip(string clipName)
        {
            PlayableAudioClip clip;
            clip.audioClip = null;
            bool found = false;
            
            //find the clip with the name
            foreach (var playableAudioClip in playableAudioClips)
            {
                if(playableAudioClip.name == clipName)
                {
                    clip = playableAudioClip;
                    found = true;
                    break;
                }
            }
            
            //error handling
            if (!found)
            {
                Debug.LogError("Clip with name "+clipName+" not found");
                return;
            }
            
            //play the clip
            if (_currentAudioSource == null)
            {
                if (fadeOnStart)
                {
                    StartCoroutine(FadeIn(audioSourceA, clip.audioClip)); 
                }
                else
                {
                    audioSourceA.clip = clip.audioClip;
                    audioSourceA.volume = 1;
                    audioSourceA.Play();
                }
                
                
                _currentAudioSource = audioSourceA;
            }
            else if(_currentAudioSource == audioSourceA)
            {
                StartCoroutine(FadeOut(audioSourceA));
                StartCoroutine(FadeIn(audioSourceB, clip.audioClip));
                
                _currentAudioSource = audioSourceB;
            }else if(_currentAudioSource == audioSourceB)
            {
                StartCoroutine(FadeOut(audioSourceB));
                StartCoroutine(FadeIn(audioSourceA, clip.audioClip));
                
                _currentAudioSource = audioSourceA;
            }
            
            
        }
        
        private IEnumerator FadeIn(AudioSource audioSource, AudioClip audioClip)
        {
            //set the clip
            audioSource.clip = audioClip;
            audioSource.Play();
            
            //fade in
            float t = 0;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                audioSource.volume = t / fadeTime;
                yield return null;
            }
        }
        
        private IEnumerator FadeOut(AudioSource audioSource)
        {
            //fade out
            float t = fadeTime;
            while (t > 0)
            {
                t -= Time.deltaTime;
                audioSource.volume = t / fadeTime;
                yield return null;
            }
            
            //stop the audio
            audioSource.Stop();
            
            yield return null;
        }
    }

    [Serializable] public struct PlayableAudioClip
    {
        public String name;
        public AudioClip audioClip;
    }
}
