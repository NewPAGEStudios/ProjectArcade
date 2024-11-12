using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public GameObject[] doors;
    private bool work;
    public void startKZ()
    {
        work = true;

    }
    public void closeKZ()
    {
        work = false;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (work)
        {
            //check other tag or layer and start death of it //Player or Enemy
            Debug.Log(other.name + "'s death has been started");
        }
    }
    IEnumerator startKZone()
    {
        yield return null;
    }
    IEnumerator closeKZone()
    {
        yield return null;
    }
}

