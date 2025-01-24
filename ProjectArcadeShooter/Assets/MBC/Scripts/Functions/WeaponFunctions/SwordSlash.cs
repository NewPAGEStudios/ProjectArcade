using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    public Animator handAnimator;
    public GameObject slashEffect;
    public WeaponManager weaponManager;
    public GameObject slashAttack;

    public Gradient col;
    public Material mat;
    public AnimationCurve curve;

    private bool first = true;
    private bool lineEffect = false;

    Coroutine rouineHolder;

    private GameObject rayHitOBJ;
    private LineRenderer lineRenderer;
    private void Start()
    {
        slashEffect.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (lineEffect)
        {
            Ray ray = new Ray();
            ray.direction = slashAttack.transform.up;
            ray.origin = slashAttack.transform.position;

            Debug.Log("open");
            Debug.DrawRay(ray.origin, ray.direction * 3.5f, Color.black,10);
            if (Physics.Raycast(ray,out RaycastHit hit, 3.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("hit");
                if (!rayHitOBJ == hit.transform.gameObject)
                {
                    GameObject ga = new();
                    lineRenderer = ga.AddComponent<LineRenderer>();
                    rayHitOBJ = hit.transform.gameObject;

                    lineRenderer.colorGradient = col;
                    lineRenderer.material = mat;

                    lineRenderer.positionCount = 1;
                    lineRenderer.SetPosition(0,hit.point);
                    lineRenderer.transform.parent = rayHitOBJ.transform;
                    Destroy(ga, 3);
                }
                else
                {
                    lineRenderer.positionCount += 1;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                }

            }
            else
            {
                Debug.Log("Doesn't hit");
            }
        }
    }
    public void Attack()
    {
        if (first)
        {
            lineEffect = true;
            first = false;
            firstAttack();
        }
        else
        {
            lineEffect = true;
            first = true;
            secondAttack();
        }
    }
    public void firstAttack()
    {
        slashEffect.SetActive(true);
        slashAttack.SetActive(true);

        handAnimator.SetBool("attackInterruption", false);
        rouineHolder = StartCoroutine(animationRoutine());
        
    }
    IEnumerator animationRoutine()
    {
        handAnimator.SetBool("fired", true);
        yield return null;
        handAnimator.SetBool("fired", false);

        while (true)
        {
            yield return null;
            if (handAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack0_weap2"))
            {
                break;
            }
        }
        yield return null;


        while (true)
        {
            yield return null;
            if (handAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack_weap_waitInput"))
            {
                break;
            }
        }
        rayHitOBJ = null;
        weaponManager.AttackEnded();
        slashEffect.SetActive(false);
        slashAttack.SetActive(false);
        lineEffect = false;
        float timer = 1f;
        while (true)
        {
            yield return null;

            timer -= Time.deltaTime;

            if(timer < 0)
            {
                handAnimator.SetBool("attackInterruption", true);
                break;
            }
        }
        yield return null;
        first = true;
        handAnimator.SetBool("attackInterruption", false);
    }
    public void secondAttack()
    {
        slashEffect.SetActive(true);
        slashAttack.SetActive(true);

        StopCoroutine(rouineHolder);
        StartCoroutine(animationRoutineSecond());
    }
    IEnumerator animationRoutineSecond()
    {
        handAnimator.SetBool("fired", true);
        yield return null;
        handAnimator.SetBool("fired", false);

        while (true)
        {
            yield return null;
            if (handAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle_weap2"))
            {
                break;
            }
        }
        lineEffect = false;
        rayHitOBJ= null;
        slashEffect.SetActive(false);
        slashAttack.GetComponentInChildren<MeleeFunctionss>().ClearGOS();
        slashAttack.SetActive(false);
        weaponManager.AttackEnded();
    }
}
