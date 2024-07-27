using UnityEngine;

namespace GameProg.UI
{
    public class ClearCounter : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _counterText;
        
        private World.World _world;
        
        private int _clearedRooms = 0;
        
        void Awake()
        {
            _world = FindObjectOfType<World.World>();
            
            if(_world == null) Debug.LogError("World not found in ClearCounter");
            if(_counterText == null) Debug.LogError("CounterText not set in ClearCounter");
        }
        
        private void HandleOnWorldGenerated()
        {
            _counterText.text = "0/" + (_world.GeneratedRooms.Count - 2);
            
            //unsubscribe from the world generated event
            _world.OnWorldGenerated -= HandleOnWorldGenerated;
            
            //subscribe to the room cleared event
            for(int i=0; i<_world.GeneratedRooms.Count; i++)
            {
                _world.GeneratedRooms[i].OnRoomCleared += HandleOnRoomCleared;
            }
        }
        
        private void HandleOnRoomCleared()
        {
            Debug.Log("Room cleared");
            
            _clearedRooms++;
            _counterText.text = _clearedRooms + "/" + (_world.GeneratedRooms.Count - 2);
            ;
        }

        private void OnEnable()
        {
            //subscribe to the world generated event
            _world.OnWorldGenerated += HandleOnWorldGenerated;
        }
        
        private void OnDisable()
        {
            //unsubscribe from the world generated event
            _world.OnWorldGenerated -= HandleOnWorldGenerated;
            
            //unsubscribe from the room cleared event
            for(int i=0; i<_world.GeneratedRooms.Count; i++)
            {
                _world.GeneratedRooms[i].OnRoomCleared -= HandleOnRoomCleared;
            }
        }
    }
}
