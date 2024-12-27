using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    GameController gc;
    PController playerController;
    WeaponManager playerWP;

    bool weaponReceived = false ;

    [Header("State1")]
    public GameObject door1;
    public enum tutorialState
    {
        movement,
        dash,
        crouch,
        takeConsAndChangeWeapom,
        meleeAttack,
        shootWeapon,
        laserTrying,
    }
    public tutorialState Tutstate;
    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.tutOpened = true;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PController>();
        playerWP = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponManager>();
    }
    private void Start()
    {
    }
    private void Update()
    {
    }

    public void startTutorial()
    {
        door1.GetComponent<DoorScripts>().open();
        weaponReceived = true;
    }
    IEnumerator TutorialGeneralRoutine()
    {
        Tutstate = tutorialState.movement;
        gc.changeSub("Tutorial_start", 3f);
        gc.pState = GameController.PlayState.inPlayerInterrupt;
        weaponReceived = false;
        while (true)
        {
            yield return null;
            if (gc.state != GameController.GameState.inGame)
            {
                continue;
            }
            if (weaponReceived)
            {
                break;
            }
        }
        while (true)
        {
            yield return null;
            if (gc.state != GameController.GameState.inGame)
            {
                continue;
            }
            if (Tutstate == tutorialState.dash)
            {
                break;
            }
        }
        while (true)//Crouch
        {
            yield return null;
            if (gc.state != GameController.GameState.inGame)
            {
                continue;
            }
            if (Tutstate == tutorialState.crouch)
            {
                break;
            }
        }
        while (true)//takeConsAndChangeWeapom
        {
            yield return null;
            if (gc.state != GameController.GameState.inGame)
            {
                continue;
            }
            if (Tutstate == tutorialState.takeConsAndChangeWeapom)
            {
                break;
            }
        }
        while (true)//meleeAttack
        {
            yield return null;
            if (gc.state != GameController.GameState.inGame)
            {
                continue;
            }
            if (Tutstate == tutorialState.meleeAttack)
            {
                break;
            }
        }
        while (true)//shootWeapon
        {
            yield return null;
            if (gc.state != GameController.GameState.inGame)
            {
                continue;
            }
            if (Tutstate == tutorialState.shootWeapon)
            {
                break;
            }
        }
        while (true)//laserTrying
        {
            yield return null;
            if (gc.state != GameController.GameState.inGame)
            {
                continue;
            }
            if (Tutstate == tutorialState.laserTrying)
            {
                break;
            }
        }


    }

}