using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DataElem
{
    //PController
    public float currentHP;
    public float maxHP;

    public float dashMeter;
    
    //WeaponManager
    public int[] weap_wrh_id;
    public int[] weap_wrh_sumAmmo;
    public int[] weap_wrh_inWeaponAmmo;
    public int[] weap_wrh_maxMagAmount;
    public int[] weap_wrh_isOwned;//1:true 0:false

    public int activeSkill_ID;

    //GameController
    public float money;

    public int[] consPosID;
    public int[] consID;
    public int[] cons_weap_ID;
    public int[] cons_skill_ID;

    public int wave_Number;
    public DataElem(PController player_control , WeaponManager player_weapon, GameController gm)
    {
        //PController
        currentHP = player_control.currentHP;
        dashMeter = player_control.currentdashMeter;
        maxHP = player_control.getMaxhHP();

        //WeaponManager
        weap_wrh_id = new int[player_weapon.holder.Length];
        weap_wrh_sumAmmo = new int[player_weapon.holder.Length];
        weap_wrh_inWeaponAmmo = new int[player_weapon.holder.Length];
        weap_wrh_maxMagAmount = new int[player_weapon.holder.Length];
        weap_wrh_isOwned = new int[player_weapon.holder.Length];
        for (int w = 0; w < player_weapon.holder.Length; w++)
        {
            weap_wrh_id[w] = player_weapon.holder[w].weaponTypeID;
            weap_wrh_sumAmmo[w] = player_weapon.holder[w].sum_ammoAmount;
            weap_wrh_inWeaponAmmo[w] = player_weapon.holder[w].inWeapon_ammoAmount;
            weap_wrh_maxMagAmount[w] = player_weapon.holder[w].maxMagAmount;

            if (player_weapon.holder[w].isOwned)
            {
                weap_wrh_isOwned[w] = 1;
            }
            else
            {
                weap_wrh_isOwned[w] = 0;
            }
        }
        if (player_weapon.active_Skill != null)
        {
            activeSkill_ID = player_weapon.active_Skill.skillTypeID;
        }
        else
        {
            activeSkill_ID = -1;
        }

        //GameController
        money = gm.money;

        consPosID = new int[gm.activeCons.Count];
        consID = new int[gm.activeConsID.Count];
        cons_weap_ID = new int[gm.activeConsWeapID.Count];
        cons_skill_ID = new int[gm.activeConsSkill.Count];
        for (int c = 0; c < gm.activeCons.Count; c++)
        {
            consPosID[c] = gm.activeCons[c];
            consID[c] = gm.activeConsID[c];
            if (gm.activeConsWeapID[c] != -1)
            {
                cons_weap_ID[c] = gm.activeConsWeapID[c];
                cons_skill_ID[c] = -1;
            }
            else if (gm.activeConsSkill[c] != -1)
            {
                cons_weap_ID[c] = -1;
                cons_skill_ID[c] = gm.activeConsSkill[c];

            }
        }
        wave_Number = gm.waveNumber;

    }
}