using UnityEngine;

namespace GameProg.Player
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]private bool playerBullet;
        
        public bool PlayerBullet { get; set; }
        
        private void Start()
        {
            //destroy the bullet max 5 seconds after it was created
            Destroy(gameObject, 5f);
            
            //set z position to 0
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Door"))
            {
                Destroy(gameObject);
            }
            
            if (playerBullet && other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit");
                Destroy(gameObject);
            }
            
            if (!playerBullet && other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player hit");
                
                //is player dashin?
                if (other.gameObject.GetComponent<PlayerController>().IsDashing)
                {
                    //destroy the bullet
                    Destroy(gameObject);
                }
                else
                {
                    //deal damage to the player
                    other.gameObject.GetComponent<PlayerController>().TakeDamage(1);
                
                Destroy(gameObject);
            }  
        }
    }
}
