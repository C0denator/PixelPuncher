using UnityEngine;

namespace GameProg.Player
{
    public class Reload : MonoBehaviour
    {
        private SpriteRenderer reloadBar;
        private Animator animator;
        public float reloadTime = 2f;
        
        // Start is called before the first frame update
        void Start()
        {
            //get references
            reloadBar = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            
            //error handling
            if (reloadBar == null)
            {
                Debug.LogError("SpriteRenderer not set");
                return;
            }
            
            if (animator == null)
            {
                Debug.LogError("Animator not set");
                return;
            }
            else
            {
                //set the reload time
                animator.speed = 1f / reloadTime;
            }
        }
        
        public void OnReloadFinished()
        {
            Debug.Log("Reloading finished");
            
            //hide the reload bar
            reloadBar.enabled = false;
        }
        
        public void OnReloadStarted()
        {
            Debug.Log("Reloading started");
            
            //show the reload bar
            reloadBar.enabled = true;
        }
    }
}
