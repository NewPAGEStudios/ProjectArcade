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
    bool OTEventforOpen;
    bool OTEventforClose;
    Animator animator;

    Coroutine coroutine1;
    Coroutine coroutine2;

    public GameObject LightOBJ;
    public GameObject HeadPOS;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        OTEventforOpen = true;
        OTEventforClose = true;
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        col = GetComponent<Collider>();
        animator.SetTrigger("Start");
    }
    public void open()
    {
        StartCoroutine(OpenRoutine());
    }
    public void close()
    {
        col.enabled = false;
        LightOBJ.SetActive(false);
        StartCoroutine(CloseRoutine());
    }
    IEnumerator OpenRoutine()
    {
        animator.SetBool("close", false);
        animator.SetBool("open", true);
        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("OpenEnd"))
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        col.enabled = true;
        LightOBJ.SetActive(true);
        yield return null;
    }
    IEnumerator CloseRoutine()
    {
        animator.SetBool("open", false);
        animator.SetBool("close", true);
        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("CloseEnd"))
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

}