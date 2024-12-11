using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stunInstanSkill : MonoBehaviour
{
    public Skill thisSkilll;
    GameController gc;

    List <GameObject> affectedGOs = new List <GameObject>();
    bool close = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<Rigidbody>();
        GetComponent<Rigidbody>().isKinematic = true;
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        StartCoroutine(endEffect());
    }
    IEnumerator endEffect()
    {
        yield return new WaitForSeconds(Time.deltaTime * 10);
        close = true;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(close)
        {
            return;
        }
        if (other.gameObject.CompareTag("EnemyColl"))
        {
            if (!affectedGOs.Contains(other.gameObject))
            {
                affectedGOs.Add(other.gameObject);
                other.GetComponent<ColliderParenter>().targetOBJ.transform.parent.GetComponent<Enemy>().Stun(1.5f);
            }
        }
    }
}
