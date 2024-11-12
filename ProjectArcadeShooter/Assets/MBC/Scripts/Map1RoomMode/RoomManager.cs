using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public NavMeshSurface navMeshWalk;
    public NavMeshSurface navMeshFly;
    [SerializeField] private Room[] rooms;
    [HideInInspector] public List<Room> roomList = new();
    [HideInInspector] public List<Room> clsoedRoomList = new();
    private void Start()
    {
        roomList = rooms.ToList<Room>();
    }
    public void startRoutineOfRoom(float roomCloseTime)
    {
        int i = Random.Range(0, roomList.Count);
        roomRoutine(rooms[i], roomCloseTime);
    }
    IEnumerator roomRoutine(Room room, float roomCloseTime)
    {
        room.EmergencyLighton();

        yield return new WaitForSeconds(3f);

        room.EmergencyLightoff();

        room.CloseRoom();

        roomList.Remove(room);
        clsoedRoomList.Add(room);

        room.roomStatus = false;
        navMeshWalk.BuildNavMesh();
        navMeshFly.BuildNavMesh();

        yield return new WaitForSeconds(roomCloseTime);

        room.OpenRoom();

        roomList.Add(room);
        clsoedRoomList.Add(room);

        room.roomStatus = true;
        navMeshWalk.BuildNavMesh();
        navMeshFly.BuildNavMesh();
    }

}
