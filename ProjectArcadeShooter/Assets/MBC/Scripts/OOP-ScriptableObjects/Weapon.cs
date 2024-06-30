using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "SCPObjects/WeaponType")]

public class Weapon : ScriptableObject
{
    public int WeaponTypeID;
    public string WeaponName;
    [Header(header: "Referances")]
    public GameObject modelGameObject;
    public Ammo usedAmmo;
    [Header(header: "EnemyObjectProperties")]
    public float attackRatio;
    public float recoilDegree;
    public float attackRecoil;
    public int magSize;
    public int usingAmmoPerAttack;
    public float reloadTime;
}
