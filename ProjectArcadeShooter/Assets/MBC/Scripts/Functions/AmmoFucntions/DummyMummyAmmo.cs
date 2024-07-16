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
        StartCoroutine(startingFunc());
    }

    IEnumerator startingFunc()
    {
        gameObject.transform.localScale = Vector3.zero;
        while (gameObject.transform.localScale.z <= 1f)
        {
            gameObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f) * Time.deltaTime * 100;
            yield return new WaitForEndOfFrame();
        }
        gameObject.transform.localScale = Vector3.one / 10;
        GetComponent<Rigidbody>().AddForce(transform.forward * ammo.bulletSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(targetTag == "PlayerColl")
        {
            if (collision.transform.CompareTag(targetTag))
            {
                collision.transform.parent.GetComponent<PController>().TakeDMG(ammo.dmg,gameObject);
                Destroy(gameObject);
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
        else
        {
            Destroy(gameObject);
        }
    }
}
