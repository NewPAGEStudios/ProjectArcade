using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public GameObject targetlight;
    private List<GameObject> activatedObjects = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Ammo"))
        {
            activatedObjects.Add(other.gameObject);  
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ammo"))
        {
            activatedObjects.Remove(other.gameObject);
        }
    }
    private void Update()
    {
        if(activatedObjects.Count > 0)
        {
            targetlight.SetActive(true);    
        }
        else
        {
            targetlight.SetActive(false);
        }
    }
}
