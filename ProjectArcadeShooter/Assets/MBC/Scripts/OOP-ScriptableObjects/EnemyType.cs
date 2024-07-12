using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "SCPObjects/EnemyType")]
public class EnemyType : ScriptableObject
{
    public int EnemyTypeID;
    
    [Header(header:"Referances")]
    public GameObject modelGameObject;
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

    [Header(header: "Difficulty")]
    [Tooltip("2->walker 7->drone 8->ranged 10->droneRanged 10->demolisher")]
    public int difficultyNumber;
}
