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
    private MaterialPropertyBlock damageMPB;
    private Material deathMat;
    private MaterialPropertyBlock deathMPB;
    private Color baseColor;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        level = 0;
        
        enemyObjectRenderer = gameObject.transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();

        mainMat = gameObject.GetComponent<Enemy>().e_type.mainMat;
        if (gameObject.GetComponent<Enemy>().e_type.getDmgMat != null)
        {
            damageMat = gameObject.GetComponent<Enemy>().e_type.getDmgMat;
        }
        deathMat = gameObject.GetComponent<Enemy>().e_type.deathMat;

        enemyObjectRenderer.sharedMaterial = mainMat;

        mainMPB = new MaterialPropertyBlock();
        baseColor = mainMat.color;
        baseColor = new Color(baseColor.r * 255, baseColor.g * 255, baseColor.b * 255);


        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnemyHealthUpdate(float amount)
    {
        currentHealth += amount;
        
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
                }
                dmgtakenRoutine = StartCoroutine(EnemyDMGTakeRoutine());
            }
        }
        else if(amount > 0)
        {
            if (dmgtakenRoutine != null)
            {
                StopCoroutine(dmgtakenRoutine);
            }
            dmgtakenRoutine = StartCoroutine(EnemyDMGTakeRoutine());
        }
    }
    IEnumerator EnemyDMGTakeRoutine()
    {
        damageMPB = new MaterialPropertyBlock();
        enemyObjectRenderer.sharedMaterial = damageMat;
        damageMPB.SetColor("_BaseColor", new Color(188, 0, 0, 1));
        enemyObjectRenderer.SetPropertyBlock(damageMPB);
        Debug.Log(baseColor.r);
        while (true)
        {
            float r = Mathf.MoveTowards(damageMPB.GetColor("_BaseColor").r, baseColor.r, Time.deltaTime * 2f);
            float g = Mathf.MoveTowards(damageMPB.GetColor("_BaseColor").g, baseColor.g, Time.deltaTime * 2f);
            float b = Mathf.MoveTowards(damageMPB.GetColor("_BaseColor").b, baseColor.b, Time.deltaTime * 2f);

            damageMPB.SetColor("_BaseColor", new Color(r, g, b, 1));

            enemyObjectRenderer.SetPropertyBlock(damageMPB);

            yield return new WaitForEndOfFrame();
            if (damageMPB.GetColor("_BaseColor").r == baseColor.r && damageMPB.GetColor("_BaseColor").g == baseColor.g && damageMPB.GetColor("_BaseColor").b == baseColor.b)
            {
                break;
            }
        }
        mainMPB.SetColor("_BaseColor", baseColor);
        enemyObjectRenderer.sharedMaterial = mainMat;
    }

    private void Die()
    {
        StartCoroutine(EnemyDeathRoutine());
    }
    IEnumerator EnemyDeathRoutine()
    {
        deathMPB = new MaterialPropertyBlock();
        enemyObjectRenderer.sharedMaterial = deathMat;
        deathMPB.SetColor("_BaseColor", baseColor);
        float valueOfStrentgh = 15f;
        deathMPB.SetFloat("_NoiseStrength", valueOfStrentgh);
        enemyObjectRenderer.SetPropertyBlock(deathMPB);
        while (true)
        {
            deathMPB.SetFloat("_NoiseStrength", valueOfStrentgh);
            valueOfStrentgh = Mathf.MoveTowards(valueOfStrentgh, 0, Time.deltaTime * 1f);

            enemyObjectRenderer.SetPropertyBlock(deathMPB);
            yield return new WaitForEndOfFrame();
            if (valueOfStrentgh <= -1)
            {
                break;
            }
        }
        gc.decreseEnemyCount();
        Destroy(gameObject);
    }
}
