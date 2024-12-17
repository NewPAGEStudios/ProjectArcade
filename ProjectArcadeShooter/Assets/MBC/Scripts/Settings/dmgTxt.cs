using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class dmgTxt : MonoBehaviour
{
    public float lifetimeTimer;
    public float lifetime = 1f;
    public bool open = false;
    private GameObject player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }
    private void Update()
    {
        if (open)
        {
            if (lifetimeTimer <= 0)
            {
                stopThis();
            }
            else
            {
                GetComponent<RectTransform>().position += new Vector3(0,0.5f*Time.deltaTime,0);
                GetComponent<TextMeshPro>().color = new Color(GetComponent<TextMeshPro>().color.r, GetComponent<TextMeshPro>().color.g, GetComponent<TextMeshPro>().color.b, GetComponent<TextMeshPro>().color.a - 4 * Time.deltaTime);
                transform.parent.LookAt(player.transform);
                lifetimeTimer -= Time.deltaTime;
            }
        }
    }
    public void startThis(string txt)
    {
        gameObject.SetActive(true);
        lifetimeTimer = lifetime;
        GetComponent<TextMeshPro>().color = new Color(GetComponent<TextMeshPro>().color.r, GetComponent<TextMeshPro>().color.g, GetComponent<TextMeshPro>().color.b,1f);
        GetComponent<RectTransform>().localPosition = Vector3.zero;
        GetComponent<TextMeshPro>().text = txt;
        open = true;
    }
    public void stopThis()
    {
        open = false;
        lifetimeTimer = lifetime;
        gameObject.SetActive(false);
    }

}
