using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public EnemyType m_Enemy;
    private float currentHP;
    private float moveSpeed;
    private float attackRat;
    private float rangeDist;

    private GameController gc;
    private GameObject modelObject;
    private void Start()
    {
        modelObject = Instantiate(m_Enemy.modelGameObject, gameObject.transform.position, new Quaternion(0, 0, 0, 0), gameObject.transform);
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        modelObject.tag = "Enemy";
        //main 
        if (m_Enemy == null)
        {
            Destroy(gameObject);
        }
        currentHP = m_Enemy.hitPoint;
        moveSpeed = m_Enemy.moveSpeed;
        rangeDist = m_Enemy.rangeDistance;
        attackRat = m_Enemy.attackRatio;

        Debug.Log(m_Enemy.ai.GetClass());
        gameObject.AddComponent(m_Enemy.ai.GetClass());
        return;//delete this block after ai completely done

        //Navmesh AI Agent select by repo
        int agentID = agenjtFindClass.GetAgentTypeIDbyName(m_Enemy.agentName);
        gameObject.AddComponent<NavMeshAgent>();
        if (agentID != -1)
        {
           GetComponent<NavMeshAgent>().agentTypeID = agentID;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void takeDmg(float dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            gc.decreseEnemyCount();
            Destroy(gameObject);
        }
    }
}