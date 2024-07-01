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
    private int currWeaponMag; //runtime ammo in Mag
    private int currWeaponAmmoAmount;

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
        if (/*currWeaponMag <= 0 || */Input.GetKeyDown(KeyCode.R))
        {
            if(handStates == ActionStateOFHands.idle)
            {
                StartCoroutine(Reload());
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ChangeWeapon(currWeaponID + 1);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if(handStates == ActionStateOFHands.idle)
            {
                Fire();

            }
        }
        //timer init

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
            FindWeaponOnRuntime(weaponTypeIndex).ammoAmount += ammoAmount;
            return;
        }
        else
        {
            currWeaponAmmoAmount += ammoAmount;
            gc.ChangeAmmoText(currWeaponAmmoAmount);
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
        if (currWeaponID != -1)
        {
            FindWeaponOnRuntime(currWeaponID).magInAmmoAmount = currWeaponMag;
            FindWeaponOnRuntime(currWeaponID).ammoAmount = currWeaponAmmoAmount;
        }

        currWeaponMag = FindWeaponOnRuntime(weaponID).magInAmmoAmount;
        currWeaponAmmoAmount = FindWeaponOnRuntime(weaponID).ammoAmount;
        magmax = FindWeaponOnRuntime(weaponID).maxMagAmount;

        gc.ChangeAmmoText(currWeaponMag);
        gc.ChangefullAmmoText(currWeaponAmmoAmount);


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
        if (currWeaponMag <= 0)
        {
            //soundManager.noAmmoFX.Play()
            return;
        }
        //Animator.SetBool("StartFire")
        Debug.Log("Weapon ID = " + currWeaponID + " -- Fired");
        currWeaponMag -= w.usingAmmoPerAttack;

        gc.ChangeAmmoText(currWeaponMag);
        gc.ChangefullAmmoText(currWeaponAmmoAmount);

        //spawnBullet()
        GameObject ammo = new();
        ammo.transform.SetPositionAndRotation(firePos.transform.position, firePos.transform.rotation);
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
        StartCoroutine(FireAnim());


        /*        StopCoroutine("recoilCoroutineKickback");
                StartCoroutine(recoilCoroutineDegree());
                StartCoroutine(recoilCoroutineKickback());*/
    }
    IEnumerator FireAnim()
    {
        hand_Animator.SetBool("fired", true);
        hand_Animator.SetBool("fired", false);
        while (true) 
        {
            if(hand_Animator.GetCurrentAnimatorStateInfo(0).IsName("idleweap" + currWeaponID))
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }


    IEnumerator Reload()
    {
        handStates = ActionStateOFHands.inReload;
        //block reload if there is not ammoAmount
        if (currWeaponAmmoAmount <= 0)
        {
            Debug.Log("You don't have enough ammo");
            StopCoroutine(Reload());
        }
        else if(currWeaponMag == magmax)
        {
            Debug.Log("Full Ammo");
            StopCoroutine(Reload());
        }
        hand_Animator.SetBool("reload", true);
        hand_Animator.SetBool("reload", false);
        while (true)
        {
            if(hand_Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_weap" + currWeaponID))
            {
                Debug.Log("Reload Anim");
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        ReloadFucntion();
        yield return null;
    }
    private void ReloadFucntion()
    {
        int toMakeFullMag = magmax - currWeaponMag;

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

        gc.ChangeAmmoText(currWeaponMag);
        gc.ChangefullAmmoText(currWeaponAmmoAmount);
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



                    activeWeapon.transform.GetChild(0).parent = inActiveWeapon.transform;

                    go.SetActive(false);
                    inActiveWeapon.transform.GetChild(i).gameObject.SetActive(true);

                    inActiveWeapon.transform.GetChild(i).parent = activeWeapon.transform;
                }
                //gc.getHandObject().transform.GetChild(i).getComponent<Animator>().setBool("Silah«ekimi"))
            }
        }
        //AnimationStart
        hand_Animator.SetTrigger("changeWeaponInterrupt");
        hand_Animator.SetInteger("weapon", weaponIndex);
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
    /*
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
    */
}
public class WeaponRuntimeHolder
{
    public int weaponTypeID;
    public int ammoAmount = 0;
    public int magInAmmoAmount = 0;
    public int maxMagAmount = 0;
    public bool isOwned = false;
}
