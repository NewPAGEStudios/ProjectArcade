using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public Enemy m_Enemy;
    private float currentHP;
    private float moveSpeed;
    private float attackRat;
    private float rangeDist;
    private void Awake()
    {
        Instantiate(m_Enemy.modelGameObject, Vector3.zero, new Quaternion(0, 0, 0, 0), gameObject.transform);
        //Instantiate(gameObjectNameWhichwillbe has animator)
    }
    private void Start()
    {
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
}