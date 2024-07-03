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
        instant,
    }
    public skillType st;

    public int skillTypeID;
    public string skillName;
    [TextArea(15, 20)]
    public string skillDescription;
    public Sprite sprite_HUD;
    public MonoScript function;
    public GameObject modelPrefab;
}
