using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameProg.UI
{
    public class PlayerHealthbar : MonoBehaviour
    {
        [SerializeField] private GameObject heartPrefab;
        [SerializeField] private float startX = 0;
        [SerializeField] private float startY = 0;
        [SerializeField] private float spacing = 0;
        
        [SerializeField] private Health.Health _playerHealth;
        private List<GameObject> _hearts = new List<GameObject>();
        [SerializeField] private Canvas _canvas;
        private World.World _world;
        // Start is called before the first frame update
        private void Awake()
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
            _world = FindObjectOfType<World.World>();
            
            //error handling
            if (_playerHealth == null) Debug.LogError("Health component not found on player");
            if (_canvas == null) Debug.LogError("Canvas not found");
            if (heartPrefab == null) Debug.LogError("Heart prefab not found");
            if (_world == null) Debug.LogError("World not found");

        }

        private void HandleOnHealthChanged()
        {
            UpdateHearts(_playerHealth.CurrentHealth);
        }

        private void UpdateHearts(int heartsNumber)
        {
            
            //disable hearts if they are not needed
            for (int i = 0; i < _hearts.Count; i++)
            {
                if (i <heartsNumber)
                {
                    _hearts[i].SetActive(true);
                }
                else
                {
                    _hearts[i].SetActive(false);
                }
            }
            
            //move hearts to the right position
            for (int i = 0; i < heartsNumber; i++)
            {
                _hearts[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(startX + i * spacing, startY);
            }
        }

        private void CreateHearts()
        {
            //create new hearts
            for (int i = 0; i < _playerHealth.MaxHealth; i++)
            {
                var heart = Instantiate(heartPrefab, _canvas.transform);
                _hearts.Add(heart);
            }
        }
        
        private void HandleOnWorldGenerated()
        {
            CreateHearts();
            UpdateHearts(_playerHealth.MaxHealth);
        }
        
        private void OnValidate()
        {
            if (heartPrefab)
            {
                UpdateHearts(_playerHealth.CurrentHealth);
            }
            
        }

        private void OnEnable()
        {
            //subscribe to the health changed event
            _playerHealth.OnHealthChanged += HandleOnHealthChanged;
            
            //subscribe to the world generated event
            _world.OnWorldGenerated += HandleOnWorldGenerated;
        }

        private void OnDisable()
        {
            //unsubscribe from the health changed event
            _playerHealth.OnHealthChanged -= HandleOnHealthChanged;
            
            //unsubscribe from the world generated event
            _world.OnWorldGenerated -= HandleOnWorldGenerated;
        }
    }
}
