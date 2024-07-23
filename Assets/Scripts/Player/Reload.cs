using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace GameProg.Player
{
    public class Reload : MonoBehaviour
    {
        private SpriteRenderer reloadBar;
        private Animator animator;
        public float reloadTime = 2f;
        private static readonly int ReloadStart = Animator.StringToHash("Reload");

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
        
        public void StartReload(float time)
        {
            //set the reload time
            reloadTime = time;
            animator.speed = 1f / reloadTime;
            
            //start animation
            animator.SetTrigger(ReloadStart);
            
            //show the reload bar
            reloadBar.enabled = true;
        }
        
        public void FinishReload()
        {
            //hide the reload bar
            reloadBar.enabled = false;
        }
    }
}
