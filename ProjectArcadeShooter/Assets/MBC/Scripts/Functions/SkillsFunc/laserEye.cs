using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserEye : MonoBehaviour
{
    private GameObject player;

    public void doFunctionWoutObject()
    {
        player = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().player;

        player.GetComponent<WeaponManager>().openEagleEye();

        laserEye le = GetComponent<laserEye>();
        Destroy(le);
    }
}
