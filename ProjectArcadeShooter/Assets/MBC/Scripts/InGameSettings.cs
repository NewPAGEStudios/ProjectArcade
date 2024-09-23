using SlimUI.ModernMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameSettings : MonoBehaviour
{

    private PController player;

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


    [Header("Game Options")]
    public GameObject swayBobtext;
    public GameObject damageVibrationtext;

    [Header("Slider Options")]
    public GameObject masterSlider;
    public GameObject musicSlider;
    public GameObject sFXSlider;
    public GameObject soundSlider;
    [Header("-----")]
    public GameObject sensitivityXSlider;
    public GameObject sensitivityYSlider;

    private float sliderValue = 0.0f;
    private float sliderValueXSensitivity = 0.0f;
    private float sliderValueYSensitivity = 0.0f;

    public void initValues()
    {
        player.ChangeSens(sliderValueYSensitivity, sliderValueYSensitivity);
        player.handleSNB(PlayerPrefs.GetInt("SwayNBobbing", 1) == 1 ? true : false);
        player.handleDV(PlayerPrefs.GetInt("DMGVibration", 1) == 1 ? true : false);
        player.handleDV(PlayerPrefs.GetInt("DMGVibration", 1) == 1 ? true : false);
        PlayerPrefs.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume",10));
        PlayerPrefs.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 10));
        PlayerPrefs.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume", 10));
        PlayerPrefs.SetFloat("SoundVolume", PlayerPrefs.GetFloat("SoundVolume", 10));
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PController>();


        masterSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MasterVolume");
        musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");
        sFXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SFXVolume");
        soundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SoundVolume");

        sensitivityXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("XSensitivity");
        sensitivityYSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("YSensitivity");

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

    public void SensitivityXSlider()
    {
        sliderValueXSensitivity = sensitivityXSlider.GetComponent<Slider>().value;
        PlayerPrefs.SetFloat("XSensitivity", sliderValueXSensitivity);
        player.ChangeSens(sliderValueYSensitivity, sliderValueYSensitivity);
    }

    public void SensitivityYSlider()
    {
        sliderValueYSensitivity = sensitivityYSlider.GetComponent<Slider>().value;
        PlayerPrefs.SetFloat("YSensitivity", sliderValueYSensitivity);
        player.ChangeSens(sliderValueYSensitivity, sliderValueYSensitivity);
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
        if (Screen.fullScreen != true)
        {

            fullscreen_parent.transform.GetChild(0).gameObject.SetActive(true);

        }
        else if (Screen.fullScreen != false)
        {

            fullscreen_parent.transform.GetChild(1).gameObject.SetActive(true);

        }
    }

}
