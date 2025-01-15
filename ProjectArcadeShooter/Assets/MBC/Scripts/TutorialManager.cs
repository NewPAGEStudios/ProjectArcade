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
    public float disolvecutOffs1 = 5f;
    public float disolvecutOffs2 = 25f;
    public float disolvecutOffs3 = 5f;
    public float disolvecutOffs4 = 5f;
    public float disolvecutOffs5 = 5f;
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
        weaponReceived = true;
    }
    public void dashCompleted()
    {
        Tutstate = tutorialState.dash;
    }
    IEnumerator TutorialGeneralRoutine()
    {
        float timer = 0f;
        Tutstate = tutorialState.movement;
        gc.changeSub("Tutorial_start", 2f);
        gc.pState = GameController.PlayState.inPlayerInterrupt;
        while (true)
        {
            if (gc.state != GameController.GameState.inGame)
            {
                continue;
            }
            if (timer >= 2) break;
            yield return null;
            timer += Time.deltaTime;
        }
        gc.pState = GameController.PlayState.inTutorial;

        gc.changeSub("Tutorial_start01", 3f);
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
        gc.changeSub("Tutorial_Dash", 3f);
        while (true)//dashCheck
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