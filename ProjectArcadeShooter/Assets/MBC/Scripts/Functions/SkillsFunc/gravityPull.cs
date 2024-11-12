using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityPull : MonoBehaviour
{
    private GameObject player;

    public void doFunctionWoutObject()
    {
        GameController gController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        player = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().player;
        for(int i = 0; i < gController.moneyOBJ_Parent.transform.childCount; i++)
        {
            if (Vector3.Distance(player.transform.position, gController.moneyOBJ_Parent.transform.GetChild(i).transform.position) < 5f)
            {
                gController.moneyOBJ_Parent.transform.GetChild(i).GetComponent<GetMoney>().forceToPoint(player.transform);
            }
        }
        gravityPull gp = GetComponent<gravityPull>();
        Destroy(gp);
    }
}
