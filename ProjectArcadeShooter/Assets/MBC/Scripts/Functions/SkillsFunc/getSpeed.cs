using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getSpeed : MonoBehaviour
{
    private GameObject player;

    public void doFunctionWoutObject()
    {
        player = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().player;

        GameController gController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        player.GetComponent<PController>().SetSpeed(1.2f, 4f);

        getSpeed gs = GetComponent<getSpeed>();
        Destroy(gs);
    }
}
