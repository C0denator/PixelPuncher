using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace World
{
    /// <summary>
    /// Component to control combat in a room. Finds all spawnpoints in the room and spawns enemies according to the wave they are assigned to.
    /// </summary>
    [RequireComponent(typeof(Room))]
    public class CombatController : MonoBehaviour
    {
        [SerializeField] private Transform spawnpointParent;
        [SerializeField] private Transform enemyParent;
        [FormerlySerializedAs("waves")] [SerializeField] private int wavesForThisRoom = 1;
        [SerializeField] private Wave[] waves;
        private int _currentWave = 0;

        private Room _room;
        private List<GameObject> currentEnemies = new List<GameObject>();

        public void Init()
        {
            //get room reference
            _room = GetComponent<Room>();
            if(_room == null) Debug.LogError("Room component not found");
            
            //subscribe to room events
            _room.OnRoomEnter += CombatStart;
            
            //find all spawnpoints in children
            Transform[] possibleSpawnpoints = spawnpointParent.GetComponentsInChildren<Transform>();
            
            List<Transform> spawnpointList = new List<Transform>();
            
            for(int i=0; i<possibleSpawnpoints.Length; i++)
            {
                if (possibleSpawnpoints[i].CompareTag("Spawnpoint"))
                {
                    spawnpointList.Add(possibleSpawnpoints[i]);
                }
            }

            if (spawnpointList.Count == 0)
            {
                Debug.LogWarning("No spawnpoints found in room "+_room.name);
                return;
            }
            
            //initialize waves
            waves = new Wave[wavesForThisRoom+1];

            for (int i = 0; i < waves.Length; i++)
            {
                waves[i].spawnpoints = new List<EnemySpawnpoint>();
            }
            
            //assign each spawnpoint to its wave
            for (int i = 0; i < spawnpointList.Count; i++)
            {
                //get script component
                EnemySpawnpoint spawnpoint = spawnpointList[i].gameObject.GetComponent<EnemySpawnpoint>();

                if (spawnpoint == null)
                {
                    Debug.LogError("Spawnpoint " + spawnpointList[i].name + " does not have an EnemySpawnpoint component");
                    return;
                }
                
                //check if index is out of bounds
                if (spawnpoint.SpawnAtWave > wavesForThisRoom)
                {
                    Debug.LogError("Spawnpoint " + spawnpointList[i].name + " has an invalid wave index");
                    return;
                }
                
                //assign to wave
                waves[spawnpoint.SpawnAtWave].spawnpoints.Add(spawnpoint);
            }
        }

        private void CombatStart()
        {
            //was the room visited before?
            if (_room.WasVisited)
            {
                return;
            }
            
            //close all doors
            _room.LockDoors();
            
            //spawn first wave
            SpawnWave();
        }
        
        private void CombatFinish()
        {
            //open all doors
            _room.OpenDoors();
            
            //invoke room cleared event
            _room.OnRoomCleared?.Invoke();
            
            //unsubscribe from room events
            _room.OnRoomEnter -= CombatStart;
        }
        
        private void SpawnWave()
        {
            //spawn enemies
            for (int i = 0; i < waves[_currentWave].spawnpoints.Count; i++)
            {
                GameObject enemy = Instantiate(waves[_currentWave].spawnpoints[i].EnemyPrefab,
                    waves[_currentWave].spawnpoints[i].transform.position, Quaternion.identity);
                enemy.transform.SetParent(enemyParent);
                
                //subscribe to enemy death event
                enemy.GetComponent<Health.Health>().OnDeath += HandleOnEnemyDeath;
                
                currentEnemies.Add(enemy);
            }
        }
        
        private void HandleOnEnemyDeath(GameObject deadEnemy)
        {
            //unsubscribe from enemy death event
            deadEnemy.GetComponent<Health.Health>().OnDeath -= HandleOnEnemyDeath;
            
            //remove enemy from list
            currentEnemies.Remove(deadEnemy);
            
            //destroy enemy
            Destroy(deadEnemy);
            
            //check if all enemies are dead
            if (currentEnemies.Count == 0)
            {
                //check if there are more waves
                if (_currentWave < wavesForThisRoom)
                {
                    _currentWave++;
                    SpawnWave();
                }
                else
                {
                    CombatFinish();
                }
            }
        }

        private void OnDisable()
        {
            //unsubscribe from room events
            if (_room != null)
            {
                _room.OnRoomEnter -= CombatStart;
            }
        }
    }
    
    [Serializable] public struct Wave
    {
        public List<EnemySpawnpoint> spawnpoints;
    }
}
