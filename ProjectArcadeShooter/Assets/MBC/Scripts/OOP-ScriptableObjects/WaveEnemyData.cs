using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu( menuName = "SCPObjects/waveEnemyData")]
public class WaveEnemyData : ScriptableObject
{
    [SerializeField]
    public WaveEnemyType.waveEnemyType[] enemyTypes;
}
