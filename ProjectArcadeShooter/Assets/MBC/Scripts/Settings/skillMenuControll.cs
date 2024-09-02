using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skillMenuControll : MonoBehaviour
{

    private GameController gc;
    [HideInInspector]
    public int currentSkill = -1;

    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void OnEnable()
    {
        currentSkill = -1;
    }
    public void skillChange(int id)
    {
        if (id != -1)
        {
            if (!transform.GetChild(id).GetComponent<Button>().interactable)
            {
                return;
            }
        }
        currentSkill = id;
    }
    public void pageChangeSkillID()
    {
        currentSkill = 0;
    }
}
