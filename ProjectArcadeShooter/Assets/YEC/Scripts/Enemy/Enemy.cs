using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    private Vector3 lastKnowPos;
    public EnemySoundController soundController;

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
    }
    public ccState state;


    // Start is called before the first frame update
    void Start()
    {
        eState = enemyState.alive;
        GameObject model = Instantiate(e_type.modelGameObject, gameObject.transform);
        

        model.name = "Model";

        model.transform.GetChild(0).gameObject.layer = 10;

        if (e_type.isRanged)
        {
            Transform tempObject = transform.Find("Model").Find("firePos");
            for (int i = 0; i < tempObject.childCount; i++)
            {
                gunBarrel.Add(tempObject.GetChild(i));
            }
        }

        animator = model.GetComponent<Animator>();

        ehp = gameObject.AddComponent<EnemyHealth>();
        ehp.maxHealth = e_type.hitPoint;

        meleeDmg = e_type.attackDMG;



        soundController = gameObject.AddComponent<EnemySoundController>();

        if(NavMesh.SamplePosition(gameObject.transform.position,out NavMeshHit hit, 500f, NavMesh.AllAreas))
        {
            gameObject.transform.position = hit.position;

            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.agentTypeID = agenjtFindClass.GetAgentTypeIDbyName(e_type.agentName);
            agent.speed = e_type.moveSpeed;
        }

        System.Type scriptMB = System.Type.GetType(e_type.agentFunction + ",Assembly-CSharp");
        gameObject.AddComponent(scriptMB);

        //ManuelAdding
        if(gameObject.TryGetComponent<WalkerMelee>(out WalkerMelee wm))
        {

        }
        else if (gameObject.TryGetComponent<WalkerRanged>(out WalkerRanged wr))
        {

        }
        else if(gameObject.TryGetComponent<DroneMA>(out DroneMA dma))
        {

        }


        fireRate = e_type.attackRatio;
        
        player = GameObject.FindGameObjectWithTag("Player");


    }

    // Update is called once per frame
    void Update()
    {
        if (state == ccState.stun || eState == enemyState.dead)
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
    //crowd controll
    public void Stun(float dur)
    {
        ehp.stunEffect(dur);
        agent.speed = 0;
        state = ccState.stun;
        Debug.Log(agent.speed);
    }
    public void StunEnd()
    {
        agent.speed = e_type.moveSpeed;
        state = ccState.normal;
    }

    //crowd controll effect
}