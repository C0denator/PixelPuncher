using UnityEngine;

namespace GameProg.Player
{
    public class Bullet : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            //ignore the player
            if (other.gameObject.CompareTag("Player"))
            {
                return;
            }
            
            //destroy the bullet
            Destroy(gameObject);    
        }
    }
}
