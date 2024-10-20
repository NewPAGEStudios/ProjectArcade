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
    public Ammo ammo;
    public string agentName;
    public Vector3 firePos;
    public Material mainMat;
    public Material getDmgMat;
    public Material deathMat;
    public string agentFunction;

    [Header(header: "EnemyObjectProperties")]
    public float hitPoint;
    public float moveSpeed;
    [Range(0.1f, 10f)]
    public float attackRatio;
    public bool isRanged;
    public bool isFlyable;
    public float rangeDistance;
    public float attackDMG;

}
