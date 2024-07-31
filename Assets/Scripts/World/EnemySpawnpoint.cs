#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace World
{
    /// <summary>
    /// Component for Spawnpoint Prefab. Holds data for the enemy prefab and the wave it should spawn at.
    /// </summary>
    public class EnemySpawnpoint : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] [Range(0,2)] private int spawnAtWave = 0;
        
        //getter
        public int SpawnAtWave => spawnAtWave;
        public GameObject EnemyPrefab => enemyPrefab;
        
        private static bool _InfoShown = false;
        
        // Start is called before the first frame update
        private void Start()
        {

            if (!_InfoShown)
            {
                Debug.LogWarning("Uses Unity Editor - Delete Gizmos before build");
                _InfoShown = true;
            }
            
            if(enemyPrefab == null) Debug.LogWarning("Enemy prefab not set in spawnpoint "+name);
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //draw the name of the prefab over the spawnpoint
            if (enemyPrefab != null)
            {
                //draw a red box at the spawnpoint
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, Vector3.one);
                
                Handles.color = Color.red;
                Handles.Label(transform.position - new Vector3(0.5f,-0.25f,0f), enemyPrefab.name);
                Handles.Label(transform.position - new Vector3(0.5f,+0.25f,0f), spawnAtWave.ToString());
            }
            else
            {
                //draw a pink box at the spawnpoint
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(transform.position, Vector3.one);
                
                Handles.color = Color.magenta;
                Handles.Label(transform.position - new Vector3(0.5f,0f,0f), "Null");
            }
        }
        #endif
    }
}
