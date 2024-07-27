using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Sound
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
        
        private static Music _instance;
        
        // Start is called before the first frame update
        void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }

        public void PlayClip(string clipName)
        {
            PlayableAudioClip clip;
            
            //default values so Unity will shut up
            clip = playableAudioClips[0];
            clip.audioClip = null;
            clip.volume = 1f;
            
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
                    audioSourceA.clip = clip.audioClip;
                    audioSourceA.volume = 0;
                    audioSourceA.clip.LoadAudioData();
                    audioSourceA.Play();
                    StartCoroutine(FadeIn(audioSourceA, clip)); 
                }
                else
                {
                    audioSourceA.clip = clip.audioClip;
                    audioSourceA.volume = clip.volume;
                    audioSourceA.clip.LoadAudioData();
                    audioSourceA.Play();
                }
                
                
                _currentAudioSource = audioSourceA;
            }
            else if(_currentAudioSource == audioSourceA)
            {
                audioSourceB.clip = clip.audioClip;
                audioSourceB.volume = 0;
                audioSourceB.clip.LoadAudioData();
                audioSourceB.Play();
                
                StartCoroutine(FadeOut(audioSourceA));
                StartCoroutine(FadeIn(audioSourceB, clip));
                
                _currentAudioSource = audioSourceB;
            }else if(_currentAudioSource == audioSourceB)
            {
                audioSourceA.clip = clip.audioClip;
                audioSourceA.volume = 0;
                audioSourceA.clip.LoadAudioData();
                audioSourceA.Play();
                
                StartCoroutine(FadeOut(audioSourceB));
                StartCoroutine(FadeIn(audioSourceA, clip));
                
                _currentAudioSource = audioSourceA;
            }
            
            
        }
        
        private IEnumerator FadeIn(AudioSource audioSource, PlayableAudioClip audioClip)
        {
            
            //fade in 
            float t = 0;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0, audioClip.volume, t / fadeTime);
                
                yield return null;
            }
            
        }
        
        private IEnumerator FadeOut(AudioSource audioSource)
        {
            //fade out
            float t = 0;
            
            //get current volume
            float startVolume = audioSource.volume;
            
            //fade out
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
                
                yield return null;
            }
            
            
            //stop the audio
            audioSource.Stop();
            
            yield return null;
        }
        
        public void StopAll()
        {
            audioSourceA.Stop();
            audioSourceB.Stop();
        }
    }

    [Serializable] public struct PlayableAudioClip
    {
        public String name;
        public AudioClip audioClip;
        [Range(0f, 1f)] public float volume;
    }
}
