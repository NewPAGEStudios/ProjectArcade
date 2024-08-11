using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    //birden fazla kullaným hakký olacak
    GameController gc;
    Collider col;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        col = GetComponent<Collider>();
    }
    private void Update()
    {
        if (gc.pState == GameController.PlayState.inWaiting)
        {
            col.enabled = true;
        }
        else
        {
            col.enabled = false;
        }
    }

}
