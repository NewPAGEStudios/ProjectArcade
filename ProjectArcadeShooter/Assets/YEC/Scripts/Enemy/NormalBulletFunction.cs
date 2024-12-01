using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBulletFunction : MonoBehaviour
{
    public Ammo baseAmmo;
    Rigidbody rb;

    public GameObject firedBy;

    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();

        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Invoke("startMovement", 0.1f);
    }

    private void startMovement()
    {
        Debug.Log(rb.name);
        rb.AddForce(transform.forward * baseAmmo.bulletSpeed, ForceMode.VelocityChange);
    }
    // private void OnCollisionEnter(Collision collision)
    // {
    //     Transform hitTransform = collision.transform;
    //         // Debug.Log("temas etti");

    //     if(hitTransform.CompareTag("PlayerColl"))
    //     {
    //         Debug.Log("temas etti");
    //         hitTransform.parent.GetComponent<PController>().TakeDMG(baseAmmo.dmg, firedBy);
    //     }
    //     Destroy(gameObject);
    // }
    private void OnTriggerEnter(Collider collider)
    {
        Transform hitTransform = collider.transform;
        Debug.Log(hitTransform.name);
        if(hitTransform.CompareTag("PlayerColl"))
        {
            hitTransform.parent.GetComponent<PController>().TakeDMG(baseAmmo.dmg, firedBy);
            Destroy(gameObject);
        }
        else if(hitTransform.gameObject.layer == 14 || hitTransform.gameObject.layer == 11)
        {
            Destroy(gameObject);
        }
    }
}
 