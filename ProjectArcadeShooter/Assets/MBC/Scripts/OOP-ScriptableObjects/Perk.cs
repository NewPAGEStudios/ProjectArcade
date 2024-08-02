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



    private void OnEnable()
    {
        Perk[] perks = Resources.LoadAll<Perk>("Perks");
        if (perks == null)
        {
            this.perkID = 0;
            return;
        }
        int maxID = 1;
        for (int i = 0; i < perks.Length; i++)
        {
            if (maxID < perks[i].perkID)
            {
                maxID = perks[i].perkID;
            }
        }
        this.perkID = maxID;
    }
}
