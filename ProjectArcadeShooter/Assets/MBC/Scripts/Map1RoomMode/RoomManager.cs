using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;



// Build and update a localized navmesh from the sources marked by NavMeshSourceTag
[DefaultExecutionOrder(-102)]
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


    //// The center of the build
    //public Transform m_Tracked;

    //// The size of the build bounds
    //public Vector3 m_Size = new Vector3(80.0f, 20.0f, 80.0f);

    //NavMeshData m_NavMesh;
    //AsyncOperation m_Operation;
    //NavMeshDataInstance m_Instance;
    //List<NavMeshBuildSource> m_Sources = new();

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
        Debug.Log("bomba");
//        UpdateNavMesh();
        navMeshFly.BuildNavMesh();
//        navMeshWalk.BuildNavMesh();
    }
//    IEnumerator Starti()
//    {
//        while (true)
//        {
//            UpdateNavMesh(true);
//            yield return m_Operation;
//        }
//    }

//    void OnEnable()
//    {
//        // Construct and add navmesh
//        m_NavMesh = new NavMeshData();
//        m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
//        if (m_Tracked == null)
//            m_Tracked = transform;
//        UpdateNavMesh(false);
//    }

//    void OnDisable()
//    {
//        // Unload navmesh and clear handle
//        m_Instance.Remove();
//    }
//    void UpdateNavMesh(bool asyncUpdate = false)
//    {
////        Debug.Log("ýkasldþasdþsa");
//        NavMeshSourceTag.Collect(ref m_Sources);
//        var defaultBuildSettings = NavMesh.GetSettingsByID(agenjtFindClass.GetAgentTypeIDbyName("EnemyWalker"));
//        var bounds = QuantizedBounds();

//        if (asyncUpdate)
//        {
//            m_Operation = NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
//            StartCoroutine(Starti());
//        }
//        else
//            NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_Sources, bounds);

//    }
//    static Vector3 Quantize(Vector3 v, Vector3 quant)
//    {
//        float x = quant.x * Mathf.Floor(v.x / quant.x);
//        float y = quant.y * Mathf.Floor(v.y / quant.y);
//        float z = quant.z * Mathf.Floor(v.z / quant.z);
//        return new Vector3(x, y, z);
//    }

//    Bounds QuantizedBounds()
//    {
//        // Quantize the bounds to update only when theres a 10% change in size
//        var center = m_Tracked ? m_Tracked.position : transform.position;
//        return new Bounds(Quantize(center, 0.1f * m_Size), m_Size);
//    }
//    void OnDrawGizmosSelected()
//    {
//        if (m_NavMesh)
//        {
//            Gizmos.color = Color.green;
//            Gizmos.DrawWireCube(m_NavMesh.sourceBounds.center, m_NavMesh.sourceBounds.size);
//        }

//        Gizmos.color = Color.yellow;
//        var bounds = QuantizedBounds();
//        Gizmos.DrawWireCube(bounds.center, bounds.size);

//        Gizmos.color = Color.green;
//        var center = m_Tracked ? m_Tracked.position : transform.position;
//        Gizmos.DrawWireCube(center, m_Size);
//    }
}
