using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameProg.UI
{
    public class FadeOut : MonoBehaviour
    {
        [SerializeField] [Range(0.1f,3f)] private float fadeOutTime = 1f;
        [SerializeField] private Image image;

        private void Start()
        {
            //error handling
            if (image == null)
            {
                Debug.LogError("Image component not found");
                return;
            }
            
            StartCoroutine(FadeOutCoroutine());
        }

        private IEnumerator FadeOutCoroutine()
        {
            float elapsedTime = 0f;
            Color color = image.color;
            
            while (elapsedTime < fadeOutTime)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Lerp(1, 0, elapsedTime / fadeOutTime);
                image.color = color;
                yield return null;
            }
            
            Destroy(gameObject);
        }
        
    }
}
