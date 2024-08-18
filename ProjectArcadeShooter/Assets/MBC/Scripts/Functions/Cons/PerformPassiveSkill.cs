using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformPassiveSkill : MonoBehaviour
{
    public Skill thisSkill;
    GameObject go;
    private GameObject player;
    GameController gc;
    public int consPosID;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

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
                gs.doFunctionWoutObject();
            }
            else if(gameObject.TryGetComponent<extraJumpAdder>(out extraJumpAdder eja))
            {
                eja.doFunctionWoutObject();
            }
        }
        else
        {
            return;
        }

        if (gc.pState != GameController.PlayState.inBoss)
        {
            int id = gc.activeCons.IndexOf(consPosID);
            gc.activeCons.RemoveAt(id);
            gc.activeConsID.RemoveAt(id);
            gc.activeConsSkill.RemoveAt(id);
            gc.activeConsWeapID.RemoveAt(id);
        }

        gameObject.transform.parent.parent = gc.consumableSpawnPointParent.transform;
        Destroy(gameObject);

    }
}
