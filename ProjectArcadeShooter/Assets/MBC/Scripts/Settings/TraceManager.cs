using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TraceManager : MonoBehaviour
{
    private float lifeTimer;
    private GameController gc;

    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        lifeTimer = gc.wallTraceLifeTime;
    }
    // Update is called once per frame
    void Update()
    {
        if (lifeTimer < 0)
        {
            Destroy(gameObject);
        }
        lifeTimer -= Time.deltaTime;
    }
}
