using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "SCPObjects/ConsumableType")]

public class Consumable : ScriptableObject
{
    public int id;
    public string nameOfC;
    [TextArea(15,20)]
    public string descriptionOfC;
    public MonoScript function;
    public Mesh modelMesh;
    public Material[] mats;
}
