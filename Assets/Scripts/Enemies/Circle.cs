using System.Collections;
using GameProg.Health;
using UnityEngine;

namespace Enemies
{
    public class Circle : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 1f;
        [SerializeField] private float _expandSpeed = 1f;
    
        private float _timeLeft = 0f;
        public int _damage = 1;
        
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Explode());
        }
    
        private IEnumerator Explode()
        {
            //set scale to 0
            transform.localScale = Vector3.zero;
        
            //expand the circle; expansions slows down over time
            while (_timeLeft > 0)
            {
                transform.localScale += Vector3.one * _expandSpeed * (_timeLeft / _lifeTime) * Time.deltaTime;
            
                _timeLeft -= Time.deltaTime;
                yield return null;
            }
        
            //destroy the object
            Destroy(gameObject,0.1f);
        
            yield return null;
        }

        // Update is called once per frame
        private void OnEnable()
        {
            _timeLeft = _lifeTime;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //look if the player has a health component
                Health playerHealth = other.gameObject.GetComponent<Health>();
                
                //is the component enabled?
                if (playerHealth.enabled)
                {
                    //do damage to the player
                    Debug.Log("Player hit");
                    
                    playerHealth.DoDamage(_damage);
                    
                    Destroy(gameObject);
                }
            }
            
        }
    }
}
