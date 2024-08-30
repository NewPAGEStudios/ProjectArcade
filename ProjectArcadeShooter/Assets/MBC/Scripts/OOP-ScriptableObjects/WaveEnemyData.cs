using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu( menuName = "SCPObjects/waveEnemyData")]
public class WaveEnemyData : ScriptableObject
{
    public int waveNumber;
    [Header("Enemies Configiration")]
    [SerializeField]
    public WaveEnemyType.waveEnemyType[] enemyTypes;
    public float enemyStatMultiplier;

    [Header("Wave Configiration")]
    public int maxConsSpawn;
    public int maxAmmoSpawn;
}
