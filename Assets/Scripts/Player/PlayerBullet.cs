using UnityEngine;

namespace GameProg.Player
{
    public class PlayerBullet : MonoBehaviour
    {

        public int Damage;
        
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
            
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit");
                
                //look if the enemy has a health component
                Health.Health enemyHealth = other.gameObject.GetComponent<Health.Health>();
                
                if(enemyHealth != null)
                {
                    //is the component enabled?
                    if (enemyHealth.enabled)
                    {
                        //do damage to the enemy
                        enemyHealth.DoDamage(Damage);
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Debug.LogWarning("Enemy has no health component");
                }
            }
            
        }
    }
}
