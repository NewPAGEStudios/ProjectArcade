using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "SCPObjects/AmmoType")]
public class Ammo : ScriptableObject
{
    public int AmmoTypeID;
    [Header(header: "Referance")]
    public string functionName;
    public GameObject modelGO;
    public TrailType trail;
    public Trail3D trail3D;
    [Tooltip("0:MainMat 1+:effects")]
    public Material[] materials;
    [Header(header: "ObjectProperties")]
    public float bulletSpeed;
    public float dmg;
    public int maxReflectionTime;
}
