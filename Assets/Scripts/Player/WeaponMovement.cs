using UnityEngine;

namespace GameProg.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class WeaponMovement : MonoBehaviour
    {
        [SerializeField] private Transform rotationCenter; //the player, where the weapon rotates around
        [SerializeField] private float distance = 1f; //distance from the player
        private SpriteRenderer spriteRenderer;
        
        // Start is called before the first frame update
        private void Start()
        {
            //get the sprite renderer
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            //error handling
            if (rotationCenter == null)
            {
                Debug.LogError("Rotation center not set");
                return;
            }
            
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer not set");
                return;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            //return if player is dead
            if (Time.timeScale == 0) return;
            
            //look at the mouse, while rotating around the player
            
            //get the mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            //get the direction from the player to the mouse
            Vector3 direction = mousePos - rotationCenter.position;
            
            //set the weapon position
            transform.position = rotationCenter.position + direction.normalized * distance;
            
            //rotate the weapon
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            //flip the weapon sprite if the mouse is on the left side
            if (direction.x < 0)
            {
                spriteRenderer.flipY = true;
                //angle += 180;
            }
            else
            {
                spriteRenderer.flipY = false;
            }
            
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
