using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class extraJumpAdder : MonoBehaviour
{
    private GameObject player;

    public void doFunctionWoutObject()
    {

        player = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().player;

        GameController gController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        player.GetComponent<PController>().addExtraJump();

        extraJumpAdder eja = GetComponent<extraJumpAdder>();
        Destroy(eja);
    }
}
