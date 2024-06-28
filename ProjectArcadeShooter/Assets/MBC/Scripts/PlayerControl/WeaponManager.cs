using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;

public class WeaponManager : MonoBehaviour
{
    //player object -- can be turn in to scriptableobject
    public float startHP;
    private float currentHP;


    //Reference
    private GameController gc;

    //RuntimeObject
    private int currWeaponID;
    private int currWeaponMag; //runtime ammo in Mag
    private int currWeaponAmmoAmount;

    //objectWeapon
    private float attackTime;
    private float attackRecoil;
    private int magmax;

    //timerWeapon
    private float attackTimer;

    //ObjectWeaponRuntime
    private weaponRuntimeHolder[] holder;


    //WeaponActiveReferance
    private GameObject activeWeaponParent;


    [SerializeField]
    private float smooth = 10f;
    [SerializeField]
    private float smoothRot = 12f;

    [Header("Sway N Bobbing")]
    //INpsector taking
    //Sway
    private Vector3 swayPos;
    private Vector3 swayEulorRot;

    //Bobbing
    private Vector3 bobPos;
    private Vector3 bobEulorRot;

    public float bobExaggeration;
    public Vector3 multiplier;

    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;

    [Header("FirePosition")]
    public GameObject firePos;

    [Header("Ref of Sway n Bob Apllier")]
    public GameObject sb_apllier;

    [Header("InverseKinematics For Right Hand Refs")]
    public GameObject targetR;
    private Vector3 targetRNormalPos;
    private Vector3 targetRNormalRot;
    private Vector3 targetRNormalScale;


    //ActionStates
    private enum actionStateOFHands
    {
        idle,
        weaponed,
        inFire,
        inReload,
        inUnsheate
    }
    actionStateOFHands handStates;



    private void Start()
    {
        handStates = actionStateOFHands.idle;
        //init
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        //init of active WQeapon Parent
        activeWeaponParent = gc.getHandObject().transform.GetChild(0).gameObject;//activeWeaponGAMEOBJECT

        //objectInit
        currentHP = startHP;
        
        //holder init
        holder = new weaponRuntimeHolder[gc.weapons.Length];
        for (int i = 0; i < holder.Length; i++)
        {
            holder[i] = new weaponRuntimeHolder();
            holder[i].weaponTypeID = gc.weapons[i].WeaponTypeID;
            holder[i].maxMagAmount = gc.weapons[i].magSize;
            holder[i].isOwned = false;
        }
        //currentWeaponID init -for now we will use it on global variable init

        //weaponStart
        magmax = -1;
        attackTime = -1;
        attackRecoil = -1;
        currWeaponID = -1;
        //        getWeapon(bombaGuy);

        //Animation IK
        targetRNormalPos = targetR.transform.localPosition;
        targetRNormalRot = targetR.transform.localEulerAngles;
        targetRNormalScale = targetR.transform.localScale;

        Debug.Log(targetRNormalRot);
    }
    private void Update()
    {
        // sway and bobbing with movement
        if (currWeaponID == -1)
        {
            return;
        }
        if (/*currWeaponMag <= 0 || */Input.GetKeyDown(KeyCode.R))
        {
            reload();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            changeWeapon(currWeaponID + 1);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (attackTimer <= 0)
            {
                fire();
            }
        }
        //timer init
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

    }

