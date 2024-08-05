using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SCPObjects/Perk")]
public class Perk : ScriptableObject
{
    public int perkID;
    
    public string perkName;
    public string perkDescription;

    public Sprite perkSpriteReferance;

}