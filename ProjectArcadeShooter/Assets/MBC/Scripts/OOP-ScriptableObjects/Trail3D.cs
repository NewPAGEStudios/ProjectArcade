using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SCPObjects/Trail3DType")]
public class Trail3D : ScriptableObject
{
    public int trail3DID;
    public float maxZScale;
    public GameObject trail;
}
