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
    public Animator hand_Animator;


    //RuntimeObject
    private int currWeaponID;
    private int currWeapon_sum_ammoAmount; //runtime ammo in Mag
    private int currWeapon_inWeapon_ammoAmount;

    private int magmax;


    //ObjectWeaponRuntime
    private WeaponRuntimeHolder[] holder;


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
    float CurveSin { get => Mathf.Sin(speedCurve); }
    float CurveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;

    [Header("FirePosition")]
    public GameObject firePos;

    [Header("Ref of Sway n Bob Apllier")]
    public GameObject sb_apllier;

    [Header("Active/Inactive Weapon's Refs")]
    public GameObject activeWeapon;
    public GameObject inActiveWeapon;

    //ActionStates
    private enum ActionStateOFHands
    {
        idle,
        inFire,
        onChange,
        inReload,
    }
    ActionStateOFHands handStates;



    private void Start()
    {
        handStates = ActionStateOFHands.idle;
        //init
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();


        //objectInit
        currentHP = startHP;
        
        //holder init
        holder = new WeaponRuntimeHolder[gc.weapons.Length];
        for (int i = 0; i < holder.Length; i++)
        {
            holder[i] = new WeaponRuntimeHolder
            {
                weaponTypeID = gc.weapons[i].WeaponTypeID,
                maxMagAmount = gc.weapons[i].magSize,
                isOwned = false
            };
        }
        //currentWeaponID init -for now we will use it on global variable init

        //weaponStart
        magmax = -1;
        currWeaponID = -1;
        //getWeapon(bombaGuy);

    }
    private void Update()
    {
        //nonweaponedFunctiond
        if (currWeaponID == -1)
        {
            return;
        }
        //weaponedFunctions
        //reload
        if (/*currWeaponMag <= 0 || */Input.GetKeyDown(KeyCode.R))
        {
            if(handStates == ActionStateOFHands.idle)
            {
                ReloadCheck();
            }
            else
            {
                Debug.Log("calledREALOAD--> " + handStates);
            }
        }
        //fire
        if (FindWeapon(currWeaponID).type == Weapon.WeaponType.semi)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (handStates == ActionStateOFHands.idle)
                {
                    Fire();
                }
                else
                {
//                    Debug.Log("calledFIRE--> " + handStates);
                }
            }
        }
        else if(FindWeapon(currWeaponID).type == Weapon.WeaponType.auto)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (handStates == ActionStateOFHands.idle)
                {
                    Fire();
                }
                else
                {
                    Debug.Log("calledFIRE--> " + handStates);
                }
            }
        }
        //weaponChange
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ChangeWeapon(currWeaponID + 1);
        }
    }

    public void TakeDMG(float dmgAmmount, GameObject dmgTakenFrom)
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
    public void GetWeapon(int weaponID)
    {
        if (weaponID >= gc.weapons.Length)//wil be deleted
        {
            return;
        }
        if (FindWeaponOnRuntime(weaponID).isOwned)
        {
            Debug.Log("You get " + weaponID + "id weapon Ammo");
            GetAmmo(weaponID,FindWeapon(weaponID).magSize);
        }
        else
        {
            Debug.Log("You get " + weaponID + "id weapon");
            FindWeaponOnRuntime(weaponID).isOwned = true;
            GetAmmo(weaponID, FindWeapon(weaponID).magSize);
        }
        if (currWeaponID == -1)
        {
            ChangeWeapon(weaponID);
        }
    }
    public void GetAmmo(int weaponTypeIndex, int ammoAmount)
    {
        if (weaponTypeIndex != currWeaponID)
        {
            FindWeaponOnRuntime(weaponTypeIndex).sum_ammoAmount += ammoAmount;
            return;
        }
        else
        {

            currWeapon_sum_ammoAmount += ammoAmount;
            gc.ChangefullAmmoText(currWeapon_sum_ammoAmount);
            return;
        }
        //delete Returns and show a feedback for getting ammo like sound or effect
    }
    private void ChangeWeapon(int weaponID)
    {
        if (weaponID >= gc.weapons.Length)
        {
            weaponID = 0;
        }
        else if (weaponID < 0)
        {
            weaponID = gc.weapons.Length - 1;
        }
        while (!FindWeaponOnRuntime(weaponID).isOwned)
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
        if (FindWeaponOnRuntime(weaponID).weaponTypeID == currWeaponID)
        {
            Debug.Log("You only have current weapon");
            return;
        }
        handStates = ActionStateOFHands.onChange;
        if (currWeaponID != -1)
        {
            FindWeaponOnRuntime(currWeaponID).inWeapon_ammoAmount = currWeapon_inWeapon_ammoAmount;
            FindWeaponOnRuntime(currWeaponID).sum_ammoAmount = currWeapon_sum_ammoAmount;
        }

        currWeapon_sum_ammoAmount = FindWeaponOnRuntime(weaponID).sum_ammoAmount;
        currWeapon_inWeapon_ammoAmount = FindWeaponOnRuntime(weaponID).inWeapon_ammoAmount;
        magmax = FindWeaponOnRuntime(weaponID).maxMagAmount;

        gc.ChangeAmmoText(currWeapon_inWeapon_ammoAmount);
        gc.ChangefullAmmoText(currWeapon_sum_ammoAmount);


        if (currWeaponID == -1)//special state it means you don't have any weapon
        {
            ChangeWeaponModel(weaponID);
        }
        else
        {
            ChangeWeaponModel(weaponID);
        }
        currWeaponID = weaponID;
    }

    private void Fire()
    {
        Weapon w = FindWeapon(currWeaponID);
        if (currWeapon_inWeapon_ammoAmount <= 0)
        {
            //soundManager.noAmmoFX.Play()
            return;
        }
        handStates = ActionStateOFHands.inFire;

        currWeapon_inWeapon_ammoAmount -= w.usingAmmoPerAttack;

        gc.ChangeAmmoText(currWeapon_inWeapon_ammoAmount);
        gc.ChangefullAmmoText(currWeapon_sum_ammoAmount);

        //spawnBullet()
        GameObject ammo = new();
        ammo.transform.SetPositionAndRotation(firePos.transform.position, firePos.transform.rotation);
        ammo.name = "Ammo";
        ammo.AddComponent(w.usedAmmo.function.GetClass());
        
        ammo.AddComponent<Rigidbody>();
        ammo.GetComponent<Rigidbody>().useGravity = false;
        ammo.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

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

        Debug.Log("Current Ammo " + currWeapon_inWeapon_ammoAmount + "/FullMag " + magmax + "/Ammo Amount " + currWeapon_sum_ammoAmount);

        //recoil animation
        StartCoroutine(FireAnim());

    }
    IEnumerator FireAnim()
    {
        hand_Animator.SetBool("fired", true);
        yield return new WaitForSeconds(0.01f);
        hand_Animator.SetBool("fired", false);
        
        while (true) 
        {
            yield return new WaitForEndOfFrame();
            if (hand_Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_weap" + currWeaponID))
            {
                break;
            }
        }
        Debug.Log("hokro");
        handStates = ActionStateOFHands.idle;
        yield return null;
    }

    private void ReloadCheck()
    {
        if (currWeapon_inWeapon_ammoAmount == magmax)
        {
            Debug.Log("Full Ammo");
            return;
        }
        else if (currWeapon_sum_ammoAmount <= 0)
        {
            Debug.Log("You don't have enough ammo");
            return;
        }
        StartCoroutine(Reload());
    }
    IEnumerator Reload()
    {
        handStates = ActionStateOFHands.inReload;

        hand_Animator.SetBool("reload", true);
        yield return new WaitForSeconds(0.01f);
        hand_Animator.SetBool("reload", false);
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (hand_Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_weap" + currWeaponID))
            {
                break;
            }
        }

        ReloadFucntion();
        yield return null;
    }
    private void ReloadFucntion()
    {
        int toMakeFullMag = magmax - currWeapon_inWeapon_ammoAmount;

        if (toMakeFullMag <= currWeapon_sum_ammoAmount)
        {
            currWeapon_inWeapon_ammoAmount = magmax;
            currWeapon_sum_ammoAmount -= toMakeFullMag;
        }
        else
        {
            currWeapon_inWeapon_ammoAmount += currWeapon_sum_ammoAmount;
            currWeapon_sum_ammoAmount = 0;
        }

        gc.ChangeAmmoText(currWeapon_inWeapon_ammoAmount);
        gc.ChangefullAmmoText(currWeapon_sum_ammoAmount);
        handStates = ActionStateOFHands.idle;
    }
    //swayNBobbing
    public void Sway(Vector3 inputCam)
    {
        Vector3 invertLook = inputCam * -0.01f;
        invertLook.x = Mathf.Clamp(invertLook.x, -0.06f, 0.06f);
        invertLook.y = Mathf.Clamp(invertLook.y, -0.06f, 0.06f);
    
        swayPos = invertLook;
    }
    public void SwayRotation(Vector3 inputCam)
    {
        Vector2 invertLook = inputCam * -4f;
        invertLook.x = Mathf.Clamp(invertLook.x, -5f, 5f);
        invertLook.y = Mathf.Clamp(invertLook.y, -5f, 5f);

        swayEulorRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }
    public void BobOffset(PController player,Vector2 moveDir)
    {
        if (player.actiontg == PController.ActionStateDependecyToGround.flat || player.actiontg == PController.ActionStateDependecyToGround.slope)
        {
            speedCurve += (Time.deltaTime * (moveDir.x + moveDir.y) * bobExaggeration) + 0.01f;
            bobPos.x = (CurveCos * bobLimit.x * 1) - (moveDir.x * travelLimit.x);
        }
        else
        {
            speedCurve += (Time.deltaTime * 1) + 0.01f;
            bobPos.x = (CurveCos * bobLimit.x * 0) - (moveDir.x * travelLimit.x);
        }
        bobPos.y = (CurveSin * bobLimit.y) - (moveDir.y * travelLimit.y);
        bobPos.z = -(moveDir.y * travelLimit.z);
    }
    public void BobRotation(Vector2 moveDir)
    {
        if(moveDir != Vector2.zero)
        {
            bobEulorRot.x = multiplier.x * (Mathf.Sin(2 * speedCurve));
            bobEulorRot.y = multiplier.y * CurveCos;
            bobEulorRot.z = multiplier.z * CurveCos * moveDir.x;
        }
        else
        {
            bobEulorRot.x = multiplier.x * (Mathf.Sin(2 * speedCurve) / 2);
            bobEulorRot.y = 0;
            bobEulorRot.z = 0;
        }
    }


    public void ConpositePositionRotation()
    {
        sb_apllier.transform.SetLocalPositionAndRotation(Vector3.Lerp(sb_apllier.transform.localPosition, swayPos + bobPos, Time.deltaTime*smooth), Quaternion.Slerp(sb_apllier.transform.localRotation, Quaternion.Euler(swayEulorRot) * Quaternion.Euler(bobEulorRot), Time.deltaTime*smoothRot));
    }

    //weapon nonFunctional events
    private void ChangeWeaponModel(int weaponIndex)
    {
        for(int i = 0; i < inActiveWeapon.transform.childCount; i++)
        {
            if (FindWeapon(weaponIndex).WeaponName == inActiveWeapon.transform.GetChild(i).name)
            {
                if (activeWeapon.transform.childCount <= 0)
                {
                    inActiveWeapon.transform.GetChild(i).gameObject.SetActive(true);
                    inActiveWeapon.transform.GetChild(i).parent = activeWeapon.transform;
                }
                else
                {
                    GameObject go = activeWeapon.transform.GetChild(0).gameObject;
                    activeWeapon.transform.localPosition = Vector3.zero;
                    activeWeapon.transform.localEulerAngles = Vector3.zero;


                    activeWeapon.transform.GetChild(0).parent = inActiveWeapon.transform;

                    go.SetActive(false);
                    inActiveWeapon.transform.GetChild(i).gameObject.SetActive(true);

                    inActiveWeapon.transform.GetChild(i).parent = activeWeapon.transform;
                }
            }
        }
        StartCoroutine(SheateAnim(weaponIndex));
        //AnimationStart
    }
    IEnumerator SheateAnim(int id)
    {
        hand_Animator.SetTrigger("changeWeaponInterrupt");
        hand_Animator.SetInteger("weapon", id);
        while (true) 
        {
            yield return new WaitForEndOfFrame();
            if (hand_Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_weap" + id))
            {
                break;
            }
        }
        Debug.Log(handStates);
        handStates = ActionStateOFHands.idle;
        yield return null;
    }
    private Weapon FindWeapon(int searchIndex)
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
    private WeaponRuntimeHolder FindWeaponOnRuntime(int searchIndex) 
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
}
public class WeaponRuntimeHolder
{
    public int weaponTypeID;
    public int sum_ammoAmount = 0;
    public int inWeapon_ammoAmount = 0;
    public int maxMagAmount = 0;
    public bool isOwned = false;
}