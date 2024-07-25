using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameProg.Enemies
{
    [RequireComponent(typeof(Health.Health))]
    public class EnemyBaseClass : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<GameObject> debrisObjects;
        
        private Health.Health _health;
        private SpriteRenderer _spriteRenderer;
        private Transform _debrisParent;
        private Transform _player;
        
        // Start is called before the first frame update
        void Start()
        {
            //get references
            _health = GetComponent<Health.Health>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _player = GameObject.FindWithTag("Player").transform;
            
            //find debris parent: look at parent and search for a gameobject with the name "Debris"
            foreach (Transform child in transform.parent.parent)
            {
                if (child.name == "Debris")
                {
                    _debrisParent = child;
                    break;
                }
            }
            
            //error handling
            if (_health == null) Debug.LogError("Health component not found on " + gameObject.name);
            if (_spriteRenderer == null) Debug.LogError("SpriteRenderer component not found on " + gameObject.name);
            if (_debrisParent == null) Debug.LogError("Debris parent not found");
            if (_player == null) Debug.LogError("Player not found");
            
            //subscribe to death event
            _health.OnDeath += HandleOnDeath;
            
        }
        void FixedUpdate()
        {
            //flip sprite if player is on the left side
            if (_player.transform.position.x < transform.position.x)
            {
                _spriteRenderer.flipX = true;
            }
            else
            {
                _spriteRenderer.flipX = false;
            }
        }
        
        private void HandleOnDeath(GameObject obj)
        {
            //unsubscribe from death event
            _health.OnDeath -= HandleOnDeath;
            
            //spawn debris
            foreach (GameObject debrisObject in debrisObjects)
            {
                debrisObject.SetActive(true);
                
                //set parent
                debrisObject.transform.SetParent(_debrisParent);
                
                //get rigidbody
                Rigidbody2D debrisRb = debrisObject.GetComponent<Rigidbody2D>();

                if (debrisRb == null)
                {
                    Debug.LogError("Rigidbody2D component not found on debris object");
                }
                else
                {
                    //apply slight random force
                    debrisRb.AddForce(new Vector2(Random.Range(-2f,2f), Random.Range(-2f,2f)), ForceMode2D.Impulse);
                }
                
                //get sprite renderer
                SpriteRenderer debrisSpriteRenderer = debrisObject.GetComponent<SpriteRenderer>();
                
                debrisSpriteRenderer.flipX = _spriteRenderer.flipX;
                
                
            }
            
            //destroy enemy
            Destroy(gameObject);
        }

        private void OnDisable()
        {
            //unsubscribe from death event
            _health.OnDeath -= HandleOnDeath;
        }
    }
}
