using System;
using System.Collections;
using UnityEngine;
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

    private Renderer enemyObjectRenderer;

    private Material mainMat;
    private MaterialPropertyBlock mainMPB;
    private Material damageMat;
    private Material deathMat;
    private Color baseColor;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        level = 0;

        enemyObjectRenderer = gameObject.transform.Find("Model").GetComponentInChildren<Renderer>();

        Debug.Log(enemyObjectRenderer.material.name);

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

    public void EnemyHealthUpdate(float amount)
    {
        currentHealth += amount;
        
        Debug.Log("CurrentHealth: "+ currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
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
        Debug.Log(mainMPB.GetColor("_BaseColor").r);

        float r = mainMPB.GetColor("_BaseColor").r;
        float g = mainMPB.GetColor("_BaseColor").g;
        float b = mainMPB.GetColor("_BaseColor").b;

        while (true)
        {
            r = Mathf.MoveTowards(r, baseColor.r*255, Time.deltaTime * 100f);
            g = Mathf.MoveTowards(g, baseColor.g*255, Time.deltaTime * 100f);
            b = Mathf.MoveTowards(b, baseColor.b*255, Time.deltaTime * 100f);

            mainMPB.SetColor("_BaseColor", new Color(r/255, g/255, b/255, 1));

            enemyObjectRenderer.SetPropertyBlock(mainMPB);

            yield return new WaitForEndOfFrame();
            if (r == baseColor.b * 255 && g == baseColor.b * 255 && b == baseColor.b * 255)
            {
                break;
            }
        }
        mainMPB = new MaterialPropertyBlock();
        mainMPB.SetColor("_BaseColor", baseColor);
        enemyObjectRenderer.SetPropertyBlock(mainMPB);
        enemyObjectRenderer.sharedMaterial = mainMat;
    }

    public void Die()
    {//id check
        StartCoroutine(EnemyDeathRoutine());
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
            valueOfStrentgh = Mathf.MoveTowards(valueOfStrentgh, -1, Time.deltaTime * 5f);

            enemyObjectRenderer.SetPropertyBlock(mainMPB);
            yield return new WaitForEndOfFrame();
            if (valueOfStrentgh <= -1)
            {
                break;
            }
        }
        gc.decreseEnemyCount(gameObject);
        Destroy(gameObject);
    }
}
