using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    public StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;
    private Vector3 lastKnowPos;

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
    public Transform gunBarrel;
    

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

    private enum ccState
    {
        normal,
        stun,
    }
    ccState state;


    // Start is called before the first frame update
    void Start()
    {
        GameObject model = Instantiate(e_type.modelGameObject, gameObject.transform);
        

        model.name = "Model";

        model.transform.GetChild(0).gameObject.layer = 10;

        if (e_type.isRanged)
        {
            gunBarrel = transform.Find("Model").Find("firePos");
        }

        animator = model.GetComponent<Animator>();

        ehp = gameObject.AddComponent<EnemyHealth>();
        ehp.maxHealth = e_type.hitPoint;

        meleeDmg = e_type.attackDMG;

        path = gameObject.AddComponent<Path>();

        //PathInitialization
        int mostPath = 2;//TODO change to random.range System with different

        for(int i = 0; i < mostPath; i++)
        {
            float x = UnityEngine.Random.Range(parentSelectedPosition.transform.Find("min").position.x, parentSelectedPosition.transform.Find("max").position.x);
            float y = parentSelectedPosition.position.y + model.transform.localScale.y;
            float z = UnityEngine.Random.Range(parentSelectedPosition.transform.Find("min").position.z, parentSelectedPosition.transform.Find("max").position.z);

            path.waypoints.Add(new Vector3(x, y, z));
        }

        stateMachine = gameObject.AddComponent<StateMachine>();
        
        if(NavMesh.SamplePosition(gameObject.transform.position,out NavMeshHit hit, 500f, NavMesh.AllAreas))
        {
            gameObject.transform.position = hit.position;

            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.agentTypeID = agenjtFindClass.GetAgentTypeIDbyName(e_type.agentName);
            agent.speed = e_type.moveSpeed;
            if(e_type.agentName == "EnemyFly"){
                agent.baseOffset = 5;
                currentBaseOffeset = agent.baseOffset;

            }
        }


        fireRate = e_type.attackRatio;
        
        player = GameObject.FindGameObjectWithTag("Player");

        stateMachine.Initialise();

    }

    // Update is called once per frame
    void Update()
    {
        if (state == ccState.stun)
        {
            return;//Updateyi patlat
        }
        currentState = stateMachine.activeState.ToString();
    }
    // void FixedUpdate(){
    //     if (e_type.EnemyTypeID == 1)
    //     {

    //         BaseOffsetValueControl();
    //     }
        
    // }
    public bool CanSeePlayer()
    {
        if(player != null)
        {
            if(Vector3.Distance(transform.position, player.transform.position) < sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if(angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitInfo = new RaycastHit();
                    if(Physics.Raycast(ray, out hitInfo, sightDistance))
                    {
                        if(hitInfo.transform.gameObject == player)
                        {
                            stateMachine.agentControl.LastKnowPos = player.transform.position;
                            return true;
                        }
                    }
                    Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                }
            }
        }
        return false;
    }
    public void MeleeAttack()
    {
        animator.SetTrigger("Attack");
        meleeObj.GetComponent<Collider>().enabled = true;
        animator.SetBool("AttackEnd", true);
        
        transform.LookAt(Player.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        StartCoroutine(AttackAnim_Routine());

    }
    IEnumerator AttackAnim_Routine()
    {
        while (true)
        {

            yield return new WaitForEndOfFrame();

            if (animator.GetCurrentAnimatorStateInfo(1).IsName("AttackEnd"))
            {
                meleeObj.GetComponent<Collider>().enabled = false;
                break;
            }

        }

        animator.SetBool("AttackEnd", false);
        
    }
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
    //crowd controll
    public void stun()
    {
        StartCoroutine(stunEffect());
    }

    //crowd controll effect
    IEnumerator stunEffect()
    //visualize et material rengi ve material property block ile // //ayr�ca hareketi k�sacak stateyi ayarla//
    {
        yield return null;

    }

}