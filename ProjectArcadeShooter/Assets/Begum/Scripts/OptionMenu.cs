using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System;
public class OptionMenu : MonoBehaviour
{
    [Header(header:"VolumeSettings")]
    public Slider volSlider;
    public TMP_InputField volText;
    public AudioMixer audioMixer;
    public void SetVolume(float volume)//Volume is -40 decibell to 0 decibel
    {
        volume *= 4;
        volume -= 40;
        audioMixer.SetFloat("Volume", volume);
    }
    public void SetVolumeWSlider(float volume)
    {
        double volumed = Math.Round(volume, 2);
        volText.text = volumed.ToString();
        SetVolume(volume);
    }
    public void SetVolumeWInputField(string volume)
    {
        if(float.TryParse(volume,out float result))
        {
            if (result > 10f)
            {
                result = 10f;
            }
            else if (result < 0)
            {
                result = 0;
            }
            SetVolume(result);
            volSlider.value = result;
        }
        else
        {
            SetVolume(10f);
            volSlider.value = result;
        }

    }
    public void SetGraphic(int indexOfGraphSettings)
    {
        QualitySettings.SetQualityLevel(indexOfGraphSettings);
    }
    public void ChangeFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolotion(int indexOfResolotion)
    {

    }
}
