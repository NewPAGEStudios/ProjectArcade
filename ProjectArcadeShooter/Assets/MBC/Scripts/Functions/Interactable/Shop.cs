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
    Animator animator;

    Coroutine coroutine1;
    Coroutine coroutine2;

    public GameObject LightOBJ;
    public GameObject HeadPOS;

    public GameObject Screen;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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

    public void openScreen()
    {
        Screen.SetActive(true);
    }
    public void closeScreen()
    {
        Screen.SetActive(false);
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
        Screen.GetComponent<zzzz>().open();
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
        Screen.GetComponent<zzzz>().close();
    }


}