using SlimUI.ModernMenu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameSettings : MonoBehaviour
{

    private PController player;
    private MiniMapManager miniMapManager;

    public GameObject texturelowtextLINE;
    public GameObject texturemedtextLINE;
    public GameObject texturehightextLINE;




    [Header("PANELS")]
    [Tooltip("The UI Panel that holds the CONTROLS window tab")]
    public GameObject PanelControls;
    [Tooltip("The UI Panel that holds the VIDEO window tab")]
    public GameObject PanelVideo;
    [Tooltip("The UI Panel that holds the GAME window tab")]
    public GameObject PanelGame;
    [Tooltip("The UI Panel that holds the SOUND window tab")]
    public GameObject PanelSound;

    [Header("SETTINGS SCREEN")]
    [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
    public GameObject lineGame;
    [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
    public GameObject lineSound;
    [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
    public GameObject lineVideo;
    [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
    public GameObject lineControls;


    [Header("VIDEO SETTINGS")]
    public GameObject fullscreentext;
    [SerializeField] private TMP_Dropdown resDropdown;


    [Header("Game Options")]
    public GameObject minimapSmoothnestext;

    public GameObject swayBobtext;
    public GameObject damageVibrationtext;

    public GameObject crossGrid;

    [Header("Slider Options")]
    public GameObject miniMapSizeSlider;
    public GameObject crossSizeSlider;
    [Header("Music")]
    public GameObject masterSlider;
    public GameObject musicSlider;
    public GameObject sFXSlider;
    public GameObject soundSlider;
    [Header("Sens")]
    public GameObject sensitivityXSlider;
    public GameObject sensitivityYSlider;
    [Header("")]
    private float sliderValueMinimapSize = 0.0f;
    private float sliderValueCrossSize = 0.0f;

    private float sliderValueXSensitivity = 0.0f;
    private float sliderValueYSensitivity = 0.0f;

    private int currentResolutionIndex = 0;
    //Resolution
    private Resolution[] res;
    private List<Resolution> resList = new();
    [Header("GameController")]
    public GameController gc;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PController>();
        miniMapManager = GameObject.FindGameObjectWithTag("MiniMap").GetComponent<MiniMapManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(Sprite sprite in gc.crossSprites)
        {
            GameObject ima = new();
            ima.transform.parent = crossGrid.transform;

            ima.transform.localPosition = Vector3.zero;
            ima.transform.localEulerAngles = Vector3.zero;
            ima.transform.localScale = Vector3.one;

            ima.name = sprite.name;

            ima.AddComponent<Image>();
            ima.GetComponent<Image>().sprite = sprite;
        }
        crossGrid.GetComponent<GridLayoutGroup>().padding.left = gc.crossSprites.Count * (int)(crossGrid.GetComponent<GridLayoutGroup>().cellSize.x + crossGrid.GetComponent<GridLayoutGroup>().spacing.x);

        crossGrid.transform.localPosition = new Vector3( - 50 - ((gc.crossSprites.Count - 1 - PlayerPrefs.GetInt("crossID", 0)) * 100), 0, 0);

        //resolution init
        res = Screen.resolutions;


        ResDropdownUpdateValues();


        miniMapSizeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MinimapSize");
        crossSizeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("CrossSize");

        masterSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MasterVolume");
        musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");
        sFXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SFXVolume");
        soundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SoundVolume");

        sensitivityXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("XSensitivity");
        sensitivityYSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("YSensitivity");

        // check minimapSmoth
        bool smMM = PlayerPrefs.GetInt("MinimapSmoothness") == 1f ? true : false;
        GameObject snMMParent = minimapSmoothnestext.transform.parent.gameObject;
        snMMParent.transform.GetChild(0).gameObject.SetActive(false);
        snMMParent.transform.GetChild(1).gameObject.SetActive(false);
        if (smMM)
        {

            snMMParent.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            snMMParent.transform.GetChild(0).gameObject.SetActive(true);
        }

        // check sway n Bobbing
        bool snb = PlayerPrefs.GetInt("SwayNBobbing") == 1 ? true : false;
        GameObject snbParent = swayBobtext.transform.parent.gameObject;
        snbParent.transform.GetChild(0).gameObject.SetActive(false);
        snbParent.transform.GetChild(1).gameObject.SetActive(false);
        if (snb)
        {

            snbParent.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            snbParent.transform.GetChild(0).gameObject.SetActive(true);
        }

        // check DamageVibration
        bool dv = PlayerPrefs.GetInt("DMGVibration") == 1 ? true : false;
        GameObject dvParent = damageVibrationtext.transform.parent.gameObject;
        dvParent.transform.GetChild(0).gameObject.SetActive(false);
        dvParent.transform.GetChild(1).gameObject.SetActive(false);
        if (dv)
        {
            dvParent.transform.GetChild(1).gameObject.SetActive(true);


        }
        else
        {
            dvParent.transform.GetChild(0).gameObject.SetActive(true);
        }


        // check fullscreen
        GameObject fullscreen_parent = fullscreentext.transform.parent.gameObject;
        fullscreen_parent.transform.GetChild(0).gameObject.SetActive(false);
        fullscreen_parent.transform.GetChild(1).gameObject.SetActive(false);
        if (Screen.fullScreen == true)
        {

            fullscreen_parent.transform.GetChild(1).gameObject.SetActive(true);

        }
        else if (Screen.fullScreen == false)
        {

            fullscreen_parent.transform.GetChild(0).gameObject.SetActive(true);

        }
        if (PlayerPrefs.GetInt("Textures") == 0)
        {
            texturelowtextLINE.gameObject.SetActive(true);
            texturemedtextLINE.gameObject.SetActive(false);
            texturehightextLINE.gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Textures") == 1)
        {
            texturelowtextLINE.gameObject.SetActive(false);
            texturemedtextLINE.gameObject.SetActive(true);
            texturehightextLINE.gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Textures") == 2)
        {
            texturelowtextLINE.gameObject.SetActive(false);
            texturemedtextLINE.gameObject.SetActive(false);
            texturehightextLINE.gameObject.SetActive(true);
        }

    }

    //UI MENUS FUNC
    public void CloseOptions()
    {
        gameObject.transform.parent.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.parent.GetChild(1).gameObject.SetActive(false);
    }
    void DisablePanels()
    {
        PanelControls.SetActive(false);
        PanelVideo.SetActive(false);
        PanelGame.SetActive(false);
        PanelSound.SetActive(false);

        lineGame.SetActive(false);
        lineControls.SetActive(false);
        lineVideo.SetActive(false);
        lineSound.SetActive(false);
    }

    public void GamePanel()
    {
        DisablePanels();
        PanelGame.SetActive(true);
        lineGame.SetActive(true);
    }
    public void SoundPanel()
    {
        DisablePanels();
        PanelSound.SetActive(true);
        lineSound.SetActive(true);
    }

    public void VideoPanel()
    {
        DisablePanels();
        PanelVideo.SetActive(true);
        lineVideo.SetActive(true);
    }

    public void ControlsPanel()
    {
        DisablePanels();
        PanelControls.SetActive(true);
        lineControls.SetActive(true);
    }


    public void TexturesLow()
    {
        PlayerPrefs.SetInt("Textures", 0);
        QualitySettings.SetQualityLevel(0);
        texturelowtextLINE.gameObject.SetActive(true);
        texturemedtextLINE.gameObject.SetActive(false);
        texturehightextLINE.gameObject.SetActive(false);
    }

    public void TexturesMed()
    {
        PlayerPrefs.SetInt("Textures", 1);
        QualitySettings.SetQualityLevel(1);
        texturelowtextLINE.gameObject.SetActive(false);
        texturemedtextLINE.gameObject.SetActive(true);
        texturehightextLINE.gameObject.SetActive(false);
    }

    public void TexturesHigh()
    {
        PlayerPrefs.SetInt("Textures", 2);
        QualitySettings.SetQualityLevel(2);
        texturelowtextLINE.gameObject.SetActive(false);
        texturemedtextLINE.gameObject.SetActive(false);
        texturehightextLINE.gameObject.SetActive(true);
    }


    public void MasterSlider()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.GetComponent<Slider>().value);
    }
    public void MusicSlider()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.GetComponent<Slider>().value);
    }
    public void SFXSlider()
    {
        PlayerPrefs.SetFloat("SFXVolume", sFXSlider.GetComponent<Slider>().value);
    }
    public void SoundSlider()
    {
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.GetComponent<Slider>().value);
    }

    public void sizeOfMinimapSlider()
    {
        sliderValueMinimapSize = miniMapSizeSlider.GetComponent<Slider>().value;
        PlayerPrefs.SetFloat("MinimapSize", sliderValueMinimapSize);
        miniMapManager.ChangeSize(50 - sliderValueMinimapSize);
    }

    public void sizeOfCross()
    {
        sliderValueCrossSize = crossSizeSlider.GetComponent<Slider>().value;
        PlayerPrefs.SetFloat("CrossSize", sliderValueCrossSize);
        gc.changeSizeOfCross(sliderValueCrossSize * 10);
        gc.changeSizeOfLaserIndication(sliderValueCrossSize * 15);
    }


    public void SensitivityXSlider()
    {
        sliderValueXSensitivity = sensitivityXSlider.GetComponent<Slider>().value;
        PlayerPrefs.SetFloat("XSensitivity", sliderValueXSensitivity);
        player.ChangeSens(sliderValueXSensitivity + 1, sliderValueYSensitivity + 1);
    }

    public void SensitivityYSlider()
    {
        sliderValueYSensitivity = sensitivityYSlider.GetComponent<Slider>().value;
        Debug.Log(sensitivityYSlider.GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("YSensitivity", sliderValueYSensitivity);
        player.ChangeSens(sliderValueXSensitivity + 1, sliderValueYSensitivity + 1);
    }

    public void MiniMapSmooth()
    {
        bool smMM = PlayerPrefs.GetInt("MinimapSmoothness") == 1f ? true : false;
        GameObject snMMParent = minimapSmoothnestext.transform.parent.gameObject;
        snMMParent.transform.GetChild(0).gameObject.SetActive(false);
        snMMParent.transform.GetChild(1).gameObject.SetActive(false);
        if (smMM)
        {
            PlayerPrefs.SetInt("MinimapSmoothness", 0);
            snMMParent.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("MinimapSmoothness", 1);
            snMMParent.transform.GetChild(1).gameObject.SetActive(true);
        }
        miniMapManager.smooth = PlayerPrefs.GetInt("MinimapSmoothness") == 1f ? true : false;
    }

    public void SwayNBobbing()
    {
        bool snb = PlayerPrefs.GetInt("SwayNBobbing") == 1 ? true : false;
        GameObject snbParent = swayBobtext.transform.parent.gameObject;
        snbParent.transform.GetChild(0).gameObject.SetActive(false);
        snbParent.transform.GetChild(1).gameObject.SetActive(false);
        if (snb)
        {
            PlayerPrefs.SetInt("SwayNBobbing", 0);
            snbParent.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("SwayNBobbing", 1);
            snbParent.transform.GetChild(1).gameObject.SetActive(true);
        }
        player.handleSNB(PlayerPrefs.GetInt("SwayNBobbing") == 1 ? true : false);

    }

    public void dmgVibration()
    {
        bool dv = PlayerPrefs.GetInt("DMGVibration") == 1 ? true : false;
        GameObject dvParent = damageVibrationtext.transform.parent.gameObject;
        dvParent.transform.GetChild(0).gameObject.SetActive(false);
        dvParent.transform.GetChild(1).gameObject.SetActive(false);
        if (dv)
        {
            PlayerPrefs.SetInt("DMGVibration", 0);
            dvParent.transform.GetChild(0).gameObject.SetActive(true);


        }
        else
        {
            PlayerPrefs.SetInt("DMGVibration", 1);
            dvParent.transform.GetChild(1).gameObject.SetActive(true);
        }
        player.handleDV(PlayerPrefs.GetInt("DMGVibration") == 1 ? true : false);
    }


    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;

        GameObject fullscreen_parent = fullscreentext.transform.parent.gameObject;
        fullscreen_parent.transform.GetChild(0).gameObject.SetActive(false);
        fullscreen_parent.transform.GetChild(1).gameObject.SetActive(false);
        if (Screen.fullScreen == true)
        {

            fullscreen_parent.transform.GetChild(1).gameObject.SetActive(true);

        }
        else if (Screen.fullScreen == false)
        {

            fullscreen_parent.transform.GetChild(0).gameObject.SetActive(true);

        }
    }
    public void crossChange(int change)
    {
        if (PlayerPrefs.GetInt("crossID") + change < 0)
        {
            PlayerPrefs.SetInt("crossID", gc.crossSprites.Count - 1);
        }
        else if (PlayerPrefs.GetInt("crossID") + change >= gc.crossSprites.Count)
        {
            PlayerPrefs.SetInt("crossID", 0);
        }
        else
        {
            PlayerPrefs.SetInt("crossID", PlayerPrefs.GetInt("crossID", 0) + change);
        }
        crossGrid.transform.localPosition = new Vector3(-50 - ((gc.crossSprites.Count - 1 - PlayerPrefs.GetInt("crossID", 0)) * 100), 0, 0);
        gc.playerPanel.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = gc.crossSprites[PlayerPrefs.GetInt("crossID", 0)];
    }


    public void ResDropdownUpdateValues()
    {
        resDropdown.ClearOptions();
        resList.Clear();
        double currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;
        for (int i = 0; i < res.Length; i++)
        {
            if (res[i].refreshRateRatio.value == currentRefreshRate)
            {
                resList.Add(res[i]);
            }
        }



        List<string> options_Res = new List<string>();
        for (int c = 0; c < resList.Count; c++)
        {
            string resolutionOption = resList[c].width + "x" + resList[c].height + " " + resList[c].refreshRateRatio + " Hz";
            options_Res.Add(resolutionOption);
            if (resList[c].width == Screen.width && resList[c].height == Screen.height)
            {
                currentResolutionIndex = c;
            }
        }


        resDropdown.AddOptions(options_Res);
        resDropdown.value = currentResolutionIndex;
        resDropdown.RefreshShownValue();

    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resList[resolutionIndex];

//        player.GetComponent<PController>().fovChange(60f);

        float scaleRatio = ((float)resolution.width / (float)resolution.height) / 1.78f;

//        player.GetComponent<PController>().fovChange(60f / scaleRatio);
//        player.GetComponent<PController>().aspectChange((float)resolution.width / (float)resolution.height);

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);



    }


}
