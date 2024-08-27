using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public WaveEnemyData[] waveEnemyData;
    void Awake()
    {
        waveEnemyData = Resources.LoadAll<WaveEnemyData>("WaveEnemyData");


        Debug.Log(waveEnemyData[0].enemyTypes[1].enemyPiece.ToString());
    }
}
