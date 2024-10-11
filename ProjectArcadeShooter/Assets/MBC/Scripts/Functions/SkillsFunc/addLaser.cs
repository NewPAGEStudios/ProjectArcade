using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addLaser : MonoBehaviour
{
    private GameObject player;

    public void doFunctionWoutObject()
    {
        player = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().player;

        GameController gController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        player.GetComponent<WeaponManager>().addLaser(20);

        addLaser al = GetComponent<addLaser>();
        Destroy(al);
    }
}
