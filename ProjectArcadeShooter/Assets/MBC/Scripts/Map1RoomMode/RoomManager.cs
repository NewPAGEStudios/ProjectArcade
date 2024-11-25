using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public NavMeshSurface navMeshWalk;
    public NavMeshSurface navMeshFly;
    [SerializeField] public Room[] rooms;
    public List<Room> roomList = new();
    public List<Room> clsoedRoomList = new();

    public Material KZ_Mat;
    public Material KZ_Mat_alert;

    public GameController gc;
    private void Start()
    {
        gc=GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        clsoedRoomList = rooms.ToList<Room>();
        OpenAll();
    }
    public void startRoutineOfRoom(float roomCloseTime,int i = -1)
    {
        if (i == -1)
        {
            i = Random.Range(0, roomList.Count);
        }
        StartCoroutine(roomRoutine(rooms[i], roomCloseTime));
    }
    IEnumerator roomRoutine(Room room, float roomCloseTime)
    {
        room.EmergencyLighton();


        float timer = 5f;
        while (timer>0)
        {
            yield return null;
            if (gc.state != GameController.GameState.inGame || gc.pState != GameController.PlayState.inWave)
            {
                continue;
            }
            timer -= Time.deltaTime;
        }

        room.EmergencyLightoff();

        roomList.Remove(room);
        clsoedRoomList.Add(room);

        room.CloseRoom();


        room.roomStatus = false;

        while(roomCloseTime > 0)
        {
            yield return null;
            if (gc.state != GameController.GameState.inGame || gc.pState != GameController.PlayState.inWave)
            {
                continue;
            }
            roomCloseTime -= Time.deltaTime;
        }


        roomList.Add(room);
        clsoedRoomList.Remove(room);

        room.OpenRoom();


        room.roomStatus = true;
    }
    public void OpenAll()
    {
        while (0 < clsoedRoomList.Count)
        {
            clsoedRoomList[0].ForceToOpen();

            roomList.Add(clsoedRoomList[0]);
            clsoedRoomList.Remove(clsoedRoomList[0]);
        }
    }
    public void CloseAll()
    {
        while (0 < roomList.Count)
        {
            roomList[0].CloseRoom();

            roomList.Add(clsoedRoomList[0]);
            clsoedRoomList.Remove(clsoedRoomList[0]);
        }
    }
    public void navme()
    {
        navMeshFly.BuildNavMesh();
        navMeshWalk.BuildNavMesh();
    }
}
