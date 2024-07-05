using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getSpeed : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PController>().SetSpeed(1.2f, 4f);
    }
}
