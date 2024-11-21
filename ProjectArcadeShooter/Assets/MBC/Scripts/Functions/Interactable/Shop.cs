using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    //birden fazla kullaným hakký olacak
    GameController gc;
    public Collider col;

    public GameObject LightOBJ;
    public GameObject HeadPOS;
    public GameObject BodyPos;

    public GameObject Screen;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        col = GetComponent<Collider>();
    }
    public void open()
    {
        col.enabled = true;
        Screen.SetActive(true);
        gc.ShopScreenChange(1);
    }
    public void close()
    {
        col.enabled = false;
        Screen.SetActive(false);
    }
    public void workScreen()
    {
        Screen.GetComponent<zzzz>().open();
    }
    public void unWorkScreen()
    {
        Screen.GetComponent<zzzz>().close();
    }

}