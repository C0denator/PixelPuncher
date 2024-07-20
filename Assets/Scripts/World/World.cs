using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameProg.World
{
    public class World : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private List<Room> rooms;
        
        public Room CurrentRoom;
        
        //getters
        public GameObject Player => player;

        private void Start()
        {
            //get rooms
            rooms = new List<Room>();
            foreach (Transform child in transform)
            {
                Room room = child.GetComponent<Room>();
                if (room != null)
                {
                    rooms.Add(room);
                }
            }
            
            //set the starting room as current room
            foreach (var room in rooms)
            {
                if (room.RoomType == RoomType.Start)
                {
                    CurrentRoom = room;
                }
            }
            
            //initialize rooms
            foreach (var room in rooms)
            {
                room.InitializeRoom();
            }
            
            //initialize doors
            foreach (var room in rooms)
            {
                room.InitializeDoors();
            }
            
            //show all starting rooms, hide the rest
            foreach (var room in rooms)
            {
                if (room.RoomType == RoomType.Start)
                {
                    room.Show();
                }
                else
                {
                    room.Hide();
                }
            }
        }
    }
}
