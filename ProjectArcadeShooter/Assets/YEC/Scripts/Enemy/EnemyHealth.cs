using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    private float currentHealthOverride;
    public float level;
    public Text txtLevel;
    public TextMeshPro tmpEnemyIndicator;
    public GameObject DMGTxtEnemyParent;
    public GameObject hpBar;
    public Animator animator;

    private GameController gc;

    private Coroutine dmgtakenRoutine;
    public Coroutine stunRoutine;

    private Renderer[] enemyObjectRenderer;

    private Material mainMat;
    private MaterialPropertyBlock mainMPB;
    private Material damageMat;
    private Material deathMat;
    private Color baseColor;
    // Start is called before the first frame update
    private Coroutine deathRoutine;

    private bool dieStarted=false;
    private List<ColliderParenter> colliderObjects = new();
    void Start()
    {
        colliderObjects = transform.GetComponentsInChildren<ColliderParenter>().ToList<ColliderParenter>();
        for (int i = 0; i < colliderObjects.Count; i++)
        {
            if(colliderObjects[i].TryGetComponent<Collider>(out Collider col))
            {

            }
            else
            {
                colliderObjects.RemoveAt(i);
            }
        }


        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        level = 0;

        enemyObjectRenderer = gameObject.transform.Find("Model").Find("TargetModel").GetComponentsInChildren<Renderer>();

        mainMat = gameObject.GetComponent<Enemy>().e_type.mainMat;
        if (gameObject.GetComponent<Enemy>().e_type.getDmgMat != null)
        {
            damageMat = gameObject.GetComponent<Enemy>().e_type.getDmgMat;
        }
        deathMat = gameObject.GetComponent<Enemy>().e_type.deathMat;

        //foreach (Renderer r in enemyObjectRenderer)
        //{
        //    r.sharedMaterial = mainMat;
        //}

        mainMPB = new MaterialPropertyBlock();
        baseColor = mainMat.color;

        currentHealth = maxHealth;
        currentHealthOverride = currentHealth;

        for (int i = 0; i < DMGTxtEnemyParent.transform.childCount; i++)
        {
            DMGTxtEnemyParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        tmpEnemyIndicator.transform.parent.LookAt(GetComponent<Enemy>().Player.transform);


        //if (currentHealth != 0)
        //{
        //    MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        //    currentHealthOverride = Mathf.MoveTowards(currentHealthOverride, currentHealth, Time.deltaTime * 30f);

        //    tmpEnemyIndicator.text = Mathf.Floor(currentHealthOverride).ToString();

        //    mpb.SetFloat("_alpha", currentHealthOverride / maxHealth);
        //    hpBar.GetComponent<Renderer>().SetPropertyBlock(mpb);

        //}
    }

    public void EnemyHealthUpdate(float amount,GameObject damagedBy)
    {
        
        currentHealth += amount;
        EnemyTxtDmgRoutine(amount.ToString());

        //Debug.Log("CurrentHealth: "+ currentHealth);
        tmpEnemyIndicator.GetComponent<TextMeshPro>().text = currentHealth.ToString();

        if (currentHealth <= 0)
        {
            Destroy(GetComponent<StateMachine>());
            if (GetComponent<Enemy>().Agent.enabled)
            {
                GetComponent<Enemy>().Agent.isStopped = true;
            }
            if (GetComponent<Enemy>().animator != null)
            {
                GetComponent<Enemy>().animator.SetBool("isWalking", false);
                Destroy(GetComponent<Enemy>().animator);
            }


            currentHealth = 0;
            gc.comboTimerReset();
            Die();
            return;
        }
        else if(amount < 0)
        {
            if (damageMat != null)
            {
                if (dmgtakenRoutine != null)
                {
                    StopCoroutine(dmgtakenRoutine);
                    /*
                                        mainMPB = new MaterialPropertyBlock();
                                        mainMPB.SetColor("_BaseColor", baseColor);
                                        enemyObjectRenderer.SetPropertyBlock(mainMPB);
                    */

                    foreach (Renderer r in enemyObjectRenderer)
                    {
                        r.sharedMaterial = mainMat;
                    }
                }
                dmgtakenRoutine = StartCoroutine(EnemyDMGTakeRoutine());
            }
        }
        else if(amount > 0)//Heal ?
        {
            if (dmgtakenRoutine != null)
            {
                StopCoroutine(dmgtakenRoutine);

                mainMPB = new MaterialPropertyBlock();
                mainMPB.SetColor("_BaseColor", baseColor);
                foreach (Renderer r in enemyObjectRenderer)
                {
                    r.SetPropertyBlock(mainMPB);
                }

                foreach (Renderer r in enemyObjectRenderer)
                {
                    r.sharedMaterial = mainMat;
                }
            }
            dmgtakenRoutine = StartCoroutine(EnemyDMGTakeRoutine());
        }
    }
    void EnemyTxtDmgRoutine(string amount)
    {
        GameObject selectedTXT = DMGTxtEnemyParent.transform.GetChild(0).gameObject;
        bool firstPartSelected = false;
        for (int i = 0; i < DMGTxtEnemyParent.transform.childCount; i++)
        {
            if (DMGTxtEnemyParent.transform.GetChild(i).GetComponent<dmgTxt>().open)
            {
                selectedTXT = DMGTxtEnemyParent.transform.GetChild(i).gameObject;
                firstPartSelected = true;
                break;
            }
        }
        if (!firstPartSelected)
        {
            selectedTXT = DMGTxtEnemyParent.transform.GetChild(0).gameObject;
            for (int i = 0; i < DMGTxtEnemyParent.transform.childCount - 1; i++)
            {
                if(DMGTxtEnemyParent.transform.GetChild(i).GetComponent<dmgTxt>().lifetimeTimer < DMGTxtEnemyParent.transform.GetChild(i + 1).GetComponent<dmgTxt>().lifetimeTimer)
                {
                    selectedTXT = DMGTxtEnemyParent.transform.GetChild(i + 1).gameObject;
                    break;
                }
            }
        }
        selectedTXT.GetComponent<dmgTxt>().startThis(amount);
    }
    IEnumerator EnemyDMGTakeRoutine()
    {
        mainMPB = new MaterialPropertyBlock();
        foreach (Renderer ren in enemyObjectRenderer)
        {
            ren.sharedMaterial = damageMat;
        }
        yield return new WaitForSeconds(0.5f);
        foreach (Renderer rende in enemyObjectRenderer)
        {
            rende.sharedMaterial = mainMat;
        }
    }

    public void Die()
    {//id check
        if (dieStarted)
        {
            return;
        }


        foreach (ColliderParenter go in colliderObjects)
        {
            go.GetComponent<Collider>().enabled = false;
        }



        GetComponent<Enemy>().eState = Enemy.enemyState.dead;

        for(int i = 0;i< GetComponent<Enemy>().e_type.moneyToDrop; i++)
        {
            GameObject mobj = Instantiate(gc.moneyOBJ,gc.moneyOBJ_Parent.transform);
            mobj.transform.position = gameObject.transform.position + new Vector3(0, 1, 0);
            mobj.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 90), UnityEngine.Random.Range(0, 90), UnityEngine.Random.Range(0, 90));
            mobj.GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.Impulse);
            mobj.GetComponent<GetMoney>().money = 5;
        }
        if(deathRoutine != null)
        {
            return;
        }
        deathRoutine = StartCoroutine(EnemyDeathRoutine());
    }
    IEnumerator EnemyDeathRoutine()
    {
        mainMPB = new MaterialPropertyBlock();
        foreach (Renderer rende in enemyObjectRenderer)
        {
            rende.sharedMaterial = deathMat;
        }
        mainMPB.SetColor("_BaseColor", baseColor);

        float valueOfStrentgh = 6f;

        mainMPB.SetFloat("_NoiseStrength", valueOfStrentgh);
        foreach (Renderer rende in enemyObjectRenderer)
        {
            rende.SetPropertyBlock(mainMPB);
        }

        while (true)
        {
            mainMPB.SetFloat("_NoiseStrength", valueOfStrentgh);
            valueOfStrentgh = Mathf.MoveTowards(valueOfStrentgh, -1, Time.deltaTime * 12.5f);

            foreach (Renderer rende in enemyObjectRenderer)
            {
                rende.SetPropertyBlock(mainMPB);
            }
            yield return null;
            if (valueOfStrentgh <= -1)
            {
                break;
            }
        }
        gc.decreseEnemyCount(gameObject);
        Destroy(gameObject);
    }
    public void stunEffect(float duration)
    {
        if (stunRoutine != null)
        {
            StopCoroutine(stunRoutine);
        }
        stunRoutine = StartCoroutine(stunEffectRoutine(duration));
    }
    IEnumerator stunEffectRoutine(float duration)
    {
        mainMPB = new MaterialPropertyBlock();
        foreach (Renderer rende in enemyObjectRenderer)
        {
            rende.sharedMaterial = damageMat;
            mainMPB.SetColor("_BaseColor", Color.blue);
            rende.SetPropertyBlock(mainMPB);
        }


        float vec = duration / Time.deltaTime;

        float r = mainMPB.GetColor("_BaseColor").r;
        float g = mainMPB.GetColor("_BaseColor").g;
        float b = mainMPB.GetColor("_BaseColor").b;

        float rtime = (r - baseColor.r * 255) / vec;
        float gtime = (g - baseColor.g * 255) / vec;
        float btime = (b - baseColor.b * 255) / vec;

        while (true)
        {
            r = Mathf.MoveTowards(r, baseColor.r * 255, rtime);
            g = Mathf.MoveTowards(g, baseColor.g * 255, gtime);
            b = Mathf.MoveTowards(b, baseColor.b * 255, btime);

            mainMPB.SetColor("_BaseColor", new Color(r / 255, g / 255, b / 255, 1));

            foreach (Renderer rende in enemyObjectRenderer)
            {
                rende.SetPropertyBlock(mainMPB);
            }


            duration -= Time.deltaTime;
            yield return null;
            if (duration <= 0) {  break; }
        }
        mainMPB = new MaterialPropertyBlock();
        mainMPB.SetColor("_BaseColor", baseColor);

        foreach (Renderer rende in enemyObjectRenderer)
        {
            rende.SetPropertyBlock(mainMPB);
            rende.sharedMaterial = mainMat;
        }


        stunRoutine = null;
        GetComponent<Enemy>().StunEnd();
    }

}
