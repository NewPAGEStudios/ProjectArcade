using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "SCPObjects/AmmoType")]
public class Ammo : ScriptableObject
{
    public int AmmoTypeID;
    [Header(header: "Referance")]
    public MonoScript function;
    public GameObject modelGO;
    [Tooltip("0:MainMat 1+:effects")]
    public Material[] materials;
    [Header(header: "ObjectProperties")]
    public float bulletSpeed;
    public float dmg;
    public int maxReflectionTime;
}
