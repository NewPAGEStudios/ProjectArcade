using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScripts : MonoBehaviour
{
    public Vector3 closedPos;
    public Vector3 opennedPos;

    public void open()
    {
        StartCoroutine(openRoutine());
    }
    IEnumerator openRoutine()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, opennedPos, Time.deltaTime * 20);
            yield return null;
            if(transform.position == opennedPos)
            {
                break;
            }
        }
    }
    public void close()
    {
        StartCoroutine(closeRoutine());
    }
    IEnumerator closeRoutine()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, closedPos, Time.deltaTime * 20);
            yield return null;
            if (transform.position == closedPos)
            {
                break;
            }
        }
    }
}
