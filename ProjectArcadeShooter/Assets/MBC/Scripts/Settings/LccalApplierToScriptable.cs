using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LccalApplierToScriptable : MonoBehaviour
{
    public Locale locale;
    public Locale localeq;
    public Locale localew;
    public Locale localee;

    public string applyCons(Consumable val,string keyName)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("ConsMenu", keyName + val.id.ToString());
    }
    public string applySkill(Skill val, string keyName)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("SkillMenu", keyName + val.skillTypeID.ToString());
    }
    public string applyWeapon(Weapon val, string keyName)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("WeaponMenu", keyName + val.WeaponTypeID.ToString());
    }
    public string applyMainMenu(string keyName)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("MainMenu", keyName);
    }
}
