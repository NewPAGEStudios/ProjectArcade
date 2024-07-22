using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformPassiveSkill : MonoBehaviour
{
    public Skill thisSkill;
    GameObject go;
    private GameObject player;

    public int consPosID;
    // Start is called before the first frame update
    void Start()
    {
        go = Instantiate(thisSkill.modelPrefab, gameObject.transform);
        go.transform.localScale = go.transform.localScale / 4;


        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        go.transform.forward = (transform.GetChild(0).position - player.transform.position).normalized;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerColl"))
        {
            System.Type script = System.Type.GetType(thisSkill.functionName + ",Assembly-CSharp");
            gameObject.AddComponent(script);
            //manuelHandling
            if (gameObject.TryGetComponent<getSpeed>(out getSpeed gs))
            {
                gs.consPosID = consPosID;
            }
        }
        else
        {
            return;
        }
    }
}
