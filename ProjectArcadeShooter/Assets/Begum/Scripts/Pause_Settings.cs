using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause_Settings : MonoBehaviour
{
    public void closeSetting()
    {
        transform.parent.GetComponent<Image>().color = new Color(transform.parent.GetComponent<Image>().color.r, transform.parent.GetComponent<Image>().color.g, transform.parent.GetComponent<Image>().color.b, 0.5f);
        transform.parent.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
