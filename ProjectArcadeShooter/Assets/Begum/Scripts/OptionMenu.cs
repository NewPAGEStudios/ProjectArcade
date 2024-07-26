using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public Slider volSlider;
    public void SetVolume(float volume)
    {
        Debug.Log(volume);
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
}
