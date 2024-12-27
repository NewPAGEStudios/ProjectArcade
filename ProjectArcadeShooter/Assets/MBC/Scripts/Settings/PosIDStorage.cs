using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosIDStorage : MonoBehaviour
{
    public int posID;

    public bool bossCons;
    private void Awake()
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if(gameObject.transform.name == transform.parent.GetChild(i).name)
            {
                posID = i;
                break;
            }
        }
    }
}
