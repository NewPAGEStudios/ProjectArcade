using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SCPObjects/WeaponGetAmmoData")]

public class WeaponGetAmmoData : ScriptableObject
{
    [SerializeField]
    public WeaponGetAmmoData_holder.weaponGetAmmoData_holder[] weaponTypes;
}
public class WeaponGetAmmoData_holder
{
    [System.Serializable]
    public struct weaponGetAmmoData_holder
    {
        public int weaponID;
        public int weaponAmmoCount;
    }
}
