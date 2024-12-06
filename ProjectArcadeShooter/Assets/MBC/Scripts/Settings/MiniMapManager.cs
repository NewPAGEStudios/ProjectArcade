using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public Camera outlineMiniMap;
    public bool smooth;
    private GameController gc;
    private Transform target;

    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        target = gc.player.transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (smooth)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.transform.position.x, 0, target.transform.position.z), Time.deltaTime * 12f);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.transform.position.x, 0, target.transform.position.z), Time.deltaTime * 200f);
        }
        transform.localEulerAngles = new Vector3(0, target.localEulerAngles.y, 0);
    }
    public void ChangeSize(float size)
    {
        outlineMiniMap.orthographicSize = size;
    }
    public void teleportMM()
    {
        transform.position = target.position;
    }
}
