using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


using UnityEngine.Localization.Settings;
public class LocaleHandler : MonoBehaviour
{
    private bool action;

    private int currentID;

    private void Start()
    {
        currentID = PlayerPrefs.GetInt("LocaleID", 0);
        StartCoroutine(setLocale(currentID));
    }

    public void ChangeLocale(int changerID)
    {
        if (action)
        {
            return;
        }
        if(currentID + changerID < 0)
        {
            currentID = LocalizationSettings.AvailableLocales.Locales.Count - 1;
        } 
        else if (currentID + changerID >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            currentID = 0;
        }
        else
        {
            currentID += changerID;
        }
        StartCoroutine(setLocale(currentID));
    }

    IEnumerator setLocale(int _LocaleID)
    {
        action = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_LocaleID];
        PlayerPrefs.SetInt("LocaleID", _LocaleID);
        currentID = _LocaleID;
        action = false;
    }


}
