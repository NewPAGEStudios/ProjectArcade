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
        Invoke("startMovement", 0.5f);
    }

    private void startMovement()
    {
        Debug.Log(rb.name);
        rb.AddForce(transform.forward * baseAmmo.bulletSpeed, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;

        if(hitTransform.CompareTag("PlayerColl"))
        {
            hitTransform.GetComponent<PController>().TakeDMG(baseAmmo.dmg, firedBy);
        }
        Destroy(gameObject);
    }
}
 