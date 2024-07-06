using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getSpeed : MonoBehaviour
{
    private GameObject player;
    GameController gc;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().player;

        player.GetComponent<PController>().SetSpeed(1.2f, 4f);

        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        gameObject.transform.parent.parent = gc.spawnPointParent.transform;
        Destroy(gameObject);


    }
}
