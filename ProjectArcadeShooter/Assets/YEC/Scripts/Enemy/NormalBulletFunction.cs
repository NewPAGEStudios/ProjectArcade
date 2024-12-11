using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBulletFunction : MonoBehaviour
{
    public Ammo baseAmmo;
    Rigidbody rb;

    private GameObject trail;
    public TrailType trailType;

    public GameObject firedBy;
    private GameController gc;

    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        rb = gameObject.AddComponent<Rigidbody>();

        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Invoke("startMovement", 0.1f);

        trailType = baseAmmo.trail;
        trail = new();
        trail.transform.parent = transform;
        trail.transform.localPosition = Vector3.zero;

        trail.AddComponent<TrailRenderer>();
        trail.GetComponent<TrailRenderer>().material = trailType.TrailMaterail;
        trail.GetComponent<TrailRenderer>().colorGradient = trailType.gradient;
        trail.GetComponent<TrailRenderer>().widthCurve = trailType.curve;
        trail.GetComponent<TrailRenderer>().time = trailType.Time;

        Destroy(gameObject, 5f);
    }

    private void startMovement()
    {
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
            trail.transform.parent = null;
            gc.DestroyOnTime(trail.gameObject, trail.GetComponent<TrailRenderer>().time * 5);
            Destroy(gameObject);
        }
        else if(hitTransform.gameObject.layer == 14 || hitTransform.gameObject.layer == 11)
        {
            trail.transform.parent = null;
            gc.DestroyOnTime(trail.gameObject, trail.GetComponent<TrailRenderer>().time * 1.5f);
            Destroy(gameObject);
        }
    }
}
 