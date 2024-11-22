using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Room : MonoBehaviour
{
    public List<GameObject> killZones = new();
    public List<GameObject> Emergency_Lights = new();
    private RoomManager roomManager;
    public bool roomStatus;
    private int openKZ;
    private int closeKZ;
    private void Awake()
    {
        roomManager = transform.parent.GetComponent<RoomManager>();
    }
    public void CloseRoom()
    {
        foreach (GameObject killzone in killZones)
        {
            killzone.GetComponent<Renderer>().enabled = true;
            killzone.GetComponent<KillZone>().work = true;
            killzone.GetComponent<Collider>().enabled = true;
            foreach (GameObject door in killzone.GetComponent<KillZone>().doors)
            {
                StartCoroutine(startKZone(door));
                closeKZ++;
            }
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
                Debug.Log(killzone.name);
            }
            if (cont)
            {
                continue;
            }
            killzone.GetComponent<Renderer>().enabled = false;
            killzone.GetComponent<KillZone>().work = false;
            killzone.GetComponent<Collider>().enabled = false;
            foreach (GameObject door in killzone.GetComponent<KillZone>().doors)
            {
                StartCoroutine(closeKZone(door));
                openKZ++;
            }
        }
    }
    public void ForceToOpen()
    {
        foreach (GameObject killzone in killZones)
        {
            killzone.GetComponent<Renderer>().enabled = false;
            killzone.GetComponent<KillZone>().work = false;
            killzone.GetComponent<Collider>().enabled = false;
            foreach (GameObject door in killzone.GetComponent<KillZone>().doors)
            {
                StartCoroutine(closeKZoneFT(door));
            }
        }
    }
    public void EmergencyLighton()
    {
        foreach(GameObject kz in killZones)
        {
            kz.GetComponent<KillZone>().alertSound = true;
            StartCoroutine(kzReady(kz));
        }
        
    }
    public void EmergencyLightoff()
    {

    }
    IEnumerator startKZone(GameObject door)
    {
        while (true)
        {
            door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, door.GetComponent<Door>().closedPos, Time.deltaTime * 2.5f);
            yield return null;
            if (door.transform.localPosition == door.GetComponent<Door>().closedPos)
            {
                break;
            }
        }
        closeKZ--;
        if (closeKZ <= 0)
        {
            roomManager.navme();
        }
    }

    IEnumerator closeKZone(GameObject door)
    {
        door.GetComponent<AudioSource>().Play();
        while (true)
        {
            door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, door.GetComponent<Door>().openedPos, Time.deltaTime * 2.5f);
            yield return null;
            if (door.transform.localPosition == door.GetComponent<Door>().openedPos)
            {
                break;
            }
        }
        openKZ--;
        if(openKZ <= 0)
        {
            roomManager.navme();
        }
    }
    IEnumerator closeKZoneFT(GameObject door)
    {
        while (true)
        {
            door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, door.GetComponent<Door>().openedPos, Time.deltaTime * 2.5f);
            yield return null;
            if (door.transform.localPosition == door.GetComponent<Door>().openedPos)
            {
                break;
            }
        }

    }
    IEnumerator kzReady(GameObject kz)
    {
        kz.GetComponent<Volume>().enabled = true;
        kz.transform.GetChild(0).gameObject.SetActive(true);
        foreach(GameObject emergencyL in Emergency_Lights)
        {
            emergencyL.GetComponentInChildren<Light>().enabled = true;
        }

        yield return new WaitForSeconds(.6f);
        kz.GetComponent<Volume>().enabled = false;
        kz.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        kz.GetComponent<Volume>().enabled = true;
        kz.transform.GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(.6f);
        kz.GetComponent<Volume>().enabled = false;
        kz.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        kz.GetComponent<Volume>().enabled = true;
        kz.transform.GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(.6f);
        kz.GetComponent<Volume>().enabled = false;
        kz.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(.15f);
        kz.GetComponent<Volume>().enabled = true;
        kz.transform.GetChild(0).gameObject.SetActive(true);
        kz.GetComponent<KillZone>().alertSound = false;

        foreach (GameObject emergencyL in Emergency_Lights)
        {
            emergencyL.GetComponentInChildren<Light>().enabled = false;
        }
    }

}
