using System;
using UnityEngine;

namespace GameProg.World
{
    [RequireComponent(typeof(Room))]
    public class CombatController : MonoBehaviour
    {
        [SerializeField] private int waves = 1;
        private int _currentWave = 0;

        private Room room;

        private void Start()
        {
            //get room reference
            room = GetComponent<Room>();
            if(room == null) Debug.LogError("Room component not found");
            
            //subscribe to room events
            room.OnRoomEnter += CombatStart;
        }

        private void CombatStart()
        {
            Debug.Log("Combat started");
            
            //close all doors
            room.LockDoors();
        }
    }
}
