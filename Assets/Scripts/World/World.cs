using UnityEngine;

namespace GameProg.World
{
    public class World : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        
        public Room CurrentRoom;
        
        //getters
        public GameObject Player => player;
    }
}
