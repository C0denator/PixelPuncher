using System.Collections.Generic;
using UnityEngine;

namespace GameProg.UI
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private GameObject heartPrefab;
        [SerializeField] private float startX = 0;
        [SerializeField] private float startY = 0;
        [SerializeField] private float spacing = 0;
        
        [SerializeField] private Health.Health _playerHealth;
        private List<GameObject> _hearts = new List<GameObject>();
        [SerializeField] private Canvas _canvas;
        
        // Start is called before the first frame update
        void Start()
        {
            //get references
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player not found");
                return;
            }
            
            _playerHealth = player.GetComponent<Health.Health>();
            _canvas = GetComponent<Canvas>();
            _hearts = new List<GameObject>();
                
            //error handling
            if (_playerHealth == null) Debug.LogError("Health component not found on player");
            if (_canvas == null) Debug.LogError("Canvas not found");
            
            //subscribe to the health changed event
            _playerHealth.OnHealthChanged += HandleOnHealthChanged;
            
            //create hearts
            UpdateHearts(_playerHealth.CurrentHealth);
        }
        
        private void HandleOnHealthChanged()
        {
            UpdateHearts(_playerHealth.CurrentHealth);
        }

        private void UpdateHearts(int numberOfHearts)
        {
            //delete old hearts
            if (_hearts.Count > 0)
            {
                for(int i = 0; i < _hearts.Count; i++)
                {
                    Destroy(_hearts[i]);
                }
            }
            
            
            //spawn hearts on the top left corner
            for (int i = 0; i < numberOfHearts; i++)
            {
                //create heart
                var heart = Instantiate(heartPrefab, _canvas.transform);
                
                //set position
                var rectTransform = heart.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(startX + i * spacing, startY);
                
                //add to list
                _hearts.Add(heart);
            }
        }

        private void OnValidate()
        {
            if (heartPrefab)
            {
                UpdateHearts(_playerHealth.MaxHealth);
            }
            
        }
    }
}
