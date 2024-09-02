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
    private void Update()
    {
        if (gc.pState == GameController.PlayState.inWaiting)
        {
            if (OTEventforOpen)
            {
                StartCoroutine(OpenRoutine());
                OTEventforOpen = false;
                OTEventforClose = true;
            }
        }
        else
        {
            if (OTEventforClose)
            {
                col.enabled = false;
                StartCoroutine(CloseRoutine());
                OTEventforOpen = true;
                OTEventforClose = false;
            }

        }
    }
    IEnumerator OpenRoutine()
    {
        animator.SetBool("close", false);
        yield return new WaitForEndOfFrame();
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
        yield return null;
    }
    IEnumerator CloseRoutine()
    {
        animator.SetBool("open", false);
        yield return new WaitForEndOfFrame();
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