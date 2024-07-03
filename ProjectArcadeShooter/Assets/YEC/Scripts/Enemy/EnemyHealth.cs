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
    private float deathTime;
    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        level = 0;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            deathTime += Time.deltaTime;
            if (deathTime > 5)
            {
                Destroy(gameObject);
                Debug.Log("süre doldu");
            }
        }
    }

    public void EnemyHealthUpdate(float amount)
    {
        if (isDead) return; // Eğer düşman zaten ölmüşse, herhangi bir işlem yapma

        currentHealth += amount;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Death");
        level++;
        if (txtLevel != null)
        {
            txtLevel.text = Convert.ToString(level);
        }
        isDead = true;
    }
}
