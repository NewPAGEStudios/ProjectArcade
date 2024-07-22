using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getSpeed : MonoBehaviour
{
    private GameObject player;
    GameController gc;

    public int consPosID;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().player;

        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        player.GetComponent<PController>().SetSpeed(1.2f, 4f);

        int id = gc.activeCons.IndexOf(consPosID);
        gc.activeCons.RemoveAt(id);
        gc.activeConsID.RemoveAt(id);
        gc.activeConsSkill.RemoveAt(id);
        gc.activeConsWeapID.RemoveAt(id);

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
