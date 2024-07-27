using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GameObject bossRoomPrefab;
        [SerializeField] private List<Room> roomPrefabs;
        [SerializeField] private int numberOfRooms = 10;
        [SerializeField] private bool debugMode = false;
        
        //getters
        public GameObject Player => player;
        public List<Room> GeneratedRooms => generatedRooms;
        
        public event Action OnWorldGenerated;

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
                        if (!door.WasUsedInGeneration)
                        {
                            availableDoors.Add(door); 
                        }
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
                                
                                randomDoor.WasUsedInGeneration = true;
                                newRoom.Doors[i].WasUsedInGeneration = true;
                                
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
            
            //generate boss room
            GameObject bossRoomGameObject = Instantiate(bossRoomPrefab, transform);
            
            //wait for the physics system to update
            yield return new WaitForFixedUpdate();
            
            Room bossRoom = bossRoomGameObject.GetComponent<Room>();
            
            //set viable position for boss room
            //get all available doors
            List<Door> availableDoorsForBoss = new List<Door>();
            foreach (var room in generatedRooms)
            {
                foreach (var door in room.Doors)
                {
                    if (!door.WasUsedInGeneration)
                    {
                        availableDoorsForBoss.Add(door); 
                    }
                }
            }
            
            bool bossRoomGenerated = false;

            while (availableDoorsForBoss.Count > 0)
            {

                //get a random door
                Door randomDoorForBoss = availableDoorsForBoss[UnityEngine.Random.Range(0, availableDoorsForBoss.Count)];
                
                //try all doors in the boss room
                for (int i = 0; i < bossRoom.Doors.Count(); i++)
                {
                    //move the boss room, so that the door is at the same position as the random door
                    bossRoomGameObject.transform.position = Vector3.zero;
                    
                    Vector3 offset = randomDoorForBoss.transform.position - bossRoom.Doors[i].transform.position;
                    
                    bossRoomGameObject.transform.position += offset;
                    
                    //wait for the physics system to update
                    yield return new WaitForFixedUpdate();
                    
                    //check if the boss room is colliding with any other room
                    if (!IsRoomCollidingWithAnyRoom(bossRoom))
                    {
                        //found valid room
                        bossRoomGenerated = true;
                        
                        randomDoorForBoss.WasUsedInGeneration = true;
                        bossRoom.Doors[i].WasUsedInGeneration = true;
                        
                        //set sorting order
                        bossRoom.wallsRenderer.sortingOrder = 100 - generatedRoomsCount;
                        
                        //add the boss room to the list of generated rooms
                        generatedRooms.Add(bossRoom);
                        
                        Debug.Log("Boss room generated!");
                        
                        break;
                    }
                }
                
                //was a room generated?
                if (bossRoomGenerated)
                {
                    break;
                }
                else
                {
                    //remove this door from available doors
                    availableDoorsForBoss.Remove(randomDoorForBoss);
                }
            }
            
            //was a room generated?
            if (!bossRoomGenerated)
            {
                Debug.LogError("No boss room generated: No available doors left");
            }
            
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
            
            //destroy doors that are marked for deletion
            foreach (var room in generatedRooms)
            {
                for (int i=0; i<room.Doors.Count; i++)
                {
                    if (room.Doors[i].MarkedForDeletion)
                    {
                        room.Doors[i].Delete();
                        i--;
                    }
                }
            }
            
            //wait for destroy() to finish
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
            
            //destroy tiles that are colliding with doors
            foreach (var room in generatedRooms)
            {
                Debug.Log("Checking room "+room.name+" for overlapping tiles");
                foreach (var door in room.Doors)
                {
                    Debug.Log("Checking door "+door.name+" for overlapping tiles");
                    door.DeleteOverlappingTiles();
                    Debug.Log("Door "+door.name+" checked for overlapping tiles");
                }
            }
            
            //wait for destroy() to finish
            yield return new WaitForEndOfFrame();

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
                    if (!debugMode)
                    {
                        room.Hide();
                    }
                    
                }
            }
            
            /*//spawn the player in the starting room
            player = Instantiate(playerPrefab, startRoom.transform.position, Quaternion.identity);*/
            
            //fire event
            OnWorldGenerated?.Invoke();
            Debug.Log("World generation finished");
            
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
