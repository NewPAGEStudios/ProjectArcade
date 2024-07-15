using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMummyFunc : MonoBehaviour
{
    private float maxHP;
    private float currentHP;


    private enum BossState
    {
        interrupted,
        inIdle,
        inAttack,
        inDeath,
    }
    private BossState bossState;
    
    private GameObject target;
    private GameObject targetPos;
    private GameController gc;
    public Boss boss;
    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        bossState = BossState.interrupted;
    }

    // Update is called once per frame
    void Update()
    {
        if (bossState == BossState.interrupted)
        {
            return;
        }
        else if (bossState == BossState.inIdle)
        {
            
        }
        else if(bossState == BossState.inAttack)
        {

        }
        else if(bossState == BossState.inDeath)
        {

        }
    }
    public void startBoss()
    {
        bossState = BossState.inIdle;
    }
    private void GetDamage(float dmg)
    {
        currentHP -= dmg;
    }
}