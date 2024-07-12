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

        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        player.GetComponent<PController>().SetSpeed(1.2f, 4f);

        gameObject.transform.parent.parent = gc.consumableSpawnPointParent.transform;
        Destroy(gameObject);


    }
    public void doFunctionWoutObject()
    {
        GameObject p = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().player;

        GameController gController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        p.GetComponent<PController>().SetSpeed(1.2f, 4f);



    }
}
