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
    public bool isMelee;
    public bool reload;
    public bool twoStateReload;
//    public AudioClip soundOfWeaponOnFire;
    [Header(header: "ShopSettings")]
    public Sprite UIRef;
    public float toBuyMoney;
    [Header(header: "WeaponObjectProperties")]
    public WeaponType type;
    public int magSize;
    public int usingAmmoPerAttack;
}
