using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class StatisticManager : MonoBehaviour
{
    GameController gc;

    [HideInInspector]
    public int waveNumber;

    public int numberofKilled;

    public string killedBy_GameOver = "";

    public int[] killedEnemy;

    public int[] numberOfFire;
    public int[] mostCombo;

    public int sumOfCombo;

    private void Awake()
    {
        gc = GetComponent<GameController>();
        numberofKilled = PlayerPrefs.GetInt("statistical_killedTime", 0);
    }


    void Start()
    {
        killedEnemy = new int[gc.waveEnemyDatas.Length];
        numberOfFire = new int[gc.waveEnemyDatas.Length];
        mostCombo = new int[gc.waveEnemyDatas.Length];

    }

    public void increaseWaveNumber()
    {
        waveNumber += 1;
    }

    public void incKilledEnemy()
    {
        killedEnemy[waveNumber] += 1;
    }
    public void incnumberOfFire()
    {
        numberOfFire[waveNumber] += 1;
    }
    public void updateMostCombo(int most)
    {
        mostCombo[waveNumber] = most;
    }

    public void incSumOfCombo(int time)
    {
        sumOfCombo += time;
    }


    public void playerDies()
    {
        numberofKilled += PlayerPrefs.GetInt("statistical_killedTime", 0) + 1;
        PlayerPrefs.SetInt("statistical_killedTime", PlayerPrefs.GetInt("statistical_killedTime", 0) + 1);
    }






    public void saveDatas()
    {
        string data = "";
        data += "ComputerName : " + Environment.MachineName + "\n\n\n";
        //PlayerDied
        data += "Player died " + numberofKilled + " times \n";
        if(killedBy_GameOver != "")
        {
            data += "Player Eliminated by : " + killedBy_GameOver + "\n";
        }
        int sumKilledEnemy = 0;
        foreach(int ke in killedEnemy)
        {
            sumKilledEnemy += ke;
        }
        //killedEnemy
        data += "Sum of killed Enemies : " + sumKilledEnemy + "\n";
        //sumOFCombo
        data += "Sum of combo player did : " + sumOfCombo + "\n";
        //Wave info
        int count = 0;
        while (count <= waveNumber)
        {
            data += ("Wave Number " + waveNumber + "\n").ToUpper();
            data += "Enemy Killed : " + killedEnemy[count] + "\n";
            data += "Number of fire Time : " + numberOfFire[count] + "\n";
            data += "Most Combo Did: " + mostCombo[count] + "\n";

            data = data + "\n";
            count++;
        }
        saveTxtData.CreateData(data);
    }
}
