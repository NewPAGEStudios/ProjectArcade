using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;

public class WeaponManager : MonoBehaviour
{
    //Reference
    private GameController gc;
    public Animator hand_Animator;
    public RayLine rl;

    [Header("Melee Attack Configiration")]
    public float meleeDmg;
    public float meleeForce;
    public float meleeCooldown;
    private bool power = false;
    [Header("Laser Settings")]
    public float laserDropRate;
    public float laserCharge = 0;
    public float maxLaserCharge;
    public float reqLaserCharge = 10;
    private bool laserFinished = false;

    //RuntimeObject
    private int currWeaponID = -1;
    private int currWeapon_sum_ammoAmount; //runtime ammo in Mag
    private int currWeapon_inWeapon_ammoAmount;

    private int magmax = -1;



    [Header("Sway N Bobbing")]
    [SerializeField]
    private float smooth = 10f;
    [SerializeField]
    private float smoothRot = 12f;

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
    public GameObject skillDisplay;

    [Header("Ref of Sway n Bob Apllier")]
    public GameObject sb_apllier;

    [Header("Active/Inactive Weapon's Refs")]
    public GameObject activeWeapon;
    public GameObject inActiveWeapon;
    private GameObject currentWeaponGO;
    [Header("Sounds")]
    public GameObject soundWeapon;
    [Header("Interaction")]
    public LayerMask lMaskOFInteraction;

    [HideInInspector]
    public Skill active_Skill;
    public List<SkillRuntimeHolder> stocked_Skills = new();
    [HideInInspector]
    public bool ot_event_skillMenuOpen = true;
    //ObjectWeaponRuntime
    public WeaponRuntimeHolder[] holder;


    private bool skill_usageCooldown;
    private bool skill_canbePerformed;
    private bool skill_holdOT;
    private Transform indicator_tf;
    private Vector3 hitnormal;

    [HideInInspector]
    public InputManager IManager;
    private PController player;
    [Header("VFX")]
    public GameObject handPower;

