using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skillMenuControll : MonoBehaviour
{

    private GameController gc;
    public int currentSkill = -1;

    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }


    public void skillChange(int id)
    {
        currentSkill = id;
    }
}
