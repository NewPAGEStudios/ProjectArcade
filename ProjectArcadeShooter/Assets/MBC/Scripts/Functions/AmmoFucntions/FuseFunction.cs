using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FuseFunction : MonoBehaviour
{
    public Ammo baseAmmo;
    Rigidbody rb;

    public GameObject firedBy;
    public GameObject target;
    public GameObject simulatedPos;
    public float speedRatio;
    void Start()
    {
        gameObject.AddComponent<Rigidbody>();
        rb = gameObject.GetComponent<Rigidbody>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        if (firedBy.CompareTag("Enemy"))
        {
            Vector3 dir = target.transform.position - firedBy.transform.position;
            dir = Quaternion.Euler(-15, 0, 0) * dir;
            rb.AddForce(dir.normalized * baseAmmo.bulletSpeed * speedRatio, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(transform.forward * baseAmmo.bulletSpeed, ForceMode.Impulse);
        }


    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(gameObject.transform.position - rb.velocity);
        Ray ray = new()
        {
            origin = transform.position,
            direction = rb.velocity.normalized,
        };
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            simulatedPos.transform.position = hit.point;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (firedBy.CompareTag("Enemy"))
        {
            if (other.gameObject.CompareTag("EnemyColl"))
            {
                if (other.GetComponent<ColliderParenter>().targetOBJ.transform.parent.gameObject == firedBy)
                {
                    return;
                }
            }
            GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;
            if (Vector3.Distance(player.gameObject.transform.position, gameObject.transform.position) <= 7.5f)
            {
                player.GetComponent<PController>().TakeDMG(baseAmmo.dmg, gameObject);
            }
        }
        Destroy(gameObject);
        Destroy(simulatedPos);
    }
}