    public void takeDMG(float dmgAmmount, GameObject dmgTakenFrom)
    {
        currentHP -= dmgAmmount;
        if (currentHP <= 0)
        {
            Debug.Log("Die");
        }
        else
        {
            Debug.Log("Damaged From " + dmgTakenFrom.name + "damage is " + dmgAmmount +" currentHP = " + currentHP);
        }
    }
    public void getWeapon(int weaponID)
    {
        if (weaponID >= gc.weapons.Length)//wil be deleted
        {
            return;
        }
        if (findWeaponOnRuntime(weaponID).isOwned)
        {
            Debug.Log("You get " + weaponID + "id weapon Ammo");
            getAmmo(weaponID,findWeapon(weaponID).magSize);
        }
        else
        {
            Debug.Log("You get " + weaponID + "id weapon");
            findWeaponOnRuntime(weaponID).isOwned = true;
            getAmmo(weaponID, findWeapon(weaponID).magSize);
        }
        if (currWeaponID == -1)
        {
            changeWeapon(weaponID);
        }
    }
    public void getAmmo(int weaponTypeIndex, int ammoAmount)
    {
        if (weaponTypeIndex != currWeaponID)
        {
            findWeaponOnRuntime(weaponTypeIndex).ammoAmount += ammoAmount;
            return;
        }
        else
        {
            currWeaponAmmoAmount += ammoAmount;
            gc.changeAmmoText(currWeaponAmmoAmount);
            return;
        }
        //delete Returns and show a feedback for getting ammo like sound or effect
    }
    private void changeWeapon(int weaponID)
    {
        if (weaponID >= gc.weapons.Length)
        {
            weaponID = 0;
        }
        else if (weaponID < 0)
        {
            weaponID = gc.weapons.Length - 1;
        }
        while (!findWeaponOnRuntime(weaponID).isOwned)
        {
            weaponID += 1;
            if (weaponID >= gc.weapons.Length)
            {
                weaponID = 0;
            }
            else if (weaponID < 0)
            {
                weaponID = gc.weapons.Length - 1;
            }
        }
        if (findWeaponOnRuntime(weaponID).weaponTypeID == currWeaponID)
        {
            Debug.Log("You only have current weapon");
            return;
        }
        if (currWeaponID != -1)
        {
            findWeaponOnRuntime(currWeaponID).magInAmmoAmount = currWeaponMag;
            findWeaponOnRuntime(currWeaponID).ammoAmount = currWeaponAmmoAmount;
        }

        currWeaponMag = findWeaponOnRuntime(weaponID).magInAmmoAmount;
        currWeaponAmmoAmount = findWeaponOnRuntime(weaponID).ammoAmount;
        magmax = findWeaponOnRuntime(weaponID).maxMagAmount;

        gc.changeAmmoText(currWeaponMag);
        gc.changefullAmmoText(currWeaponAmmoAmount);

        attackTime = findWeapon(weaponID).attackRatio;
        attackRecoil = findWeapon(weaponID).attackRecoil;
        attackTimer = 0;

        if (currWeaponID == -1)//special state it means you don't have any weapon
        {
            changeWeaponModel(weaponID);
        }
        else
        {
            changeWeaponModel(weaponID);
        }
        currWeaponID = weaponID;
        Debug.Log("Current weapon has changed to " +  weaponID + " id number");
    }

    private void fire()
    {
        Weapon w = findWeapon(currWeaponID);
        if (currWeaponMag <= 0)
        {
            //soundManager.noAmmoFX.Play()
            return;
        }
        //Animator.SetBool("StartFire")
        Debug.Log("Weapon ID = " + currWeaponID + " -- Fired");
        currWeaponMag -= w.usingAmmoPerAttack;

        gc.changeAmmoText(currWeaponMag);
        gc.changefullAmmoText(currWeaponAmmoAmount);

        attackTimer = attackTime;

        //spawnBullet()
        GameObject ammo = new GameObject();
        ammo.transform.position = firePos.transform.position;
        ammo.transform.rotation = firePos.transform.rotation;
        ammo.name = "Ammo";
        ammo.AddComponent(w.usedAmmo.function.GetClass());
        
        ammo.AddComponent<Rigidbody>();
        ammo.GetComponent<Rigidbody>().useGravity = false;
        //ManuelAdding
        if(ammo.TryGetComponent<ReflectBulletFunctions>(out ReflectBulletFunctions rbf))
        {
            rbf.modelMat = w.usedAmmo.materials;
            rbf.bulletSpeed = w.usedAmmo.bulletSpeed;
            rbf.mostHitCanBeDone = w.usedAmmo.maxReflectionTime;
            rbf.dmg = w.usedAmmo.dmg;
            Instantiate(w.usedAmmo.modelGO, ammo.transform);
        }
        else
        {
            Debug.Log("Bomba!!!");
        }


        Debug.Log("Current Ammo " + currWeaponMag + "/FullMag " + magmax + "/Ammo Amount " + currWeaponAmmoAmount);
        //recoil animation
        StopCoroutine("recoilCoroutineKickback");
        StartCoroutine(recoilCoroutineDegree());
        StartCoroutine(recoilCoroutineKickback());
    }

