using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDmg : MonoBehaviour
{
    private PController pController;
    [SerializeField]
    private GameObject enemyModel;
    public bool attackAvaible = false;
    private void Start()
    {
        enemyModel.transform.parent.GetComponent<Enemy>().meleeObj = gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("PlayerColl"))
        {
            Debug.Log("Attack avaible: " + attackAvaible);
            if (!attackAvaible)
            {
                return;
            }
            attackAvaible = false;
            pController = other.transform.parent.GetComponent<PController>();
            pController.TakeDMG(enemyModel.transform.parent.GetComponent<Enemy>().e_type.attackDMG, enemyModel.transform.parent.gameObject);
        }
    }

}
