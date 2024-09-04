using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonerMovemetn : MonoBehaviour
{
    GameController gc;

    public float speedOfRotation;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gc.pState == GameController.PlayState.inWaiting && gc.pState == GameController.PlayState.inWave)
        {
            transform.Rotate(new Vector3(0, 15, 0) * speedOfRotation * Time.deltaTime);
        }
    }
}
