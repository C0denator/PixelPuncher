using UnityEngine;

namespace GameProg.Enemies
{
    public class EnemyBullet : MonoBehaviour
    {
        
        public int Damage = 1;
        
        private void Start()
        {
            //destroy the bullet max 5 seconds after it was created
            Destroy(gameObject, 20f);
            
            //set z position to 0
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Door"))
            {
                Destroy(gameObject);
            }
            
            if (other.gameObject.CompareTag("Player"))
            {
                //look if the player has a health component
                Health.Health playerHealth = other.gameObject.GetComponent<Health.Health>();
                
                //is the component enabled?
                if (playerHealth.enabled)
                {
                    //do damage to the player
                    Debug.Log("Player hit");
                    
                    playerHealth.DoDamage(Damage);
                    
                    Destroy(gameObject);
                }
            }
            
        }
    }
}
