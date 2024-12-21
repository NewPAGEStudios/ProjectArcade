using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
public class WaveEnemyType
{
    [System.Serializable]
    public struct waveEnemyType
    {
        public int enemyId;
        public string enemyName;
        public int enemyPiece;
    }

}

