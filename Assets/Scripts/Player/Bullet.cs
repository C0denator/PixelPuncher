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
                Destroy(gameObject);
            }  
        }
    }
}
