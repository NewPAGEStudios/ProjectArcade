using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> killZones = new();
    public List<GameObject> Emergency_Lights = new();
    private RoomManager roomManager;
    public bool roomStatus;
    private void Start()
    {
        roomManager = transform.parent.GetComponent<RoomManager>();
    }
    public void CloseRoom()
    {
        foreach (GameObject killzone in killZones)
        {
            killzone.SetActive(true);
        }
    }
    public void OpenRoom()
    {
        foreach (GameObject killzone in killZones)
        {
            bool cont = false;
            foreach (Room room in roomManager.clsoedRoomList)
            {
                if (room.killZones.Contains(killzone))
                {
                    cont = true;
                    break;
                }
            }
            if (cont)
            {
                continue;
            }
            killzone.SetActive(false);
        }
    }
    public void EmergencyLighton()
    {

    }
    public void EmergencyLightoff()
    {

    }
}
