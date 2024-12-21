using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public GameObject[] doors;
    public bool work;
    public bool alertSound;
    private RoomManager roomManager;
    private GameObject minimap_OBJ;
    private GameObject collide_OBJ;
    private void Start()
    {
        minimap_OBJ = transform.GetChild(0).gameObject;
        collide_OBJ = transform.GetChild(1).gameObject;
        minimap_OBJ.SetActive(false);
        collide_OBJ.SetActive(false);
        roomManager = transform.parent.parent.GetChild(0).GetComponent<RoomManager>();
    }
    private void Update()
    {
        if(alertSound || work)
        {
            minimap_OBJ.SetActive(true);
            collide_OBJ.SetActive(true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (work)
        {
            if (other.CompareTag("PlayerColl"))
            {
                other.transform.parent.GetComponent<PController>().TakeDMG(1000, gameObject);
            }
            else if(other.CompareTag("EnemyColl"))
            {
                other.GetComponent<ColliderParenter>().targetOBJ.transform.parent.GetComponent<EnemyHealth>().Die();
            }
        }
        if (alertSound)
        {
            Debug.Log("kmb");
            if (other.CompareTag("PlayerColl"))
            {
                Debug.Log("kmbXXL");
                GetComponent<AudioSource>().volume = 1f;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (alertSound)
        {
            Debug.Log("kmb");
            if (other.CompareTag("PlayerColl"))
            {
                Debug.Log("kmbXXL");
                GetComponent<AudioSource>().volume = 1f;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (alertSound)
        {
            if (other.CompareTag("PlayerColl"))
            {
                GetComponent<AudioSource>().volume = 0f;
            }
        }
    }
}

