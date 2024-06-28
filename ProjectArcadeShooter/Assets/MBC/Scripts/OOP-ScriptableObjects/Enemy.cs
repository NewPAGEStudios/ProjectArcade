using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "SCPObjects/EnemyType")]
public class Enemy : ScriptableObject
{
    public int EnemyTypeID;
    
    [Header(header:"Referances")]
    public GameObject modelGameObject;
    public Animator animator;
    public MonoScript ai;
    public Ammo ammo;
    public string agentName;

    [Header(header: "EnemyObjectProperties")]
    public float hitPoint;
    public float moveSpeed;
    public float attackRatio;
    public bool isRanged;
    public float rangeDistance;
    public float attackDMG;
}
