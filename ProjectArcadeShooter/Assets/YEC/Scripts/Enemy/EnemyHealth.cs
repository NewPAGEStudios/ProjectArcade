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
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        level = 0;
        
        enemyObjectRenderer = gameObject.transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();

        mainMat = gameObject.GetComponent<Enemy>().e_type.mainMat;
        damageMat = gameObject.GetComponent<Enemy>().e_type.getDmgMat;
        deathMat = gameObject.GetComponent<Enemy>().e_type.deathMat;

        enemyObjectRenderer.sharedMaterial = mainMat;


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
        }
        else if(amount < 0)
        {
            if (dmgtakenRoutine != null)
            {
                StopCoroutine(dmgtakenRoutine);
            }
            dmgtakenRoutine = StartCoroutine(EnemyDMGTakeRoutine());
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

            

        yield return null;
    }

    private void Die()
    {
        gc.decreseEnemyCount();
        Destroy(gameObject);
    }
    IEnumerator EnemyDeathRoutine()
    {
        yield return null;
    }
}
