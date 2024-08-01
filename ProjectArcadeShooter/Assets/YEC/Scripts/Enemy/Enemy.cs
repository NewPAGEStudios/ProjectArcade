using System.Collections;
using System.Collections.Generic;
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
    public float fieldOfView = 85f;
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
    // Start is called before the first frame update
    void Start()
    {
        if (e_type.isRanged)
        {
            gunBarrel = transform.Find("firePos");
        }

        GameObject model = Instantiate(e_type.modelGameObject, gameObject.transform);

        animator = model.GetComponent<Animator>();

        ehp = gameObject.AddComponent<EnemyHealth>();
        ehp.maxHealth = e_type.hitPoint;

        meleeDmg = e_type.attackDMG;

        path = gameObject.AddComponent<Path>();
        //PathInitialization
        int mostPath = 2;//TODO change to random.range System with different

        for(int i = 0; i < mostPath; i++)
        {
            float x = Random.Range(parentSelectedPosition.transform.Find("min").position.x, parentSelectedPosition.transform.Find("max").position.x);
            float y = parentSelectedPosition.position.y + model.transform.localScale.y;
            float z = Random.Range(parentSelectedPosition.transform.Find("min").position.z, parentSelectedPosition.transform.Find("max").position.z);

            path.waypoints.Add(new Vector3(x, y, z));
        }

        stateMachine = gameObject.AddComponent<StateMachine>();
        
        if(NavMesh.SamplePosition(gameObject.transform.position,out NavMeshHit hit, 500f, NavMesh.AllAreas))
        {
            gameObject.transform.position = hit.position;

            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.agentTypeID = agenjtFindClass.GetAgentTypeIDbyName(e_type.agentName);
            agent.speed = e_type.moveSpeed;

        }


        fireRate = e_type.attackRatio;

        stateMachine.Initialise();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        currentState = stateMachine.activeState.ToString();
    }
    public bool CanSeePlayer()
    {
        if(player != null)
        {
            if(Vector3.Distance(transform.position, player.transform.position) < sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if(angleToPlayer>= -fieldOfView && angleToPlayer<=fieldOfView)
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
}
