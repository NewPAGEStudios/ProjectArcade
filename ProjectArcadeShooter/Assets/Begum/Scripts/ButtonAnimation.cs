using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ButtonAnimation : MonoBehaviour
{
    private Image backGround;
    public float animationSpeed;
    private bool ishovering = false;
    [Header("startingConfigration")]
    public Vector2 mainImageSize;
    public Vector2 mainImageSizeNormal;
    public Vector2 backgroundImageSize;



    // Update is called once per frame
    void Update()
    {
        if(ishovering)
        {
            backGround.rectTransform.sizeDelta = new Vector2(Mathf.MoveTowards(backGround.rectTransform.sizeDelta.x, backgroundImageSize.x, Time.deltaTime * animationSpeed), Mathf.MoveTowards(backGround.rectTransform.sizeDelta.y, backgroundImageSize.y, Time.deltaTime * animationSpeed/(backgroundImageSize.x/backgroundImageSize.y)));
        }
    }

    public void onHover(GameObject buttonParent)
    {
        backGround = buttonParent.transform.GetChild(1).GetComponent<Image>();
        buttonParent.transform.GetChild(2).GetComponent<TextMeshProUGUI>().faceColor = Color.black;
        buttonParent.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(mainImageSize.x, mainImageSize.y);
        ishovering = true;

    }

    public void outHover(GameObject  buttonParent)
    {
        ishovering = false;
        backGround.rectTransform.sizeDelta = Vector2.zero;
        buttonParent.transform.GetChild(2).GetComponent<TextMeshProUGUI>().faceColor = Color.white;
        buttonParent.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(mainImageSizeNormal.x, mainImageSizeNormal.y);
        backGround = null;
        
    }

}
