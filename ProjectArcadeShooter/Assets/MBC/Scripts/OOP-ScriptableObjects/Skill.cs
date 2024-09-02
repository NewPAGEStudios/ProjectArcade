using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "SCPObjects/SkillType")]
public class Skill : ScriptableObject
{
    public enum skillType
    {
        active,
        passive,
    }
    public skillType st;

    public int skillTypeID;
    public string skillName;
    [TextArea(15, 20)]
    public string skillDescription;
    [Tooltip("0:MainMat 1+:effects")]
    public Material[] materials;
    public Sprite sprite_HUD;
    public string functionName;
    public GameObject modelPrefab;

    public float toBuyMoney;
}