    private Coroutine meleeRoutine;
    //ActionStates
    private enum ActionStateOFHands
    {
        idle,
        inFire,
        onChange,
        inReload,
        inMelee,
    }
    ActionStateOFHands handStates;
    private bool onSkillUsage;
    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        player = GetComponent<PController>();
    }

    private void Start()
    {



        handStates = ActionStateOFHands.idle;
        //currentWeaponID init -for now we will use it on global variable init

        //weaponStart
        magmax = -1;
        currWeaponID = -1;
        //getWeapon(bombaGuy);
        //skill Initialization
        active_Skill = null;
        onSkillUsage = false;
        skill_holdOT = true;
        skill_usageCooldown = false;


        for(int i = 0; i< gc.skills.Length; i++)
        {
            stocked_Skills.Add(new SkillRuntimeHolder(gc.skills[i], 0));
        }

        laserCharge = maxLaserCharge;


    }




    private void Update()
    {
        if (gc.pState == GameController.PlayState.inPlayerInterrupt || gc.pState == GameController.PlayState.inCinematic || gc.state != GameController.GameState.inGame || player.ccstate != PController.CCStateOfPlayer.normal)
        {
            if (onSkillUsage)
            {
                skillCancel();
            }
            return;
        }
        //nonweaponedFunctiond
        //weaponedFunctions
        //reload
        if (IManager.getReloadPressed() || currWeapon_inWeapon_ammoAmount <= 0)
        {
            if (currWeaponID == -1)
            {
                return;
            }
            if (!FindWeapon(currWeaponID).reload)
            {
                currWeapon_inWeapon_ammoAmount = currWeapon_sum_ammoAmount;
            }
            else
            {
                if (handStates == ActionStateOFHands.idle)
                {
                    if (!onSkillUsage)
                    {
                        ReloadCheck();
                    }
                }
                else
                {
                }

            }
        }
        //fire
        if (currWeaponID >= 0)
        {
            if (FindWeapon(currWeaponID).type == Weapon.WeaponType.semi)
            {
                if (IManager.getFirePressed())
                {
                    Debug.Log("HandState: " + handStates);
                    if (handStates == ActionStateOFHands.idle)
                    {
                        if (FindWeapon(currWeaponID).isMelee)
                        {
                            Attack();

                        }
                        else
                        {
                            Fire();
                        }
                    }
                    else
                    {
                    }
                }
            }
            else if (FindWeapon(currWeaponID).type == Weapon.WeaponType.auto)
            {
                if (IManager.fireHolding)
                {
                    if (handStates == ActionStateOFHands.idle)
                    {
                        if (FindWeapon(currWeaponID).isMelee)
                        {
                            Attack();

                        }
                        else 
                        {
                            Fire();
                        }
                    }
                    else
                    {
                    }
                }
            }
            //weaponChange
            if (onSkillUsage)
            {
                if (IManager.getMouseScroll() > 0)
                {
                    if(active_Skill.st == Skill.skillType.active)
                    {
                        indicator_tf.localEulerAngles += new Vector3(0, 5 * 50 * Time.deltaTime, 0);
                    }
                }
                else if (IManager.getMouseScroll() < 0)
                {
                    if (active_Skill.st == Skill.skillType.active)
                    {
                        indicator_tf.localEulerAngles += new Vector3(0, -5 * 50 * Time.deltaTime, 0);
                    }
                }
            }
            else
            {
                if (IManager.getMouseScroll() < 0)
                {
                    ChangeWeapon(currWeaponID - 1);
                }
                else if (IManager.getMouseScroll() > 0)
                {
                    ChangeWeapon(currWeaponID + 1);
                }
            }
        }
        //MeleeAttack
        if (IManager.getMeleePressed())
        {
            if(meleeRoutine == null)
            {
                meleeRoutine = StartCoroutine(meleeAnimSync());
            }
        }
        //skillMenu
        if (IManager.skillMenu)
        {
            if (ot_event_skillMenuOpen)
            {
                ot_event_skillMenuOpen = false;
                gc.openSkillMenu();
            }
        }
        //skillUsing
        if (active_Skill != null)
        {
            if(handStates != ActionStateOFHands.inReload)
            {
                if (!skill_usageCooldown)
                {
                    if (Input.GetKey(KeyCode.Q))
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            skillCancel();
                        }
                        else
                        {
                            //visualize
                            skillStayOpen();

                        }
                    }
                    else if (Input.GetKeyUp(KeyCode.Q))
                    {
                        skill_usageCooldown = false;
                        skillPerform();
                    }
                }
                else
                {
                    if (Input.GetKeyUp(KeyCode.Q))
                    {
                        skill_usageCooldown = false;
                    }
                }
            }//HandState Control

        }
        if (Physics.Raycast(firePos.transform.parent.position, firePos.transform.forward, out RaycastHit hitInfo ,2f,lMaskOFInteraction,QueryTriggerInteraction.Collide))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(hitInfo.transform.TryGetComponent<Shop>(out Shop sh))
                {
                    gc.Interact(0, hitInfo.transform.gameObject);
                }
            }
            gc.DisplayInstruction(true,0);
        }
        else
        {
            if (!onSkillUsage)
            {
                gc.DisplayInstruction(false, 0);
            }
        }

    }
    private void FixedUpdate()
    {
        if (gc.pState == GameController.PlayState.inPlayerInterrupt || gc.pState == GameController.PlayState.inCinematic || gc.state == GameController.GameState.inShop || player.ccstate != PController.CCStateOfPlayer.normal)
        {
            return;
        }

    }
    public void startPowerfullAttack()
    {
        handPower.GetComponent<ColliderParenter>().targetOBJ.SetActive(true);
        power = true;
    }
    //laser
    public void addLaser(float amount)
    {
        if (laserFinished)
        {
            laserCharge += amount;
            if (laserCharge > reqLaserCharge)
            {
                laserFinished = false;
            }
        }

        if (laserCharge > maxLaserCharge)
        {
            laserCharge = maxLaserCharge;
        }
        gc.LaserIndicator(laserCharge, maxLaserCharge);
    }
    public void laserOpen()//CallsFrom fixed
    {
        if (gc.pState == GameController.PlayState.inPlayerInterrupt || gc.pState == GameController.PlayState.inCinematic || gc.state != GameController.GameState.inGame || player.ccstate != PController.CCStateOfPlayer.normal)
        {
            gc.laserIndicationCross(false);
            rl.gameObject.SetActive(false);
            return;
        }
        if(currWeaponID == -1)
        {
            gc.laserIndicationCross(false);
            rl.gameObject.SetActive(false);
            return;
        }
        if (handStates != ActionStateOFHands.idle && handStates != ActionStateOFHands.inFire)
        {
            gc.laserIndicationCross(false);
            rl.gameObject.SetActive(false);
            laserCharge += Time.fixedDeltaTime * laserDropRate / 4;
            if (laserCharge >= maxLaserCharge)
            {
                laserCharge = maxLaserCharge;
            }
            gc.LaserIndicator(maxLaserCharge, laserCharge);

            return;
        }
        if (laserFinished)
        {
            gc.laserIndicationCross(false);
            rl.gameObject.SetActive(false);
            laserCharge += Time.fixedDeltaTime * laserDropRate / 4;
            if (laserCharge >= reqLaserCharge)
            {
                laserFinished = false;
            }
            gc.LaserIndicator(maxLaserCharge, laserCharge);
            return;
        }
        if (IManager.laserOpen)
        {
            gc.laserIndicationCross();
            rl.gameObject.SetActive(true);
//            rl.ReflectLaser();
            laserCharge -= Time.fixedDeltaTime * laserDropRate;
            if (laserCharge <= 0)
            {
                laserCharge = 0;
                laserFinished = true;
                return;
            }
        }
        else
        {
            gc.laserIndicationCross(false);
            rl.gameObject.SetActive(false);
            laserCharge += Time.fixedDeltaTime * laserDropRate/4;
            if (laserCharge >= maxLaserCharge)
            {
                laserCharge = maxLaserCharge;
            }
        }
        gc.LaserIndicator(maxLaserCharge, laserCharge);
    }

    //Acquire weapon R ammo
    public void GetWeaponR(int weaponID)
    {
        if (weaponID >= gc.weapons.Length)//wil be deleted
        {
            return;
        }
        if (FindWeaponOnRuntime(weaponID).isOwned)
        {
            Debug.Log("You get " + weaponID + "id weapon Ammo");
            foreach (WeaponGetAmmoData_holder.weaponGetAmmoData_holder wgadh in gc.weaponGetAmmoData.weaponTypes)
            {
                if(wgadh.weaponID == weaponID)
                {
                    GetAmmo(weaponID, wgadh.weaponAmmoCount);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("You get " + weaponID + "id weapon");
            FindWeaponOnRuntime(weaponID).isOwned = true;
            foreach (WeaponGetAmmoData_holder.weaponGetAmmoData_holder wgadh in gc.weaponGetAmmoData.weaponTypes)
            {
                if (wgadh.weaponID == weaponID)
                {
                    GetAmmo(weaponID, wgadh.weaponAmmoCount);
                    break;
                }
            }
        }
        gc.notification(3, -1, null, FindWeapon(weaponID));
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
            gc.ChangeAmmoText(currWeapon_inWeapon_ammoAmount, currWeapon_sum_ammoAmount);
            return;
        }
        //delete Returns and show a feedback for getting ammo like sound or effect
    }
    public void ChangeWeapon(int weaponID)
    {
        Debug.Log("ChangeWeapon" + weaponID + "bomba " + currWeaponID);
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
        if (FindWeaponOnRuntime(weaponID).weaponTypeID == currWeaponID)//if it's the only weapon
        {
            return;
        }
        handStates = ActionStateOFHands.onChange;
        if (currWeaponID != -1)//if player  have more than 1 weapon
        {
            FindWeaponOnRuntime(currWeaponID).inWeapon_ammoAmount = currWeapon_inWeapon_ammoAmount;
            FindWeaponOnRuntime(currWeaponID).sum_ammoAmount = currWeapon_sum_ammoAmount;
            if (currentWeaponGO.TryGetComponent<Animator>(out Animator anim))
            {
                anim.SetTrigger("Interruption");
            }
        }
        else
        {
        }


        StopAllCoroutines();

        currWeapon_sum_ammoAmount = FindWeaponOnRuntime(weaponID).sum_ammoAmount;
        currWeapon_inWeapon_ammoAmount = FindWeaponOnRuntime(weaponID).inWeapon_ammoAmount;
        magmax = FindWeaponOnRuntime(weaponID).maxMagAmount;

        gc.ChangeAmmoText(currWeapon_inWeapon_ammoAmount, currWeapon_sum_ammoAmount);


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

    //Fire
    private void Attack()
    {
        Weapon w = FindWeapon(currWeaponID);
        if (currWeapon_inWeapon_ammoAmount <= 0)
        {
            //soundManager.noAmmoFX.Play()
            return;
        }
        handStates = ActionStateOFHands.inFire;

        currWeapon_inWeapon_ammoAmount -= w.usingAmmoPerAttack;
        currWeapon_sum_ammoAmount = currWeapon_inWeapon_ammoAmount;

        gc.ChangeAmmoText(currWeapon_inWeapon_ammoAmount, currWeapon_sum_ammoAmount);

        //if (soundWeapon.transform.Find(currWeaponID.ToString()).GetComponent<AudioSource>().isPlaying)
        //{
        //    soundWeapon.transform.Find(currWeaponID.ToString()).GetComponent<AudioSource>().Stop();
        //}
        //soundWeapon.transform.Find(currWeaponID.ToString()).GetComponent<AudioSource>().Play();

        if (!gc.tutOpened)
        {
            gc.statisticManager.incnumberOfFire();
        }

        //manuel Adding
        if(activeWeapon.transform.GetChild(0).TryGetComponent<SwordSlash>(out SwordSlash ss))
        {
            ss.handAnimator = hand_Animator;
            ss.weaponManager = this;
            Debug.Log("k�mn");
            ss.Attack();
        }
    }
    public void AttackEnded()
    {
        handStates = ActionStateOFHands.idle;
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

        if (FindWeapon(currWeaponID).twoStateReload)
        {
            activeWeapon.transform.GetChild(0).Find("ammo").GetComponent<TextMeshPro>().text = currWeapon_inWeapon_ammoAmount.ToString();
        }

        gc.ChangeAmmoText(currWeapon_inWeapon_ammoAmount, currWeapon_sum_ammoAmount);

        if (soundWeapon.transform.Find(currWeaponID.ToString()).GetComponent<AudioSource>().isPlaying)
        {
            soundWeapon.transform.Find(currWeaponID.ToString()).GetComponent<AudioSource>().Stop();
        }
        soundWeapon.transform.Find(currWeaponID.ToString()).GetComponent<AudioSource>().Play();


        if (!gc.tutOpened)
        {
            gc.statisticManager.incnumberOfFire();
        }
        //spawnBullet()
        GameObject ammo = new();
        ammo.transform.SetPositionAndRotation(firePos.transform.position, firePos.transform.rotation);
        ammo.name = "Ammo";
        ammo.layer = 7;
        System.Type scriptMB = System.Type.GetType(w.usedAmmo.functionName + ",Assembly-CSharp");
        ammo.AddComponent(scriptMB);
        
        //ManuelAdding
        if(ammo.TryGetComponent<ReflectBulletFunctions>(out ReflectBulletFunctions rbf))
        {
            rbf.modelMat = w.usedAmmo.materials;
            rbf.bulletSpeed = w.usedAmmo.bulletSpeed;
            rbf.mostHitCanBeDone = w.usedAmmo.maxReflectionTime;

            rbf.calcvec = firePos.transform.GetChild(0).transform.position;

            rbf.firedBy = gameObject;

            rbf.dmg = w.usedAmmo.dmg;

            rbf.trailType = w.usedAmmo.trail;

            rbf.layerMask = w.usedAmmo.lmask;

            rbf.trace = w.usedAmmo.wallTrace;

            GameObject go = Instantiate(w.usedAmmo.modelGO, ammo.transform);
            go.layer = 7;
        }
        else if(ammo.TryGetComponent<FastAmmoBulletFunction>(out FastAmmoBulletFunction fabf))
        {
            fabf.modelMat = w.usedAmmo.materials;
            fabf.bulletSpeed = w.usedAmmo.bulletSpeed;
            fabf.mostHitCanBeDone = w.usedAmmo.maxReflectionTime;

            fabf.calcvec = firePos.transform.GetChild(0).transform.position;

            fabf.firedBy = gameObject;

            fabf.dmg = w.usedAmmo.dmg;

            fabf.trailType = w.usedAmmo.trail;

            fabf.layerMask = w.usedAmmo.lmask;

            fabf.trace = w.usedAmmo.wallTrace;

            GameObject go = Instantiate(w.usedAmmo.modelGO, ammo.transform);
            go.layer = 7;

        }
        else
        {
            Debug.Log("Bomba!!!");
        }

//        Debug.Log("Current Ammo " + currWeapon_inWeapon_ammoAmount + "/FullMag " + magmax + "/Ammo Amount " + currWeapon_sum_ammoAmount);

        //recoil animation
        StartCoroutine(FireAnim());

    }
    IEnumerator FireAnim()
    {
        currentWeaponGO.transform.Find("Particle System").GetComponent<ParticleSystem>().Play();
        hand_Animator.SetBool("fired", true);
        yield return null;
        hand_Animator.SetBool("fired", false);
        
        while (true) 
        {
            yield return null;
            if (hand_Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_weap" + currWeaponID))
            {
                break;
            }
        }
        handStates = ActionStateOFHands.idle;
        yield return null;
    }
    
    //Reload
    private void ReloadCheck()
    {
        if (!FindWeapon(currWeaponID).reload)
        {
            return;
        }

        if (currWeapon_inWeapon_ammoAmount == magmax)
        {
//            Debug.Log("Full Ammo");
            return;
        }
        else if (currWeapon_sum_ammoAmount <= 0)
        {
 //           Debug.Log("You don't have enough ammo");
            return;
        }
        StartCoroutine(Reload());
    }
    IEnumerator Reload()
    {
        handStates = ActionStateOFHands.inReload;
        if (FindWeapon(currWeaponID).twoStateReload)
        {
            hand_Animator.SetBool("reload", true);
            while (true)
            {
                yield return null;
                if (hand_Animator.GetCurrentAnimatorStateInfo(0).IsName("reload1_weap" + currWeaponID))
                {
                    break;
                }
            }
            hand_Animator.SetBool("reload", false);
            ReloadFucntion();
            while (true)
            {
                yield return null;
                if (hand_Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_weap" + currWeaponID))
                {
                    break;
                }
            }
            handStates = ActionStateOFHands.idle;
            yield return null;
        }
        else
        {
            hand_Animator.SetBool("reload", true);
            activeWeapon.transform.GetChild(0).GetComponent<Animator>().SetTrigger("reload");
            yield return null;
            hand_Animator.SetBool("reload", false);
            while (true)
            {
                yield return null;
                if (hand_Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_weap" + currWeaponID))
                {
                    break;
                }
            }

            ReloadFucntion();
            handStates = ActionStateOFHands.idle;
            yield return null;
        }
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
        if (FindWeapon(currWeaponID).twoStateReload)
        {
            activeWeapon.transform.GetChild(0).Find("ammo").GetComponent<TextMeshPro>().text = currWeapon_inWeapon_ammoAmount.ToString();
        }
        gc.ChangeAmmoText(currWeapon_inWeapon_ammoAmount, currWeapon_sum_ammoAmount);
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
    public void BobOffset(PController player,Vector2 moveDir,float yjump)
    {
        if (player.actiontg == PController.ActionStateDependecyToGround.flat || player.actiontg == PController.ActionStateDependecyToGround.slope)
        {
            speedCurve += (Time.deltaTime * (moveDir.x + moveDir.y) * bobExaggeration) + 0.01f;
            bobPos.x = (CurveCos * bobLimit.x * 1) - (moveDir.x * travelLimit.x);
            bobPos.y = (CurveSin * bobLimit.y * 0) - (travelLimit.y);
        }
        else
        {
            speedCurve += (Time.deltaTime * 1) + 0.01f;
            bobPos.x = (CurveCos * bobLimit.x * 0) - (moveDir.x * travelLimit.x);
            bobPos.y = (bobLimit.y * yjump) - (travelLimit.y);
        }
        bobPos.z = - (moveDir.y * travelLimit.z);
    }

    public void BobRotation(Vector2 moveDir)
    {
        if(moveDir != Vector2.zero)
        {
            bobEulorRot.x = multiplier.x * Mathf.Sin(2 * speedCurve);
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

    //MeleAttack
    IEnumerator meleeAnimSync()
    {
        StartCoroutine(attackRoutine());
        handStates = ActionStateOFHands.inMelee;
        hand_Animator.SetTrigger("melee");
        while(true)
        {
            if (hand_Animator.GetCurrentAnimatorStateInfo(1).IsName("MeleeEnd"))
            {
                break;
            }
            yield return null;
        }
        MeleeAttack();
        handStates = ActionStateOFHands.idle;
        meleeRoutine = null;
    }
    public void MeleeAttack()
    {
        Ray ray = new Ray();
        ray.direction = firePos.transform.forward;
        ray.origin = firePos.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                if (power)
                {
                    //GetComponent<ColliderParenter>().targetOBJ.transform.parent
                    power = false;

                    handPower.GetComponent<ColliderParenter>().targetOBJ.SetActive(false);
                    hit.transform.GetComponent<Enemy>().blow(firePos.transform.forward + new Vector3(0, 1, 0), meleeForce * 10);
                    hit.transform.GetComponent<EnemyHealth>().EnemyHealthUpdate(-meleeDmg * 2, gameObject);
                }
                else
                {
                    //GetComponent<ColliderParenter>().targetOBJ.transform.parent
                    hit.transform.GetComponent<Enemy>().blow(firePos.transform.forward + new Vector3(0, 1, 0), meleeForce);
                    hit.transform.GetComponent<EnemyHealth>().EnemyHealthUpdate(-meleeDmg, gameObject);
                }
            }
        }
    }

    IEnumerator attackRoutine()
    {
        Time.timeScale = 0.6f;
        yield return new WaitForSecondsRealtime(0.2f);
        while (true)
        {
            Time.timeScale += 0.1f;
            if(Time.timeScale >= 1f)
            {
                Time.timeScale = 1;
                break;
            }
            yield return null;
        }
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
        currentWeaponGO = activeWeapon.transform.GetChild(0).gameObject;
        StartCoroutine(SheateAnim(weaponIndex));
        gc.ChangeActiveWeapon(weaponIndex);
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
        handStates = ActionStateOFHands.idle;
        yield return null;
    }

    //Skills
    public void getSkill(Skill sk)
    {
        int skillnm = stocked_Skills.IndexOf(stocked_Skills.Find(x => x.skill == sk));
        if (stocked_Skills[skillnm].count == 0)
        {
            active_Skill = sk;
            gc.changeSpriteOfActiveSkill(sk.sprite_HUD);
        }
        gc.notification(2, -1, sk, null);
        stocked_Skills[skillnm].count += 1;
        }
    public void changeSkills(int skill_id)
    {
        ot_event_skillMenuOpen = true;
        if (skill_id == -1)
        {
            return;
        }
        int skillnm = stocked_Skills.IndexOf(stocked_Skills.Find(x => x.skill.skillTypeID == skill_id));
        if (stocked_Skills[skillnm].count == 0)
        {
            return;
        }
        active_Skill = stocked_Skills[skillnm].skill;
        gc.changeSpriteOfActiveSkill(active_Skill.sprite_HUD);

    }
    private void skillStayOpen()
    {
        if (skill_holdOT)
        {
            gc.DisplayInstruction(true, 1);
            hand_Animator.SetTrigger("skill" + 0);//TODO:active_Skill.skillTypeID;
            if(active_Skill.st == Skill.skillType.passive)
            {

            }
            else
            {
                GameObject goParent = new();
                goParent.transform.parent = gc.skillIndicatorParent.transform;
                goParent.name = "indicatorParent";

                GameObject go = Instantiate(active_Skill.indicator, goParent.transform);
                go.name = "indicator";
                if (go.TryGetComponent<Collider>(out Collider col))
                {
                    col.enabled = false;
                }
                go.GetComponent<MeshRenderer>().material = gc.skillIndicatorMaterial;
                indicator_tf = go.transform;
            }
            onSkillUsage = true;
            skill_holdOT = false;
        }
        if(active_Skill.st == Skill.skillType.passive)
        {
            skillDisplay.SetActive(true);
            MaterialPropertyBlock m_propertyBlock = new();
            m_propertyBlock.SetTexture("_baseTexture", active_Skill.sprite_HUD.texture);
            skillDisplay.GetComponentInChildren<Renderer>().SetPropertyBlock(m_propertyBlock);
            skill_canbePerformed = true;
            return;
        }
        if (Physics.Raycast(firePos.transform.position, firePos.transform.forward,out RaycastHit hit,10f))
        {
            if(hit.transform.CompareTag("Ground"))
            {
                //indicator
                indicator_tf.parent.gameObject.SetActive(true);
                indicator_tf.parent.position = hit.point + new Vector3(0, 1, 0);
                indicator_tf.parent.forward = (indicator_tf.position - gameObject.transform.position).normalized;
                indicator_tf.parent.up = Vector3.up;

                if(active_Skill.skillTypeID == 0)
                {
                    indicator_tf.parent.eulerAngles = new Vector3(0, indicator_tf.eulerAngles.y, indicator_tf.eulerAngles.z);
                }

                skill_canbePerformed = true;
            }
            else
            {
                indicator_tf.parent.gameObject.SetActive(false);
                skill_canbePerformed = false;
            }
        }
        else
        {
            indicator_tf.parent.gameObject.SetActive(false);
            skill_canbePerformed = false;
        }
    }
    private void skillPerform()
    {
        gc.DisplayInstruction(false, 1);
        if (skill_canbePerformed)
        {
            skillDisplay.SetActive(false);
            if (active_Skill.st == Skill.skillType.passive)
            {
                System.Type script = System.Type.GetType(active_Skill.functionName + ",Assembly-CSharp");
                skillDisplay.AddComponent(script);
                if(skillDisplay.TryGetComponent<getSpeed>(out getSpeed gs))
                {
                    gs.doFunctionWoutObject();
                }
                else if(skillDisplay.TryGetComponent<extraJumpAdder>(out extraJumpAdder eja))
                {
                    eja.doFunctionWoutObject();
                }
                else if(skillDisplay.TryGetComponent<dashAdder>(out dashAdder da))
                {
                    da.doFunctionWoutObject();
                }
                else if(skillDisplay.TryGetComponent<healPlayer>(out healPlayer hp))
                {
                    hp.doFunctionWoutObject();
                }
                else if (skillDisplay.TryGetComponent<addLaser>(out addLaser al))
                {
                    al.doFunctionWoutObject();
                }
                else if (skillDisplay.TryGetComponent<gravityPull>(out gravityPull gp))
                {
                    gp.doFunctionWoutObject();
                }
                else if(skillDisplay.TryGetComponent<SuckerPunch>(out SuckerPunch sp))
                {
                    sp.doFunctionWoutObject();
                }
                else if(skillDisplay.TryGetComponent<laserEye>(out laserEye le))
                {
                    le.doFunctionWoutObject();
                }
                StartCoroutine(PerformSkillAnim(0));
            }
            else
            {
                indicator_tf.parent.gameObject.SetActive(false);
                GameObject skillOBJ = Instantiate(active_Skill.modelPrefab, indicator_tf.position, indicator_tf.rotation, gc.skillObject.transform);
                Destroy(indicator_tf.parent.gameObject);

                System.Type script = System.Type.GetType(active_Skill.functionName + ",Assembly-CSharp");
                skillOBJ.AddComponent(script);

                //manuel handling
                if (skillOBJ.TryGetComponent<wallriser>(out wallriser wr))
                {
                    wr.skill = active_Skill;
                    StartCoroutine(PerformSkillAnim(active_Skill.skillTypeID));
                }
                else if (skillOBJ.TryGetComponent<stunInstanSkill>(out stunInstanSkill sis))
                {
                    sis.thisSkilll = active_Skill;
                    StartCoroutine(PerformSkillAnim(0));
                }

            }
            skill_holdOT = true;

            int skillnm = stocked_Skills.IndexOf(stocked_Skills.Find(x => x.skill == active_Skill));

            stocked_Skills[skillnm].count -= 1;
            gc.notification(4, -1, stocked_Skills[skillnm].skill, null);
            if (stocked_Skills[skillnm].count > 0)
            {
                changeSkills(stocked_Skills[skillnm].skill.skillTypeID);
            }
            else
            {
                active_Skill = null;
                gc.closeSpriteOfActiveSkill(null);
                skillDisplay.SetActive(false);
            }
        }
        else
        {
            skillCancel();
        }
    }
    private void skillCancel()
    {
        gc.DisplayInstruction(false, 1);
        if (active_Skill.st == Skill.skillType.passive)
        {
            skillDisplay.SetActive(false);
        }
        else
        {
            Destroy(indicator_tf.parent.gameObject);
        }
        skill_canbePerformed = false;
        skill_usageCooldown = true;
        StartCoroutine(CancelSkillAnim(0));//TODO: active_Skill.skillTypeID;
        skill_holdOT = true;
        skillDisplay.SetActive(false);
    }
    //Animation
    public void doubleHandAnimation(int interaction)
    {
        StopAllCoroutines();

        hand_Animator.SetInteger("Interaction_id", interaction);
        hand_Animator.SetTrigger("interaction");
    }
    public void doubleHandAnimationStop()
    {
        hand_Animator.SetInteger("Interaction_id", -1);
    }
    //WallHack Skill
    private Coroutine eagleEyeRoutine = null;
    public void openEagleEye()
    {
        Debug.Log("kmb");
        if(eagleEyeRoutine != null)
        {
            StopCoroutine(eagleEyeRoutine);
        }
        eagleEyeRoutine = StartCoroutine(eagleEye());
    }
    IEnumerator eagleEye()
    {
        gc.changeStateOfWallhackSkill(true);
        float timer = 5f;
        while (true)
        {
            yield return null;

            if (gc.state != GameController.GameState.inGame || gc.pState == GameController.PlayState.inCinematic)
            {
                continue;
            }
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                break;
            }
        }
        gc.changeStateOfWallhackSkill(false);
    }

    IEnumerator CancelSkillAnim(int id)
    {
        onSkillUsage = false;
        hand_Animator.SetBool("cancel_skill" + id, true);
        yield return null;
        hand_Animator.SetBool("cancel_skill" + id, false);

    }
    IEnumerator PerformSkillAnim(int id)
    {
        onSkillUsage = false;
        hand_Animator.SetBool("perform_skill" + id,true);
        yield return null;
        hand_Animator.SetBool("perform_skill" + id, false);
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
            if (holder[i].weaponTypeID == searchIndex)
            {
                return holder[i];
            }
        }
        Application.Quit();
        return null;
    }
    public void LoadGameGetCurrentWeapon()
    {

    }
}
[System.Serializable]
public class WeaponRuntimeHolder
{
    public int weaponTypeID;
    public int sum_ammoAmount;
    public int inWeapon_ammoAmount;
    public int maxMagAmount;
    public bool isOwned;
    public WeaponRuntimeHolder(int weaponTypeID,int maxmagAmount)
    {
        this.weaponTypeID = weaponTypeID;
        this.sum_ammoAmount = 0;
        this.inWeapon_ammoAmount = 0;
        this.maxMagAmount = maxmagAmount;
        this.isOwned = false;
    }
}
[System.Serializable]
public class SkillRuntimeHolder
{
    public Skill skill;
    public int count;
    public SkillRuntimeHolder(Skill sk , int count)
    {
        skill = sk;
        this.count = count;
    }
}
