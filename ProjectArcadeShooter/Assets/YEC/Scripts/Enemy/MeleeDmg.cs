using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDmg : MonoBehaviour
{
    private PController pController;
    [SerializeField]
    private GameObject enemyModel;
    private void Start()
    {
        enemyModel.transform.parent.GetComponent<Enemy>().meleeObj = gameObject;
        gameObject.GetComponent<Collider>().enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("PlayerColl"))
        {
            pController = other.transform.parent.GetComponent<PController>();
            pController.TakeDMG(20, enemyModel.transform.parent.gameObject);
        }
    }

}
