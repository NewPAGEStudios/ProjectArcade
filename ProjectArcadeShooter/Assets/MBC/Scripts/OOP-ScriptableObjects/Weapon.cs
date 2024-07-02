using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "SCPObjects/WeaponType")]

public class Weapon : ScriptableObject
{
    public enum WeaponType
    {
        semi,
        auto
    }


    public int WeaponTypeID;
    public string WeaponName;
    [Header(header: "Referances")]
    public GameObject modelGameObject;
    public Ammo usedAmmo;
    [Header(header: "EnemyObjectProperties")]
    public WeaponType type;
    public int magSize;
    public int usingAmmoPerAttack;
}
