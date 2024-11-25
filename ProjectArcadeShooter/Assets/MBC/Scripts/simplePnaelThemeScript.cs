using SlimUI.ModernMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SlimUI.ModernMenu.UIMenuManager;

public class simplePnaelThemeScript : MonoBehaviour
{
    public enum Theme { custom1, custom2, custom3 };
    [Header("THEME SETTINGS")]
    public Theme theme;
    public ThemedUIData themeController;


    private void Start()
    {
        SetThemeColors();
    }

    void SetThemeColors()
    {
        switch (theme)
        {
            case Theme.custom1:
                themeController.currentColor = themeController.custom1.graphic1;
                themeController.textColor = themeController.custom1.text1;
                break;
            case Theme.custom2:
                themeController.currentColor = themeController.custom2.graphic2;
                themeController.textColor = themeController.custom2.text2;
                break;
            case Theme.custom3:
                themeController.currentColor = themeController.custom3.graphic3;
                themeController.textColor = themeController.custom3.text3;
                break;
            default:
                Debug.Log("Invalid theme selected.");
                break;
        }
    }

}
