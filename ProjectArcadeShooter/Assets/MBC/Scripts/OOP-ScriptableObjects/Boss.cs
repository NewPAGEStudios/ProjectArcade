using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "SCPObjects/BossType")]
public class Boss : ScriptableObject
{
    public int BossID;
    [Header(header:"Bosses Object Configiration")]
    public string bossName;
    public GameObject boss;
    public GameObject mapParent;
    [Header(header:"Bosses functional Configiration")]
    public MonoScript bossController;
    public AudioSource bossMusic;
}