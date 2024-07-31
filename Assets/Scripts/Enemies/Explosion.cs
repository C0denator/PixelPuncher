using System.Collections;
using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Explosion that damages the player
    /// </summary>
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 1f;
        [SerializeField] private float _expandSpeed = 1f;
    
        private float _timeLeft = 0f;
        public int _damage = 1;
        private GameMaster _gameMaster;

        private void Awake()
        {
            _gameMaster = FindObjectOfType<GameMaster>();
            
            if (_gameMaster == null) Debug.LogError("GameMaster not found");
        }

        // Start is called before the first frame update
        void Start()
        {
            if (_gameMaster.GigachadMode)
            {
                _lifeTime *= 3;
                _expandSpeed *= 3;
            }
            
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

        private void OnEnable()
        {
            _timeLeft = _lifeTime;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //look if the player has a health component
                Health.Health playerHealth = other.gameObject.GetComponent<Health.Health>();
                
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
