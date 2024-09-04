using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManColl : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField]
    private GameObject enemyModel;
    // Start is called before the first frame update
    void Start()
    {
        enemy = enemyModel.transform.parent.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PlayerColl"))
        {
            other.transform.parent.GetComponent<PController>().TakeDMG(enemy.e_type.attackDMG, enemyModel.transform.parent.gameObject);
            Invoke("patlatYeah", Time.deltaTime * 2);
        }
    }
    private void patlatYeah()
    {
        enemy.ehp.Die();
    }
}
