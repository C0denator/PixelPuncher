using System;
using UnityEngine;

namespace GameProg.Player
{
    public class Bullet : MonoBehaviour
    {
        private void Start()
        {
            //destroy the bullet max 5 seconds after it was created
            Destroy(gameObject, 5f);
        }

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
