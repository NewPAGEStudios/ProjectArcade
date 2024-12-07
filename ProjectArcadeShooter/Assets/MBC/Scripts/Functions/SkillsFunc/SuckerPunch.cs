using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckerPunch : MonoBehaviour
{
    private GameObject player;
    public void doFunctionWoutObject()
    {
        player = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().player;

        player.GetComponent<WeaponManager>().startPowerfullAttack();

        SuckerPunch sp = GetComponent<SuckerPunch>();
        Destroy(sp);
    }
}