    private void reload()
    {
        //block reload if there is not ammoAmount
        if (currWeaponAmmoAmount <= 0)
        {
            Debug.Log("You don't have enough ammo");
            return;
        }
        else if(currWeaponMag == magmax)
        {
            Debug.Log("Full Ammo");
            return;
        }
        int toMakeFullMag = magmax - currWeaponMag;
        //reload section
        if (toMakeFullMag <= currWeaponAmmoAmount)
        {
            currWeaponMag = magmax;
            currWeaponAmmoAmount -= toMakeFullMag;
        }
        else
        {
            currWeaponMag += currWeaponAmmoAmount;
            currWeaponAmmoAmount = 0;
        }

        gc.changeAmmoText(currWeaponMag);
        gc.changefullAmmoText(currWeaponAmmoAmount);

    }
    //swayNBobbing
    public void sway(Vector3 inputCam)
    {
        Vector3 invertLook = inputCam * -0.01f;
        invertLook.x = Mathf.Clamp(invertLook.x, -0.06f, 0.06f);
        invertLook.y = Mathf.Clamp(invertLook.y, -0.06f, 0.06f);
    
        swayPos = invertLook;
    }
    public void swayRotation(Vector3 inputCam)
    {
        Vector2 invertLook = inputCam * -4f;
        invertLook.x = Mathf.Clamp(invertLook.x, -5f, 5f);
        invertLook.y = Mathf.Clamp(invertLook.y, -5f, 5f);

        swayEulorRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }
    public void bobOffset(pController player,Vector2 moveDir)
    {
        if (player.actiontg == pController.actionStateDependecyToGround.flat || player.actiontg == pController.actionStateDependecyToGround.slope)
        {
            speedCurve += (Time.deltaTime * (moveDir.x + moveDir.y) * bobExaggeration) + 0.01f;
            bobPos.x = (curveCos * bobLimit.x * 1) - (moveDir.x * travelLimit.x);
        }
        else
        {
            speedCurve += (Time.deltaTime * 1) + 0.01f;
            bobPos.x = (curveCos * bobLimit.x * 0) - (moveDir.x * travelLimit.x);
        }
        bobPos.y = (curveSin * bobLimit.y) - (moveDir.y * travelLimit.y);
        bobPos.z = -(moveDir.y * travelLimit.z);
    }
    public void bobRotation(pController player, Vector2 moveDir)
    {
        if(moveDir != Vector2.zero)
        {
            bobEulorRot.x = multiplier.x * (Mathf.Sin(2 * speedCurve));
            bobEulorRot.y = multiplier.y * curveCos;
            bobEulorRot.z = multiplier.z * curveCos * moveDir.x;
        }
        else
        {
            bobEulorRot.x = multiplier.x * (Mathf.Sin(2 * speedCurve) / 2);
            bobEulorRot.y = 0;
            bobEulorRot.z = 0;
        }
    }


    public void conpositePositionRotation()
    {
        sb_apllier.transform.localPosition = Vector3.Lerp(sb_apllier.transform.localPosition, swayPos + bobPos, Time.deltaTime*smooth);
        sb_apllier.transform.localRotation = Quaternion.Slerp(sb_apllier.transform.localRotation, Quaternion.Euler(swayEulorRot) * Quaternion.Euler(bobEulorRot), Time.deltaTime*smoothRot);
    }

