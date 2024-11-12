using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetMoney : MonoBehaviour
{
    GameController gc;
    public float money;
    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerColl"))
        {
            gc.AddMainCurrency(money);
            Destroy(gameObject);
        }
    }
    public void forceToPoint(Transform point)
    {
        Vector3 dir = point.position - transform.position;
        GetComponent<Rigidbody>().AddForce(dir.normalized * 25f, ForceMode.Impulse);
    }
}
