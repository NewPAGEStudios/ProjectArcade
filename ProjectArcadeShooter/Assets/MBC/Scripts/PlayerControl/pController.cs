using Cinemachine;
using System.Collections;
using System.Collections.Generic;
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
    private float slideForce;
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
    public float sensX;
    public float sensY;


    private CameraPOVExtension cpove;

    GameController gc;
    WeaponManager weaponManager;

    //skill variables
    private int extrajump;

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


        sensX = PlayerPrefs.GetFloat("XSensitivity");
        sensY = PlayerPrefs.GetFloat("YSensitivity");


        gc.AddDashIndicator(maxdashmeter);
        currentdashMeter = maxdashmeter;
        gc.DashIndicator(currentdashMeter);

        cpove = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>().GetComponent<CameraPOVExtension>();
        sensX = 10f;
        sensY = 10f;
        cpove.SensivityChanger(sensX, sensY);
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
        Sliding();
        Dashing();
        //sway n Bob

        //weaponPrecadural Anim
        weaponManager.Sway(iManager.getCameraMovement());
        weaponManager.SwayRotation(iManager.getCameraMovement());

        CheckGround();
        weaponManager.ConpositePositionRotation();
    }
    private void FixedUpdate()
    {
        if (gc.pState == GameController.PlayState.inPlayerInterrupt || gc.pState == GameController.PlayState.inCinematic || gc.state == GameController.GameState.inShop || ccstate != CCStateOfPlayer.normal)
        {
            return;
        }
        Move();

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
    //EðilmeBaþla
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
    }
    //EÐilme Bit

    private void Move()
    {
        if(actiontp == ActionStateDependecyToPlayer.slide || actiontp == ActionStateDependecyToPlayer.dash)
        {//prevent movement
            return;
        }
        Vector2 moveDirVect2 = iManager.getPlayerMovement();// (x,y) (x,y,z) (x,0,y)
        weaponManager.BobOffset(this,moveDirVect2);
        weaponManager.BobRotation(moveDirVect2);



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
                Debug.LogWarning("action State to Player Sýkýntýlý");
                break;
        }

        switch (actiontg)
        {
            case ActionStateDependecyToGround.flat:
                rb.AddForce(moveSpeed * Time.fixedDeltaTime * moveDir, ForceMode.Force);
                break;
            case ActionStateDependecyToGround.slope:
                rb.AddForce(moveSpeed * Time.fixedDeltaTime * Slope(moveDir), ForceMode.Force);
                break;
            case ActionStateDependecyToGround.onAir:
                rb.AddForce(moveSpeedOnAir * Time.fixedDeltaTime * moveDir, ForceMode.Force);
                break;
            default:
                Debug.LogWarning("action State to Ground Sýkýntýlý");
                break;
        }
        //        rb.AddForce(Slope(moveDir) * moveSpeed * Time.fixedDeltaTime, ForceMode.Force);


        Vector3 temp = new(rb.velocity.x, rb.velocity.y, rb.velocity.z);


        float speedTemp = temp.magnitude;

        if (speedTemp > maxSpeed)
        {
            Vector3 liimitedVec = temp.normalized * maxSpeed;

            switch (actiontg)
            {
                case ActionStateDependecyToGround.slope:
                    rb.velocity = new Vector3(liimitedVec.x, liimitedVec.y, liimitedVec.z);
                    break;
                case ActionStateDependecyToGround.flat:
                    break;
                case ActionStateDependecyToGround.onAir:
                    break;
                default:
                    rb.velocity = new Vector3(liimitedVec.x, rb.velocity.y, liimitedVec.z);
                    break;

            }
        }
    }


    //KaymaDodge
    private void Sliding()
    {
        if (iManager.getSlidePressed())
        {
            if(actiontp != ActionStateDependecyToPlayer.idle || actiontg == ActionStateDependecyToGround.onAir)
            {
                return;
            }
            if (currentdashMeter < 25)
            {
                return;
            }

            Transform targetScaleExchangeUnit = gameObject.transform.GetChild(0);//getchild(0) = firts child of hierarcy it will be model be sure to do that
            Transform targetPosExchangeUnit = gameObject.transform;


            scale = targetScaleExchangeUnit.localScale;
            pos = targetPosExchangeUnit.position;

            targetScaleExchangeUnit.localScale = new Vector3(scale.x, scale.y / 2, scale.z);
            targetPosExchangeUnit.position = new Vector3(pos.x, pos.y - scale.y / 2, pos.z);

            switch (actiontg)
            {
                case ActionStateDependecyToGround.flat:
                    rb.AddForce(transform.forward * slideForce, ForceMode.VelocityChange);
                    break;
                case ActionStateDependecyToGround.slope:
                    rb.AddForce(Slope(transform.forward) * slideForce, ForceMode.VelocityChange);
                    break;
            }

            actiontp = ActionStateDependecyToPlayer.slide;

            currentdashMeter -= 25;
            gc.DashIndicator(currentdashMeter);

            Invoke(nameof(SlidingNormal), slideDuration);
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

            actiontp = ActionStateDependecyToPlayer.dash;

            currentdashMeter -= 25;
            gc.DashIndicator(currentdashMeter);

            Vector2 moveDirVect2 = iManager.getPlayerMovement();// (x,y) (x,y,z) (x,0,y)
            Vector3 moveDir = new(moveDirVect2.x, 0f, moveDirVect2.y);
            moveDir = gameObject.transform.right * moveDir.x + gameObject.transform.forward * moveDir.z;

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
            Invoke(nameof(DashingNormal), slideDuration - 0.03f);
        }
    }
    private void DashingNormal()
    {
        rb.velocity = Vector3.zero;
        actiontp = ActionStateDependecyToPlayer.idle;
    }
    private void SlidingNormal()
    {
        Transform targetScaleExchangeUnit = gameObject.transform.GetChild(0);//getchild(0) = firts child of hierarcy it will be model be sure to do that
        Transform targetPosExchangeUnit = gameObject.transform;

        rb.velocity = new Vector3(0, 0, 0);
        scale = targetScaleExchangeUnit.localScale;
        pos = targetPosExchangeUnit.position;

        targetPosExchangeUnit.position = new Vector3(pos.x, pos.y + scale.y, pos.z);
        targetScaleExchangeUnit.localScale = new Vector3(scale.x, scale.y * 2, scale.z);

        actiontp = ActionStateDependecyToPlayer.idle;
    }


    private Vector3 Slope(Vector3 directionOnNormalPlane)
    {
        return Vector3.ProjectOnPlane(directionOnNormalPlane, slopePlaneNormal).normalized;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            float angleOfPlane = Vector3.Angle(Vector3.up, collision.contacts[0].normal);

            if(angleOfPlane > max_angleOfPlane || angleOfPlane == 0)
            {
                actiontg = ActionStateDependecyToGround.flat;


                slopePlaneNormal = Vector3.up;
                rb.useGravity = true;
            }
            else
            {//is Slope granted
                actiontg = ActionStateDependecyToGround.slope;
                slopePlaneNormal = collision.contacts[0].normal;
                rb.useGravity = false;
            }
        }
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
        Debug.DrawRay(gameObject.transform.position, checkGroundVector * (offsetScale + 0.2f), Color.red, Time.deltaTime);
        if (Physics.Raycast(gameObject.transform.position, checkGroundVector, offsetScale + 0.2f))
        {
            rb.drag = moveDrag;
            maxSpeed = maxSpeedOnGround;
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
        rb.AddForce(throwDirection * 3f, ForceMode.Impulse);
        StartCoroutine(cced(CCStateOfPlayer.ccd, 0.2f));
    }


    IEnumerator cced(CCStateOfPlayer cc ,float duration)
    {
        ccstate = cc;
        yield return new WaitForSeconds(duration);
        ccstate = CCStateOfPlayer.normal;
    }
    //OOP Handling
    public void TakeDMG(float dmgAmmount, GameObject dmgTakenFrom)
    {
        currentHP -= dmgAmmount;
        if (currentHP <= 0)
        {
            gc.EndGame();
        }
        else
        {//0ön 1ön sað 2 sað 3 arka sað 4 arka 5 arka sol 6sol 7 sol ön
            
            //DamageVisualzie
            gc.changeHPOfPlayer(maxHP, currentHP);


            GameObject capsule = gameObject.transform.GetChild(0).gameObject;
            GameObject ori = capsule.transform.GetChild(0).gameObject;

            int pos = 0;

            if(Vector3.Dot(ori.transform.right,Vector3.Normalize(ori.transform.position - dmgTakenFrom.transform.position)) > 0)
            {
                //soll
                Vector3 targetDirection = dmgTakenFrom.transform.position - gameObject.transform.position;
                float angle = Vector3.Angle(ori.transform.forward, targetDirection.normalized);
                angle = 360 - angle;

                pos = (int)(angle / 45);
            }
            else
            {
                //sað
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
                default:break;
            }
            gc.takeDmgEffect();
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
    public void setMaxHP(float newHP)
    {
        maxHP = newHP;
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
            slideForce *= multiplier;
            dashForce *= multiplier;
            StartCoroutine(SpeedMultiplierDuration(multiplier, duration));
        }
    }
    IEnumerator SpeedMultiplierDuration(float multiplier,float duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed /= multiplier;
        slideForce /= multiplier;
        dashForce /= multiplier;
    }
    public void addExtraJump()
    {
        extrajump += 1;
    }
}