    //weapon nonFunctional events
    private void changeWeaponModel(int weaponIndex)
    {
        for(int i = 0; i < gc.getHandObject().transform.childCount; i++)
        {
            if (findWeapon(weaponIndex).WeaponName == gc.getHandObject().transform.GetChild(i).name)
            {
                if (activeWeaponParent.transform.childCount <= 0)
                {
                    gc.getHandObject().transform.GetChild(i).gameObject.SetActive(true);
                    gc.getHandObject().transform.GetChild(i).parent = activeWeaponParent.transform;
                    for (int j = 0; j < gc.getHandObject().transform.GetChild(i).childCount; j++)
                    {
                        if (gc.getHandObject().transform.GetChild(i).GetChild(j).name == "TargetR")
                        {
                            targetR.transform.position = gc.getHandObject().transform.GetChild(i).GetChild(j).position;
                            break;
                        }
                        else if (gc.getHandObject().transform.GetChild(i).GetChild(j).name == "TargetL")
                        {
                            break;
                        }
                    }

                }
                else
                {
                    GameObject go = activeWeaponParent.transform.GetChild(0).gameObject;



                    activeWeaponParent.transform.GetChild(0).parent = gc.getHandObject().transform;

                    go.SetActive(false);
                    gc.getHandObject().transform.GetChild(i).gameObject.SetActive(true);

                    gc.getHandObject().transform.GetChild(i).parent = activeWeaponParent.transform;

                    for (int j = 0; j < gc.getHandObject().transform.GetChild(i).childCount; j++)
                    {
                        if(gc.getHandObject().transform.GetChild(i).GetChild(j).name == "TargetR")
                        {
                            targetR.transform.position = gc.getHandObject().transform.GetChild(i).GetChild(j).position;
                            break;
                        }
                        else if (gc.getHandObject().transform.GetChild(i).GetChild(j).name == "TargetL")
                        {
                            break;
                        }
                    }


                }
                //gc.getHandObject().transform.GetChild(i).getComponent<Animator>().setBool("Silah«ekimi"))
            }
        }
    }
    private Weapon findWeapon(int searchIndex)
    {
        for(int i = 0; i < gc.weapons.Length; i++)
        {
            if (gc.weapons[i].WeaponTypeID == searchIndex)
            {
                return gc.weapons[i];
            }
        }
        Application.Quit();//make a error
        return null;
    }
    private weaponRuntimeHolder findWeaponOnRuntime(int searchIndex) 
    {
        for (int i = 0; i < holder.Length; i++)
        {
            if (gc.weapons[i].WeaponTypeID == searchIndex)
            {
                return holder[i];
            }
        }
        Application.Quit();
        return null;
    }
    IEnumerator recoilCoroutineDegree()
    {
        GameObject changerGORef = targetR;
        changerGORef = targetR;
        float degreOfRotation = findWeapon(currWeaponID).recoilDegree;
        changerGORef.transform.Rotate(+1 * Time.deltaTime * 100f, 0, 0, Space.Self);
        while (true)
        {
            if(changerGORef.transform.localEulerAngles.x < targetRNormalRot.x + degreOfRotation)
            {
                changerGORef.transform.Rotate(+1 * Time.deltaTime * 500f, 0, 0, Space.Self);
            }
            else
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("attackTimer between anim : " + attackTimer);
        float remainingDegreeToNormalize = MathF.Abs(changerGORef.transform.localEulerAngles.x - targetRNormalRot.x);
        int frameTime = (int)(attackTimer / Time.deltaTime);
        frameTime += 1;
        float velocityOfRotation = remainingDegreeToNormalize / frameTime;
        Debug.Log(velocityOfRotation);
        while (true)
        {
            if (frameTime <= 0)
            {
                break;
            }
            else
            {
                changerGORef.transform.Rotate(-velocityOfRotation * 2.3f, 0, 0, Space.Self);
            }
            if (remainingDegreeToNormalize > 0)
            {
            }
            else
            {
                break;
            }
            yield return new WaitForEndOfFrame();
            if (attackTimer <= attackTime / 2)
            {
                break;
            }
        }
        changerGORef.transform.localEulerAngles = targetRNormalRot;
    }
    IEnumerator recoilCoroutineKickback()
    {
        GameObject changerGORef = targetR;
        changerGORef.transform.localPosition = new Vector3(0, 0, targetRNormalPos.z - attackRecoil);
//        changerGORef.transform.Translate(new Vector3(0, -attackRecoil, 0),Space.Self);
//        changerGORef.transform.Translate(changerGORef.transform.parent.TransformDirection(0, 0, -attackRecoil), Space.Self);
        while (true)
        {
            changerGORef.transform.position = Vector3.MoveTowards(changerGORef.transform.position, changerGORef.transform.parent.TransformPoint(new Vector3(0, 0, targetRNormalPos.z)), Time.deltaTime);
            if (changerGORef.transform.position == changerGORef.transform.parent.TransformPoint(new Vector3(0, 0, targetRNormalPos.z)))
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
public class weaponRuntimeHolder
{
    public int weaponTypeID;
    public int ammoAmount = 0;
    public int magInAmmoAmount = 0;
    public int maxMagAmount = 0;
    public bool isOwned = false;
}
