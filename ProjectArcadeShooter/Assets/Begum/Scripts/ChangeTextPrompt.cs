using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class ChangeTextPrompt : MonoBehaviour
{
    [SerializeField] private LocalizedString localStringPrompt;

    [SerializeField] private TextMeshProUGUI textRef;

    private string text_prompt;

   

   /* private void OnEnable()
    {
        localStringPrompt.Arguments = new object[] { text_prompt };
        localStringPrompt.StringChanged += Update_text;
    }

    private void OnDisable()
    {
        localStringPrompt.StringChanged -= Update_text;
    }*/
    private void Update_text(string value)
    {
        textRef.text = value;
    }
   
    public void ChangeTP(string value)
    {
        text_prompt = value;
        localStringPrompt.Arguments = new object[] { text_prompt };
        Debug.Log(localStringPrompt.Arguments[0]);
        localStringPrompt.Arguments[0] = text_prompt;
        localStringPrompt.RefreshString();
    }

    
   
}
