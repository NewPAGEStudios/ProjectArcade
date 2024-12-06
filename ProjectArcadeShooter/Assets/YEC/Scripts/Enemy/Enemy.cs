using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    private Vector3 lastKnowPos;
    public EnemySoundController soundController;
    public EnemyFXController fxController;

    public EnemyHealth ehp;
    public NavMeshAgent Agent { get => agent;}
    public GameObject Player {get=> player;}
    public Vector3 LastKnowPos { get => lastKnowPos; set => lastKnowPos = value; }


    [HideInInspector]
    public float meleeDmg;

    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 360f;
    public float eyeHeight;

    [Header("Weapon Values")]
    public List<Transform> gunBarrel = new();
    

    public float fireRate;
    [SerializeField]
    private string currentState;

    [Header(header:"Referances")]
    public Path path;
    public Transform parentSelectedPosition;
    [HideInInspector]
    public EnemyType e_type;
    public Animator animator;
    public float currentBaseOffeset;
    public GameObject meleeObj;

    private Rigidbody rb;
    private bool blown = false;


    private Coroutine blowRoutine;


    public enum enemyState
    {
        alive,
        dead
    }
    public enemyState eState;
    public enum ccState
    {
        normal,
        stun,
        blown,
    }
    public ccState state;


    // Start is called before the first frame update
    void Start()
    {
        eState = enemyState.alive;


        fireRate = e_type.attackRatio;
        GameObject model = Instantiate(e_type.modelGameObject, gameObject.transform);
        

        model.name = "Model";

        model.transform.GetChild(0).gameObject.layer = 10;


        if (e_type.isRanged)
        {
            Transform tempObject = transform.GetChild(0).Find("firePos");
            for (int i = 0; i < tempObject.childCount; i++)
            {
                gunBarrel.Add(tempObject.GetChild(i));
            }
        }

        animator = model.GetComponent<Animator>();
        animator.speed = 1 * (e_type.moveSpeed / 2);

        ehp = gameObject.AddComponent<EnemyHealth>();
        ehp.maxHealth = e_type.hitPoint;

        meleeDmg = e_type.attackDMG;

        ehp.tmpEnemyIndicator = model.transform.Find("EnemyHPIndicator").GetChild(0).GetComponent<TextMeshPro>();
        ehp.hpBar = model.transform.Find("EnemyHPIndicator").GetChild(1).GetComponent<ColliderParenter>().targetOBJ;

        soundController = gameObject.AddComponent<EnemySoundController>();
        fxController = gameObject.AddComponent<EnemyFXController>();

        if (NavMesh.SamplePosition(gameObject.transform.position,out NavMeshHit hit, 500f, NavMesh.AllAreas))
        {
            gameObject.transform.position = hit.position;

            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.agentTypeID = agenjtFindClass.GetAgentTypeIDbyName(e_type.agentName);
            agent.speed = e_type.moveSpeed;
            agent.angularSpeed=200f;
        }

        System.Type scriptMB = System.Type.GetType(e_type.agentFunction + ",Assembly-CSharp");
        gameObject.AddComponent(scriptMB);

        rb = gameObject.AddComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        BoxCollider col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.providesContacts = true;
        col.center = new Vector3(0, 1f, 0);
        col.size = new Vector3(0.75f, 1.75f, 1);
        player = GameObject.FindGameObjectWithTag("Player");


    }

    // Update is called once per frame
    void Update()
    {
        //        Debug.Log("State = " + state + " eState = " + eState);
        if (state == ccState.stun || eState == enemyState.dead || state == ccState.blown)
        {
            return;//Updateyi patlat
        }
    }
    //public bool CanSeePlayer()
    //{
    //    if(player != null)
    //    {
    //        if(Vector3.Distance(transform.position, player.transform.position) < sightDistance)
    //        {
    //            Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
    //            float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
    //            if(angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
    //            {
    //                Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
    //                RaycastHit hitInfo = new RaycastHit();
    //                if(Physics.Raycast(ray, out hitInfo, sightDistance))
    //                {
    //                    if(hitInfo.transform.gameObject == player)
    //                    {
    //                        stateMachine.agentControl.LastKnowPos = player.transform.position;
    //                        return true;
    //                    }
    //                }
    //                Debug.DrawRay(ray.origin, ray.direction * sightDistance);
    //            }
    //        }
    //    }
    //    return false;
    //}

    public void BaseOffsetValueControl(){
        // enemy.Agent.SetDestination(enemy.Player.transform.position);
        // Debug.Log("aradaki mesafe"+distance+"range"+ e_type.rangeDistance);
        agent.radius = 0.6f;// nedense 0.5 değerinde player yüksekteyken takip etmiyor
        float distance = Vector3.Distance(Player.transform.position, transform.position);
        float playerHight = player.transform.position.y - transform.position.y;
        // Debug.Log(playerHight);
        RaycastHit hitInfo = new RaycastHit();
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, 3) && hitInfo.collider.gameObject.tag != "PlayerColl" && hitInfo.collider.gameObject.tag != "EnemyColl")
        {
            Debug.Log("nesne:" + hitInfo.collider.name+ "tag:"+ hitInfo.collider.gameObject.tag);
            agent.speed = 0.1f;
            agent.baseOffset += 0.07f;
            // if(hitInfo.distance < 5){

            // }
            // else{
            //     Debug.Log("hızlan");
            //     agent.speed = 5;
            // }
        }
        else
        {
            agent.speed = 5;
        }

        if(distance <= 15 && agent.baseOffset >= agent.baseOffset + playerHight)
        {

            // -8.935724
            // if(Physics.Raycast(transform.position, transform.forward, out hitInfo, 15))
            // {

            //     /////////////////
            //     // if(hitInfo.transform.CompareTag("PlayerColl"))
            //     // {

            //     // }
            //     // else
            //     // {
            //     //     if(player.transform.position.y > transform.position.y)
            //     //     {
                        
            //     //         // Mathf.MoveTowards(agent.baseOffset,transform.position.y,2);

            //     //         agent.baseOffset += playerHight;
            //     //     }
            //     // }
            // }
            for(float i = distance + 1f; i >= distance; i-= 0.5f)
            {
                agent.baseOffset -= 0.01f; 
            }
        }
        
        if(agent.baseOffset < currentBaseOffeset + playerHight && distance >=15)
        {
            agent.baseOffset += 0.01f;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //rampada
        Ray downwardRay = new Ray(transform.position, Vector3.down);
        RaycastHit hitDownInfo = new RaycastHit();
        if(Physics.Raycast(downwardRay, out hitDownInfo))
        {
            if(hitDownInfo.distance < 0.9)
            {
                agent.baseOffset += 0.04f;
            }
        }
    }
    public void endCC()
    {
        if(blowRoutine == null && ehp.stunRoutine == null)
        {
            state = ccState.normal;
        }
    }
    //crowd controll
    public void Stun(float dur)
    {
        ehp.stunEffect(dur);
        agent.speed = 0;
        state = ccState.stun;
        animator.enabled = false;
    }
    public void StunEnd()
    {
        Debug.Log("kbmm");
        agent.speed = e_type.moveSpeed;
        animator.enabled = true;
        endCC();
    }

    public void blow(Vector3 forceDirection, float power)
    {
        if(agent == null)
        {
            return;
        }
        agent.enabled = false;
        state = ccState.blown;
        rb.isKinematic = false;
        if(e_type.EnemyTypeID==0 || e_type.EnemyTypeID==1)
        {
            animator.SetTrigger("GetMelee");
        }

        rb.AddForce(forceDirection * power, ForceMode.VelocityChange);

        blown = true;
        if (blowRoutine != null)
        {
            StopCoroutine(blowRoutine);
        }
        blowRoutine = StartCoroutine(blownHitRoutine());
    }
    IEnumerator blownHitRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        while (true)
        {
            if (blown)
            {
                if (Physics.Raycast(gameObject.transform.position, Vector3.down, 1.5f))
                {
                    blown = false;
                    break;
                }
            }
            yield return new WaitForFixedUpdate();
        }
        yield return null;


        while (true)
        {
            yield return null;

            if (animator == null)
            {
                break;
            }

            if (e_type.EnemyTypeID == 2|| e_type.EnemyTypeID == 3 || e_type.EnemyTypeID == 4)
            {
                break;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("meleedmgTakenWait"))
            {
                animator.SetTrigger("EndMelee");
                break;
            }
        }
        rb.isKinematic = true;
        agent.enabled = true;
        endCC();
    }
    //crowd controll effect
}