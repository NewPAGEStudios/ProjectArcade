using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseFunction : MonoBehaviour
{
    public Ammo baseAmmo;
    Rigidbody rb;

    public GameObject firedBy;
    public GameObject simulatedPos;
    void Start()
    {
        gameObject.AddComponent<Rigidbody>();
        rb = gameObject.GetComponent<Rigidbody>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.AddForce(transform.forward * baseAmmo.bulletSpeed, ForceMode.Impulse);

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
        if (Physics.Raycast(ray,out RaycastHit hit,100))
        {
            simulatedPos.transform.position = hit.point;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (firedBy.CompareTag("Enemy"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;
            if(Vector3.Distance(player.gameObject.transform.position,gameObject.transform.position) <= 7.5f)
            {
                player.GetComponent<PController>().TakeDMG(baseAmmo.dmg,gameObject);
            }
        }
        Destroy(gameObject);

    }
}
