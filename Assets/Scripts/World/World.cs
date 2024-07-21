using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameProg.World
{
    public class World : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [FormerlySerializedAs("rooms")] [SerializeField] private List<Room> generatedRooms;
        public Room CurrentRoom;
        [Header("World Generation")]
        [SerializeField] private Room startRoom;
        [SerializeField] private List<Room> roomPrefabs;
        [SerializeField] private int numberOfRooms = 10;
        
        //getters
        public GameObject Player => player;

        private void Start()
        {
            //generate world
            GenerateWorld();
            
            Debug.Log("Generated "+(generatedRooms.Count-1)+" rooms");
            
            //set the starting room as current room
            foreach (var room in generatedRooms)
            {
                if (room.RoomType == RoomType.Start)
                {
                    CurrentRoom = room;
                }
            }
            
            //initialize rooms
            foreach (var room in generatedRooms)
            {
                room.InitializeRoom();
            }
            
            //initialize doors
            foreach (var room in generatedRooms)
            {
                room.InitializeDoors();
            }
            
            //show all starting rooms, hide the rest
            foreach (var room in generatedRooms)
            {
                if (room.RoomType == RoomType.Start)
                {
                    room.Show();
                    
                    //open doors in starting room
                    room.OpenDoors();
                }
                else
                {
                    room.Hide();
                }
            }
        }

        private void GenerateWorld()
        {
            int generatedRoomsCount = 0;
            
            //create start room
            Room start = Instantiate(startRoom, transform);
            this.generatedRooms.Add(start);
            start.transform.position = Vector3.zero;
            
            //move player in the middle of the start room
            player.transform.position = start.transform.position;
            
            
            //generate rooms until the desired number of rooms is reached
            while (generatedRoomsCount < numberOfRooms){
                
                //get all available doors
                List<Door> availableDoors = new List<Door>();
                foreach (var room in generatedRooms)
                {
                    foreach (var door in room.Doors)
                    {
                        availableDoors.Add(door);
                    }
                }
                
                bool roomGenerated = false;
                
                //for all available doors
                while (availableDoors.Count > 0) //todo: decrement availableDoors.Count
                {
                    Debug.Log("Trying to generate a new room. Available doors count: "+availableDoors.Count);
                    
                    //get a random door
                    Door randomDoor = availableDoors[UnityEngine.Random.Range(0, availableDoors.Count)];
                    
                    //get available room prefabs
                    List<Room> availableRoomPrefabs = new List<Room>();
                    foreach (var roomPrefab in roomPrefabs)
                    {
                        availableRoomPrefabs.Add(roomPrefab);
                    }
                    
                    //for all available room prefabs
                    while (availableRoomPrefabs.Count > 0) //todo: decrement availableRoomPrefabs.Count
                    {
                        //get a random room prefab
                        Room roomPrefab = availableRoomPrefabs[UnityEngine.Random.Range(0, availableRoomPrefabs.Count)];
                        
                        //instatiate a new room
                        Room newRoom = Instantiate(roomPrefab, transform, true);

                        Debug.Log("Trying with prefab: "+newRoom.name);
                        
                        //for all doors in the new room
                        for (int i = 0; i < newRoom.Doors.Count; i++)
                        {
                            Debug.Log("Checking door "+i);
                            //stuff
                        }
                        
                        //was a room generated?
                        if (roomGenerated)
                        {
                            break;
                        }
                        else
                        {
                            //remove this room from available room prefabs
                            availableRoomPrefabs.Remove(roomPrefab);
                            
                            //destroy the room
                            Destroy(newRoom.gameObject);
                        }
                        
                    }
                    
                    //was a room generated?
                    if (roomGenerated)
                    {
                        break;
                    }
                    else
                    {
                        //remove this door from available doors
                        availableDoors.Remove(randomDoor);
                    }
                    
                }
                
                //was a room generated?
                if (roomGenerated)
                {
                    generatedRoomsCount++;
                }
                else
                {
                    Debug.LogError("No room generated: No available doors left");
                    break;
                }
                
            }
        }
        
        private bool IsRoomCollidingWithAnyRoom(Room room)
        {
            foreach (var Room in generatedRooms)
            {
                if (room == Room) continue;
                
                if (room.IsCollidingWithRoom(Room))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
