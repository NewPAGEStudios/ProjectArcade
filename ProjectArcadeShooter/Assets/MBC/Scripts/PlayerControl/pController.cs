using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PController : MonoBehaviour
{
    private float maxSpeed;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float moveDrag;

    [SerializeField]
    private float maxSpeedOnGround;
    [SerializeField]
    private float maxSpeedOutGround;

    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpDrag;

    [SerializeField]
    private float crouchSpeed;

    [SerializeField]
    private float slideDuration;
    [SerializeField]
    private float slideForce;

    private InputManager iManager;
    private Rigidbody rb;

    private bool crouchEvent = true;

    private float targetScale;
    private float targetPosY;
    private Vector3 scale;
    private Vector3 pos;


    private Vector3 slopePlaneNormal = Vector3.up;


    [SerializeField]
    private float max_angleOfPlane;

    [SerializeField]
    private float sens;
    private Vector3 cam_StartingRotation;


    WeaponManager weaponManager;

    public enum ActionStateDependecyToPlayer
    {
        idle,
        crouch,
        slide,
    }
    public enum ActionStateDependecyToGround
    {
        flat,
        slope,
        onAir,
    }
    [HideInInspector]
    public ActionStateDependecyToPlayer actiontp;
    [HideInInspector]
    public ActionStateDependecyToGround actiontg;

    // Start is called before the first frame update
    void Start()
    {
        iManager = InputManager.Instance;
        rb = GetComponent<Rigidbody>();
        weaponManager = GetComponent<WeaponManager>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


    }

    // Update is called once per frame
    void Update()
    {
//        Debug.Log("action to player = " + actiontp + " || action to ground = " + actiontg);
        Jump();
        Crouch();
        CrouchExit();
        Sliding();
        CamRotation();
        CheckGround();
        weaponManager.ConpositePositionRotation();
    }
    private void FixedUpdate()
    {
        Move();

    }
    private void Jump()
    {
        if(iManager.getJumpedPressed())
        {
            if(actiontg == ActionStateDependecyToGround.onAir || actiontp != ActionStateDependecyToPlayer.idle)
            {
                return;
            }
            switch (actiontg)
            {
                case ActionStateDependecyToGround.flat:
                    break;
                case ActionStateDependecyToGround.slope:
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
        if(Input.GetKey(KeyCode.LeftControl))
        {
            Transform targetScaleExchangeUnit = gameObject.transform.GetChild(0);//getchild(0) = firts child of hierarcy it will be model be sure to do that
            Transform targetPosExchangeUnit = gameObject.transform;
            if (actiontp == ActionStateDependecyToPlayer.slide)
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
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (!crouchEvent)
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

            //scale
            scale.y = Mathf.MoveTowards(scale.y, targetScale, Time.deltaTime * crouchSpeed);
            targetScaleExchangeUnit.localScale = scale;
            //position
            pos.y = Mathf.MoveTowards(pos.y, targetPos, Time.deltaTime * crouchSpeed);
            targetPosExchangeUnit.position = new Vector3(targetPosExchangeUnit.position.x, pos.y, targetPosExchangeUnit.position.z);
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
        if(actiontp == ActionStateDependecyToPlayer.slide)
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
                rb.AddForce(moveSpeed * Time.fixedDeltaTime * moveDir, ForceMode.Force);
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
// before actinoStateUpdate            rb.AddForce(Slope(transform.forward) * slideForce, ForceMode.VelocityChange);

            actiontp = ActionStateDependecyToPlayer.slide;

            Invoke(nameof(SlidingNormal), slideDuration);
        }
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

    private void CamRotation()//tmmlandý
    {
        if(cam_StartingRotation == null)
        {
            cam_StartingRotation = Camera.main.transform.localRotation.eulerAngles;
        }
        Vector2 deltaInput = iManager.getCameraMovement();
        cam_StartingRotation.x += deltaInput.x * Time.deltaTime * sens;
        cam_StartingRotation.y += deltaInput.y * Time.deltaTime * sens;
        cam_StartingRotation.y = Mathf.Clamp(cam_StartingRotation.y, -60, 60);
        Camera.main.transform.localRotation = Quaternion.Euler(-cam_StartingRotation.y, 0f, 0f);
        gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.x, cam_StartingRotation.x, gameObject.transform.localRotation.z);

        //weaponPrecadural Anim
        weaponManager.Sway(deltaInput);
        weaponManager.SwayRotation(deltaInput);
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


}
