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
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        if (gc.pState == GameController.PlayState.inWaiting)
        {
            col.enabled = true;
            meshRenderer.enabled = true;
        }
        else
        {
            col.enabled = false;
            meshRenderer.enabled = false;
        }
    }

}
