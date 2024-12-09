using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    GameController gc;
    [Header("State1")]
    public GameObject door1;

    public enum tutorialState
    {
        movement,
        dash,
        crouch,
        takeWeapon,
        meleeAttack,
        shootWeapon,
        laserTrying,
    }
    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.tutOpened = true;
    }


    public void firstOpen()
    {

    }
}