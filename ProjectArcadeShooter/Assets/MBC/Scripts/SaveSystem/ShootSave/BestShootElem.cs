using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BestShootElem
{
    //current rbf
    public int[] xPos = new int[6];
    public int[] yPos = new int[6];
    public int[] zPos = new int[6];

    public int xPosfb;
    public int yPosfb;
    public int zPosfb;

    //other rbfs
    public int[] current_hit;



    //player pos
    public int[] player_Pos = new int[3];
    public int[] player_Rot = new int[3];

    //Enemies
    public int[] enemyID;
    public int[] enemies_Pos;

}
