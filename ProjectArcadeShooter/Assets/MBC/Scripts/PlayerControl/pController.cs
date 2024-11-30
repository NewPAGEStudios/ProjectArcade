using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PController : MonoBehaviour
{
    [Header(header: "Health Configiration")]
    [SerializeField]
    private float maxHP;

    private float cHP;
    public float currentHP { get => cHP; set => cHP = value; }


    [Header(header: "Speed Configiration")]
    private float maxSpeed;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float moveDrag;

    [Header(header: "SpeedLimit Configiration")]
    [SerializeField]
    private float maxSpeedOnGround;
    [SerializeField]
    private float maxSpeedOutGround;

    [Header(header: "Air Configiration")]
    [SerializeField]
    private float moveSpeedOnAir;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpDrag;

    [Header(header: "Crouch Configiration")]
    [SerializeField]
    private float crouchSpeed;

    [Header(header:"Slide/Dash Configiration")]
    [SerializeField]
    private float slideDuration;
    [SerializeField]
    private float dashForce;
    [SerializeField]
    private float maxdashmeter;
    private float cdm;
    public float currentdashMeter {  get => cdm; set => cdm = value; }
    [SerializeField]
    private float dashslideadder;

    private InputManager iManager;
    private Rigidbody rb;

    private bool crouchEvent = true;

    private float targetScale;
    private float targetPosY;
    private Vector3 scale;
    private Vector3 pos;


    private Vector3 slopePlaneNormal = Vector3.up;


    [Header(header: "Slope Configiration")]
    [SerializeField]
    private float max_angleOfPlane;

    [Header(header: "Cam/Look Configiration")]
    [SerializeField]
    private float sensX;
    private float sensY;
    [SerializeField]
    private Camera mainCam;
    private Vector3 cam_StartingRotation;
    [Header(header: "SoundsParent Objects")]
    public GameObject soundParent;
    private bool snb;
    private bool dv;
    [Header("Effects Ref")]
    public GameObject dashEffect;
    private CameraPOVExtension cpove;

    GameController gc;
    WeaponManager weaponManager;

    //skill variables
    private int extrajump;
    private float targetFOV;
    private float targetFOVnormal;
    //Coroutines
    private Coroutine speedEfect;
    //Timer
    private float moveTimerSound=0.5f;

    public enum ActionStateDependecyToPlayer
    {
        idle,
        crouch,
        slide,
        dash,
    }
    public enum ActionStateDependecyToGround
    {
        flat,
        slope,
        onAir,
    }
    public enum CCStateOfPlayer
    {
        normal,
        ccd,
    }
    [HideInInspector]
    public CCStateOfPlayer ccstate;
    [HideInInspector]
    public ActionStateDependecyToPlayer actiontp;
    [HideInInspector]
    public ActionStateDependecyToGround actiontg;



    private void Awake()
    {
        //REFERANCES

        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        weaponManager = GetComponent<WeaponManager>();

    }

    // Start is called before the first frame update
    void Start()
    {
        iManager = InputManager.Instance;
        rb = GetComponent<Rigidbody>();

        weaponManager.IManager = iManager;
        gc.IManager = iManager;

        sensX = PlayerPrefs.GetFloat("XSensitivity", 10f) + 1;
        sensY = PlayerPrefs.GetFloat("YSensitivity", 10f) + 1;

        snb = PlayerPrefs.GetInt("SwayNBobbing", 1) == 1 ? true : false;
        dv = PlayerPrefs.GetInt("DMGVibration", 1) == 1 ? true : false;


//        gc.AddDashIndicator(maxdashmeter);
        currentdashMeter = maxdashmeter;
        gc.DashIndicator(currentdashMeter);

    }

    // Update is called once per frame
    void Update()
    {
        if(gc.pState==GameController.PlayState.inPlayerInterrupt||gc.pState == GameController.PlayState.inCinematic || gc.state==GameController.GameState.inShop || ccstate != CCStateOfPlayer.normal)
        {
            return;
        }
        if (currentdashMeter < maxdashmeter)
        {
            currentdashMeter += dashslideadder * Time.deltaTime;
            gc.DashIndicator(currentdashMeter);
        }



        Jump();
        Crouch();
        CrouchExit();
        Dashing();

        if (snb)//apply sway
        {
            weaponManager.Sway(iManager.getCameraMovement());
            weaponManager.SwayRotation(iManager.getCameraMovement());
            weaponManager.ConpositePositionRotation();
        }

        CheckGround();

    }

    private void FixedUpdate()
    {
        if (gc.pState == GameController.PlayState.inPlayerInterrupt || gc.pState == GameController.PlayState.inCinematic || gc.state == GameController.GameState.inShop || ccstate != CCStateOfPlayer.normal)
        {
            return;
        }

        Move();
        CamRotation();

        weaponManager.laserOpen();

        if (Input.GetKeyDown(KeyCode.P))
        {
            rb.AddForce(-gameObject.transform.forward * 2, ForceMode.VelocityChange);
        }
    }
    private void CamRotation()//tmmland�
    {
        if (cam_StartingRotation == null)
        {
            cam_StartingRotation = mainCam.transform.localRotation.eulerAngles;
        }
        Vector2 deltaInput = iManager.getCameraMovement();
        cam_StartingRotation.x += deltaInput.x * Time.deltaTime * sensX;
        cam_StartingRotation.y += deltaInput.y * Time.deltaTime * sensY;
        cam_StartingRotation.y = Mathf.Clamp(cam_StartingRotation.y, -60, 60);
        mainCam.transform.localRotation = Quaternion.Euler(-cam_StartingRotation.y, 0f, 0f);
        gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.x, cam_StartingRotation.x, gameObject.transform.localRotation.z);
        //compass Visualize



        //weaponProcedural Anim
        weaponManager.Sway(deltaInput);
        weaponManager.SwayRotation(deltaInput);
    }
    private void Jump()
    {
        if(iManager.getJumpedPressed())
        {
            if (actiontp != ActionStateDependecyToPlayer.idle)
            {
                return;
            }
            if(actiontg == ActionStateDependecyToGround.onAir && extrajump <= 0)
            {
                return;
            }
            switch (actiontg)
            {
                case ActionStateDependecyToGround.flat:
                    break;
                case ActionStateDependecyToGround.slope:
                    break;
                case ActionStateDependecyToGround.onAir:
                    extrajump -= 1;
                    break;
                default:break;
            }
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    //EğilmeBaşla
    private void Crouch()
    {
        //iManager.crouching;
        if(iManager.crouching)
        {
            Transform targetScaleExchangeUnit = gameObject.transform.GetChild(0);//getchild(0) = firts child of hierarcy it will be model be sure to do that
            Transform targetPosExchangeUnit = gameObject.transform;
            if (actiontp == ActionStateDependecyToPlayer.slide ||actiontp == ActionStateDependecyToPlayer.dash)
            {
                return;
            }
            if (crouchEvent)
            {
                scale = targetScaleExchangeUnit.localScale;
                pos = targetPosExchangeUnit.position;
                /*
                gameObject.transform.localScale = new Vector3(scale.x, scale.y / 2, scale.z);
                gameObject.transform.position = new Vector3(pos.x, pos.y - scale.y / 2, pos.z);
                */
                targetPosY = pos.y - scale.y / 2;
                targetScale = scale.y / 2;
                moveSpeed /= 2;
                crouchEvent = false;
            }
            if(pos.y == targetPosY)
            {
                actiontp = ActionStateDependecyToPlayer.crouch;
                return;
            }
            //scale
            scale.y = Mathf.MoveTowards(scale.y, targetScale, Time.deltaTime * crouchSpeed);
            targetScaleExchangeUnit.localScale = scale;
            //position
            pos.y = Mathf.MoveTowards(pos.y, targetPosY, Time.deltaTime * crouchSpeed);

            targetPosExchangeUnit.position = new Vector3(targetPosExchangeUnit.position.x, pos.y, targetPosExchangeUnit.position.z);
        }
    }
    private void CrouchExit()
    {
        if (!crouchEvent)
        {
            if (!iManager.crouching)
            {
                Transform targetScaleExchangeUnit = gameObject.transform.GetChild(0);//getchild(0) = firts child of hierarcy it will be model be sure to do that
                Transform targetPosExchangeUnit = gameObject.transform;

                scale = targetScaleExchangeUnit.localScale;
                pos = targetPosExchangeUnit.position;
                /*
                gameObject.transform.position = new Vector3(pos.x, pos.y + scale.y, pos.z);
                gameObject.transform.localScale = new Vector3(scale.x, scale.y * 2, scale.z);
                */
                targetPosY = pos.y + scale.y;
                targetScale = scale.y * 2;
                moveSpeed *= 2;
                StartCoroutine(CrouchExitEvent(targetScale, targetPosY));
                crouchEvent = true;
            }
        }
    }
    IEnumerator CrouchExitEvent(float targetScale,float targetPos)
    {
        Transform targetScaleExchangeUnit = gameObject.transform.GetChild(0);//getchild(0) = firts child of hierarcy it will be model be sure to do that
        Transform targetPosExchangeUnit = gameObject.transform;
        while (true)
        {

            //position
            pos.y = Mathf.MoveTowards(pos.y, targetPos, Time.deltaTime * crouchSpeed);
            targetPosExchangeUnit.position = new Vector3(targetPosExchangeUnit.position.x, pos.y, targetPosExchangeUnit.position.z);

            //scale
            scale.y = Mathf.MoveTowards(scale.y, targetScale, Time.deltaTime * crouchSpeed);
            targetScaleExchangeUnit.localScale = scale;
            if (scale.y >= targetScale && pos.y >= targetPos)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        actiontp = ActionStateDependecyToPlayer.idle;

        Camera.main.transform.localPosition = new Vector3(0, 1.35f, 0);
        transform.GetChild(0).localScale = new Vector3(1, 1.35f, 1);

    }
    //EĞilme Bit

    private void Move()
    {
        if(actiontp == ActionStateDependecyToPlayer.slide || actiontp == ActionStateDependecyToPlayer.dash)
        {//prevent movement
            return;
        }
        Vector2 moveDirVect2 = iManager.getPlayerMovement();// (x,y) (x,y,z) (x,0,y)
        if (snb)//apply bob
        {
            weaponManager.BobOffset(this, moveDirVect2,rb.velocity.y);
            weaponManager.BobRotation(moveDirVect2);
        }



        if (moveDirVect2 == Vector2.zero)
        {
            return;
        }


        Vector3 moveDir = new(moveDirVect2.x, 0f, moveDirVect2.y);

        moveDir = gameObject.transform.right * moveDir.x + gameObject.transform.forward * moveDir.z;
        switch (actiontp)
        {
            case ActionStateDependecyToPlayer.idle:
                break;
            case ActionStateDependecyToPlayer.crouch:
                break;
            default:
                Debug.LogWarning("action State to Player Sıkıntılı");
                break;
        }
        switch (actiontg)
        {
            case ActionStateDependecyToGround.flat:
                rb.AddForce(moveSpeed * Time.fixedDeltaTime * moveDir, ForceMode.Acceleration);
                break;
            case ActionStateDependecyToGround.slope:
                Vector3 sloped = Slope(moveDir);
                rb.AddForce(moveSpeed * Time.fixedDeltaTime * sloped, ForceMode.Acceleration);
                break;
            case ActionStateDependecyToGround.onAir:
                rb.AddForce(moveSpeedOnAir * Time.fixedDeltaTime * moveDir, ForceMode.Acceleration);
                break;
            default:
                Debug.LogWarning("action State to Ground Sıkıntılı");
                break;
        }


        Vector3 temp = new(rb.velocity.x, rb.velocity.y, rb.velocity.z);


        float speedTemp = temp.magnitude;

        if (actiontg != ActionStateDependecyToGround.onAir)
        {
            if (moveTimerSound <= 0)
            {
                soundParent.transform.Find("Footsteps").GetComponent<AudioSource>().Play();
                moveTimerSound = 0.5f;
            }
            else
            {
                moveTimerSound -= Time.fixedDeltaTime;
            }
        }
        if(actiontg == ActionStateDependecyToGround.onAir)
        {
            if(speedTemp > maxSpeed)
            {

            }
        }

        if (speedTemp > maxSpeed)
        {
            Vector3 liimitedVec = temp.normalized * maxSpeed;

            switch (actiontg)
            {
                case ActionStateDependecyToGround.slope:
                    rb.velocity = new Vector3(liimitedVec.x, liimitedVec.y, liimitedVec.z);
                    break;
                case ActionStateDependecyToGround.flat:
                    rb.velocity = new Vector3(liimitedVec.x, liimitedVec.y, liimitedVec.z);
                    break;
                case ActionStateDependecyToGround.onAir:
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
                    break;
                default:
                    rb.velocity = new Vector3(liimitedVec.x, rb.velocity.y, liimitedVec.z);
                    break;

            }
        }
    }


    private void Dashing()
    {
        if (iManager.getDashPressed())
        {
            if(actiontp == ActionStateDependecyToPlayer.slide || actiontp == ActionStateDependecyToPlayer.crouch)
            {
                return;
            }
            if (currentdashMeter < 25)
            {
                return;
            }
            Vector2 moveDirVect2 = iManager.getPlayerMovement();// (x,y) (x,y,z) (x,0,y)
            if (moveDirVect2 == Vector2.zero)
            {
                return;
            }

            actiontp = ActionStateDependecyToPlayer.dash;

            currentdashMeter -= 25;
            gc.DashIndicator(currentdashMeter);

            Vector3 moveDir = new(moveDirVect2.x, 0f, moveDirVect2.y);
            moveDir = gameObject.transform.right * moveDir.x + gameObject.transform.forward * moveDir.z;

            if(moveDirVect2.x > 0f)
            {
                dashEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            } 
            else if(moveDirVect2.x < 0f)
            {
                dashEffect.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            }
            if(moveDirVect2.y > 0f)
            {
                dashEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            }
            else if (moveDirVect2.y < 0f)
            {
                dashEffect.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
            }

            switch (actiontg)
            {
                case ActionStateDependecyToGround.flat:
                    rb.AddForce(moveDir * dashForce, ForceMode.VelocityChange);
                    break;
                case ActionStateDependecyToGround.slope:
                    rb.AddForce(Slope(moveDir) * dashForce, ForceMode.VelocityChange);
                    break;
                case ActionStateDependecyToGround.onAir:
                    rb.AddForce(moveDir * dashForce, ForceMode.VelocityChange);
                    break;
            }
            gc.DashEffectOpener(slideDuration - 0.03f);
            StartCoroutine(DashingNormal(slideDuration - 0.03f, moveDir));
        }
    }
    IEnumerator DashingNormal(float remainingTime,Vector3 moveDir)
    {
        yield return new WaitForSeconds(remainingTime);
        rb.velocity = moveDir*10f;
        actiontp = ActionStateDependecyToPlayer.idle;
    }
    private Vector3 Slope(Vector3 directionOnNormalPlane)
    {
        return Vector3.ProjectOnPlane(directionOnNormalPlane, slopePlaneNormal);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.layer == 7)//7 is ammo
        {
            return;
        }
        else if (collision.gameObject.layer == 9)
        {
            //manuelAdding
            if(collision.transform.parent.parent.gameObject.TryGetComponent<DummyMummyFunc>(out DummyMummyFunc dmf))
            {

                if (dmf.meleeDMG_Activated)
                {
                    TakeDMG(dmf.meleeDMG, dmf.gameObject);
                    ThrowPlayer(dmf.gameObject.transform.position);
                }
            }
        }
    }

    private void CheckGround()
    {
        float offsetScale = gameObject.transform.localScale.y;
        Vector3 checkGroundVector = Vector3.zero;
        if (actiontg == ActionStateDependecyToGround.onAir)
        {
            checkGroundVector = -transform.up.normalized;
        }
        else
        {
            checkGroundVector = -slopePlaneNormal.normalized;
        }
        Debug.DrawRay(gameObject.transform.position, checkGroundVector * (offsetScale + 0.75f), Color.red, Time.deltaTime);
        if (Physics.Raycast(gameObject.transform.position, checkGroundVector,out RaycastHit hit,offsetScale + 0.75f))
        {
            float angleOfPlane = Vector3.Angle(Vector3.up, hit.normal);

            rb.drag = moveDrag;
            maxSpeed = maxSpeedOnGround;

            if (angleOfPlane > max_angleOfPlane || angleOfPlane == 0)
            {
                actiontg = ActionStateDependecyToGround.flat;

                slopePlaneNormal = Vector3.up;
                rb.useGravity = true;
            }
            else
            {//is Slope granted
                actiontg = ActionStateDependecyToGround.slope;
                slopePlaneNormal = hit.normal;
                rb.useGravity = false;
            }

        }
        else
        {
            actiontg = ActionStateDependecyToGround.onAir;
            rb.drag = jumpDrag;
            rb.useGravity = true;
            maxSpeed = maxSpeedOutGround;
        }
    }


    public InputManager getIManager()
    {
        return iManager;
    }




    //CC Handling
    public void ThrowPlayer(Vector3 source)
    {
        Vector3 throwDirection = gameObject.transform.position - source;
        throwDirection = throwDirection.normalized;
        rb.AddForce(throwDirection * 15f, ForceMode.Impulse);
        StartCoroutine(cced(CCStateOfPlayer.ccd, 0.6f));
    }


    IEnumerator cced(CCStateOfPlayer cc ,float duration)
    {
        ccstate = cc;
        yield return new WaitForSeconds(duration);
        ccstate = CCStateOfPlayer.normal;
    }
    //OOP Handling
    public void ChangeSens(float x,float y)
    {
        sensX = x;
        sensY = y;
    }
    public void handleSNB(bool value)
    {
        snb = value;
    }
    public void handleDV(bool value)
    {
        dv = value;
    }


    public void TakeDMG(float dmgAmmount, GameObject dmgTakenFrom)
    {
        currentHP -= dmgAmmount;

        //0ön 1ön sağ 2 sağ 3 arka sağ 4 arka 5 arka sol 6sol 7 sol ön
        //DamageVisualzie
        gc.changeHPOfPlayer(maxHP, currentHP);


        GameObject capsule = gameObject.transform.GetChild(0).gameObject;
        GameObject ori = capsule.transform.GetChild(0).gameObject;

        int pos = 0;

        if (Vector3.Dot(ori.transform.right, Vector3.Normalize(ori.transform.position - dmgTakenFrom.transform.position)) > 0)
        {
            //soll
            Vector3 targetDirection = dmgTakenFrom.transform.position - gameObject.transform.position;
            float angle = Vector3.Angle(ori.transform.forward, targetDirection.normalized);
            angle = 360 - angle;

            pos = (int)(angle / 45);
        }
        else
        {
            //sağ
            Vector3 targetDirection = dmgTakenFrom.transform.position - gameObject.transform.position;
            float angle = Vector3.Angle(ori.transform.forward, targetDirection);

            pos = (int)(angle / 45);
        }
        //0:back 1:left 2:rigth 3:front
        switch (pos)
        {
            case 0: gc.HandleDMGtakenUI(3); break;
            case 1: gc.HandleDMGtakenUI(3); gc.HandleDMGtakenUI(2); break;
            case 2: gc.HandleDMGtakenUI(2); break;
            case 3: gc.HandleDMGtakenUI(0); gc.HandleDMGtakenUI(2); break;
            case 4: gc.HandleDMGtakenUI(0); break;
            case 5: gc.HandleDMGtakenUI(0); gc.HandleDMGtakenUI(1); break;
            case 6: gc.HandleDMGtakenUI(1); break;
            case 7: gc.HandleDMGtakenUI(1); gc.HandleDMGtakenUI(3); break;
            default: break;
        }
        
        if (dv)//check get damage effect
        {
            mainCam.gameObject.GetComponent<CameraShake>().StartCamShake();
        }

        if (currentHP <= 0)
        {
            gc.EndGame();
        }
    }
    public void HealDMG(float healAmount, GameObject healTakenFrom)
    {
        currentHP += healAmount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        gc.changeHPOfPlayer(maxHP, currentHP);
    }
    public float getMaxhHP()
    {
        return maxHP;
    }
    public void setMaxHP(float amount,float duration)
    {
        if (duration == -1)
        {
            maxHP = amount;
        }
        else
        {

        }
    }
    //skillHandling
    public void SetSpeed(float multiplier, float duration)
    {
        if(duration == -1)//Permanent
        {

        }
        else
        {
            moveSpeed *= multiplier;
            dashForce *= multiplier;
            if (speedEfect != null)
            {
                StopCoroutine(speedEfect);
            }
            speedEfect = StartCoroutine(SpeedMultiplierDuration(multiplier, duration));
        }
    }
    IEnumerator SpeedMultiplierDuration(float multiplier,float duration)
    {
        Camera handCam = mainCam.transform.GetChild(0).GetComponent<Camera>();
        targetFOV = 80;
        float minusDuration = 0f;
        while (true)
        {            
            handCam.fieldOfView = Mathf.MoveTowards(handCam.fieldOfView, targetFOV, Time.deltaTime * 30);
            mainCam.fieldOfView = Mathf.MoveTowards(mainCam.fieldOfView, targetFOV, Time.deltaTime * 30);
            if (handCam.fieldOfView == targetFOV && mainCam.fieldOfView == targetFOV)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
            minusDuration += Time.deltaTime;
        }

        float timer= duration-minusDuration;
        while (true)
        {
            handCam.fieldOfView = targetFOV;
            mainCam.fieldOfView = targetFOV;


            yield return new WaitForEndOfFrame();
            if (timer <= 0)
            {
                break;
            }
            timer -= Time.deltaTime;
        }
        
        moveSpeed /= multiplier;
        dashForce /= multiplier;

        targetFOVnormal = 60;

        while (true)
        {
            handCam.fieldOfView = Mathf.MoveTowards(handCam.fieldOfView, targetFOVnormal, Time.deltaTime * 30);
            mainCam.fieldOfView = Mathf.MoveTowards(mainCam.fieldOfView, targetFOVnormal, Time.deltaTime * 30);
            if (handCam.fieldOfView == targetFOVnormal&& mainCam.fieldOfView == targetFOVnormal)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public void addExtraJump()
    {
        extrajump += 1;
    }
    public void addDash()
    {
        currentdashMeter += 25;
        if (currentdashMeter > maxdashmeter)
        {
            currentdashMeter = maxdashmeter;
        }
        gc.DashIndicator(currentdashMeter);
    }
    public void fovChange(float fov)
    {
        Camera handCam = mainCam.transform.GetChild(0).GetComponent<Camera>();

        mainCam.fieldOfView = fov;
        handCam.fieldOfView = fov;
        targetFOV = fov + (fov * 20 / 60);
        targetFOVnormal = fov - (fov * 20 / 60);
    }
    public void aspectChange(float aspect)
    {
        Camera handCam = mainCam.transform.GetChild(0).GetComponent<Camera>();

        mainCam.aspect = aspect;
        handCam.aspect = aspect;

    }
}
