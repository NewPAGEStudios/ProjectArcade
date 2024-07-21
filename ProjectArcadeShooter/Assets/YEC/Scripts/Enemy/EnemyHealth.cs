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

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        level = 0;
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
    }

    private void Die()
    {
        gc.decreseEnemyCount();
        Destroy(gameObject);
    }
}
