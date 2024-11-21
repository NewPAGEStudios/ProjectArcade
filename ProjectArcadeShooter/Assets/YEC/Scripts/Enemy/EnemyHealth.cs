using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public float level;
    public Text txtLevel;
    public Animator animator;

    private GameController gc;

    private Coroutine dmgtakenRoutine;
    private Coroutine stunRoutine;

    private Renderer enemyObjectRenderer;

    private Material mainMat;
    private MaterialPropertyBlock mainMPB;
    private Material damageMat;
    private Material deathMat;
    private Color baseColor;
    // Start is called before the first frame update
    private Coroutine deathRoutine;


    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        level = 0;

        enemyObjectRenderer = gameObject.transform.Find("Model").GetComponentInChildren<Renderer>();

        mainMat = gameObject.GetComponent<Enemy>().e_type.mainMat;
        if (gameObject.GetComponent<Enemy>().e_type.getDmgMat != null)
        {
            damageMat = gameObject.GetComponent<Enemy>().e_type.getDmgMat;
        }
        deathMat = gameObject.GetComponent<Enemy>().e_type.deathMat;

        enemyObjectRenderer.sharedMaterial = mainMat;

        mainMPB = new MaterialPropertyBlock();
        baseColor = mainMat.color;

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnemyHealthUpdate(float amount,GameObject damagedBy)
    {
        
        currentHealth += amount;
        
        Debug.Log("CurrentHealth: "+ currentHealth);

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

                    enemyObjectRenderer.sharedMaterial = mainMat;
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
                enemyObjectRenderer.SetPropertyBlock(mainMPB);

                enemyObjectRenderer.sharedMaterial = mainMat;
            }
            dmgtakenRoutine = StartCoroutine(EnemyDMGTakeRoutine());
        }
    }
    IEnumerator EnemyDMGTakeRoutine()
    {
        mainMPB = new MaterialPropertyBlock();
        enemyObjectRenderer.sharedMaterial = damageMat;
        mainMPB.SetColor("_BaseColor", new Color(188, 0, 0, 1));
        enemyObjectRenderer.SetPropertyBlock(mainMPB);

        float r = mainMPB.GetColor("_BaseColor").r;
        float g = mainMPB.GetColor("_BaseColor").g;
        float b = mainMPB.GetColor("_BaseColor").b;

        //while (true)
        //{
        //    r = Mathf.MoveTowards(r, baseColor.r*255, Time.deltaTime * 100f);
        //    g = Mathf.MoveTowards(g, baseColor.g*255, Time.deltaTime * 100f);
        //    b = Mathf.MoveTowards(b, baseColor.b*255, Time.deltaTime * 100f);

        //    mainMPB.SetColor("_BaseColor", new Color(r/255, g/255, b/255, 1));
        //    mainMPB.SetColor("_EmissionColor",)
        //    enemyObjectRenderer.SetPropertyBlock(mainMPB);

        //    yield return new WaitForEndOfFrame();
        //    if (r == baseColor.b * 255 && g == baseColor.b * 255 && b == baseColor.b * 255)
        //    {
        //        break;
        //    }
        //}
        yield return new WaitForSeconds(0.5f);
        mainMPB = new MaterialPropertyBlock();
        mainMPB.SetColor("_BaseColor", baseColor);
        enemyObjectRenderer.SetPropertyBlock(mainMPB);
        enemyObjectRenderer.sharedMaterial = mainMat;
    }

    public void Die()
    {//id check

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
        enemyObjectRenderer.sharedMaterial = deathMat;
        mainMPB.SetColor("_BaseColor", baseColor);
        float valueOfStrentgh = 6f;
        mainMPB.SetFloat("_NoiseStrength", valueOfStrentgh);
        enemyObjectRenderer.SetPropertyBlock(mainMPB);
        while (true)
        {
            mainMPB.SetFloat("_NoiseStrength", valueOfStrentgh);
            valueOfStrentgh = Mathf.MoveTowards(valueOfStrentgh, -1, Time.deltaTime * 12.5f);

            enemyObjectRenderer.SetPropertyBlock(mainMPB);
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
        enemyObjectRenderer.sharedMaterial = damageMat;
        mainMPB.SetColor("_BaseColor", Color.blue);
        enemyObjectRenderer.SetPropertyBlock(mainMPB);

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

            enemyObjectRenderer.SetPropertyBlock(mainMPB);


            duration -= Time.deltaTime;
            yield return null;
            if (duration <= 0) {  break; }
        }
        mainMPB = new MaterialPropertyBlock();
        mainMPB.SetColor("_BaseColor", baseColor);
        enemyObjectRenderer.SetPropertyBlock(mainMPB);
        enemyObjectRenderer.sharedMaterial = mainMat;
        GetComponent<Enemy>().StunEnd();
    }
}
