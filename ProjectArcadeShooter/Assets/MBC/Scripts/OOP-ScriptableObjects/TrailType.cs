using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SCPObjects/TrailType")]
public class TrailType : ScriptableObject
{
    public int trailID;
    public Gradient gradient;
    public AnimationCurve curve;
    public Material TrailMaterail;
    public float Time;
}
