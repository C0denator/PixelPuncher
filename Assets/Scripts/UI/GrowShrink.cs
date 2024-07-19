using System;
using UnityEngine;

namespace GameProg.UI
{
    /// <summary>
    /// This script is used to grow and shrink objects.
    /// </summary>
    public class GrowShrink : MonoBehaviour
    {
        
        [SerializeField] private float growTime = 1f;
        [SerializeField] private float shrinkTime = 1f;
        [SerializeField] private float maxSize = 2f;
        [SerializeField] private float minSize = 0.5f;
        private bool _isGrowing = true;
        private float elapsedTime = 0f;
        

        // Update is called once per frame
        private void Update()
        {
            if (_isGrowing)
            {
                elapsedTime += Time.deltaTime;
                transform.localScale = Vector3.one * Mathf.Lerp(minSize, maxSize, elapsedTime / growTime);
                if (elapsedTime >= growTime)
                {
                    transform.localScale = Vector3.one * maxSize;
                    _isGrowing = false;
                    elapsedTime = 0f;
                }
            }
            else
            {
                elapsedTime += Time.deltaTime;
                transform.localScale = Vector3.one * Mathf.Lerp(maxSize, minSize, elapsedTime / shrinkTime);
                if (elapsedTime >= shrinkTime)
                {
                    transform.localScale = Vector3.one * minSize;
                    _isGrowing = true;
                    elapsedTime = 0f;
                }
            }
        }
    }
}
