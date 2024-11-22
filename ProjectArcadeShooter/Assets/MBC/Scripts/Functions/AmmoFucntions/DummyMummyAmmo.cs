using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DummyMummyAmmo : MonoBehaviour
{
    public Ammo ammo;
    public string targetTag;
    void Start()
    {
        Instantiate(ammo.modelGO, gameObject.transform);
        gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        gameObject.transform.parent = null;

        GetComponent<Rigidbody>().AddForce(transform.forward * ammo.bulletSpeed, ForceMode.Impulse);
        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if(targetTag == "Player")
        {
            if (collision.transform.CompareTag(targetTag))
            {
                Debug.Log("Bomba");
                collision.transform.GetComponent<PController>().TakeDMG(ammo.dmg,gameObject);
            }
        }
        else if(targetTag == "Enemy" || targetTag == "Boss")
        {
            if (collision.transform.CompareTag("Enemy"))
            {

            }
            else if (collision.transform.parent.CompareTag("Boss"))
            {

            }
        }
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Destroy(gameObject);
    }
}
