using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameProg.World
{
    public class World : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] [CanBeNull] private GameObject player;
        [FormerlySerializedAs("rooms")] [SerializeField] private List<Room> generatedRooms;
        public Room CurrentRoom;
        [Header("World Generation")]
        [SerializeField] private Room startRoom;
        [SerializeField] private List<Room> roomPrefabs;
        [SerializeField] private int numberOfRooms = 10;
        
        //getters
        public GameObject Player => player;
        
        public static event Action OnWorldGenerated;

        private void Start()
        {
            //generate world
            StartCoroutine(GenerateWorld());
            
        }

        private IEnumerator GenerateWorld()
        {
            int generatedRoomsCount = 0;
            
            //create start room
            Room start = Instantiate(startRoom, transform);
            this.generatedRooms.Add(start);
            start.transform.position = Vector3.zero;
            
            //set order in layer to 100
            start.wallsRenderer.sortingOrder = 100;
            
            
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
                while (availableDoors.Count > 0)
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
                    while (availableRoomPrefabs.Count > 0)
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
                            
                            //move the new room, so that the door is at the same position as the random door
                            Vector3 offset = randomDoor.transform.position - newRoom.Doors[i].transform.position;
                            newRoom.transform.position += offset;
                            
                            //wait for the physics system to update
                            yield return new WaitForFixedUpdate();
                            
                            //check if the new room is colliding with any other room
                            if (!IsRoomCollidingWithAnyRoom(newRoom))
                            {
                                //found valid room
                                roomGenerated = true;
                                generatedRoomsCount++;
                                
                                //set sorting order
                                newRoom.wallsRenderer.sortingOrder = 100 - generatedRoomsCount;
                                
                                //add the new room to the list of generated rooms
                                generatedRooms.Add(newRoom);
                                
                                Debug.Log("Room generated!");
                                
                                break;
                            }
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
                if (!roomGenerated)
                {
                    Debug.LogError("No room generated: No available doors left");
                    break;
                    
                }
                
            }
            
            Debug.Log("Generated "+(generatedRooms.Count-1)+" rooms");

            //set the starting room as current room
            foreach (var room in generatedRooms)
            {
                if (room.RoomType == RoomType.Start)
                {
                    CurrentRoom = room;
                    break;
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
            
            //wait for Destroy() to finish
            yield return new WaitForEndOfFrame();
            
            //remove missing references in door lists
            foreach (var room in generatedRooms)
            {
                for (int i = 0; i < room.Doors.Count; i++)
                {
                    if (room.Doors[i] == null)
                    {
                        room.Doors.RemoveAt(i);
                        i--;
                    }
                }
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
            
            //spawn the player in the starting room
            player = Instantiate(playerPrefab, startRoom.transform.position, Quaternion.identity);
            
            //fire event
            OnWorldGenerated?.Invoke();
            
            yield return null;
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
