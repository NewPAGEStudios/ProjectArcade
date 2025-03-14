using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastAmmoBulletFunction : MonoBehaviour
{
    private GameController gc;

    //effects
    MaterialPropertyBlock m_PropertyBlock;
    Renderer modelRender;

    public Vector3 calcvec;

    public GameObject firedBy;

    public LayerMask layerMask;

    //shader
    public Material[] modelMat;
    Color baseColor;

    //ObjectProperties
    public float bulletSpeed;
    public int numberOfCollisionHit;
    public int mostHitCanBeDone;
    public float dmg;
    private float lifeTime = 10f;

    private bool interrupt_Movement;


    private List<Vector3> reflectionVecs = new();
    private List<Vector3> reflectionPoints = new();
    private List<Vector3> reflectionNormals = new();

    public TrailType trailType;
    private GameObject trail;
    public Trail3D trail3D;

    public GameObject trace;

    private AudioSource hitSound;
    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        modelRender = GetComponentInChildren<MeshRenderer>();
        m_PropertyBlock = new MaterialPropertyBlock();

        baseColor = modelRender.sharedMaterial.GetColor("_BaseColor");

        m_PropertyBlock.SetFloat("_fresnalPow", 2f);
        modelRender.SetPropertyBlock(m_PropertyBlock);
        //Trail Renderer
        trail = new();
        trail.transform.parent = transform;
        trail.transform.localPosition = Vector3.zero;

        trail.AddComponent<TrailRenderer>();
        trail.GetComponent<TrailRenderer>().material = trailType.TrailMaterail;
        trail.GetComponent<TrailRenderer>().colorGradient = trailType.gradient;
        trail.GetComponent<TrailRenderer>().widthCurve = trailType.curve;
        trail.GetComponent<TrailRenderer>().time = trailType.Time;

        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().freezeRotation = false;
        gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

        gameObject.AddComponent<CapsuleCollider>();
        gameObject.GetComponent<CapsuleCollider>().isTrigger = true;

        Ray ray = new(calcvec, transform.forward);

        interrupt_Movement = false;

        hitSound = modelRender.transform.Find("Sounds").GetChild(0).GetComponent<AudioSource>();

        for (int i = 0; i < mostHitCanBeDone; i++)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 200, layerMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 vec = Vector3.Reflect(ray.direction, hit.normal);
                ray = new Ray(hit.point, vec);
                reflectionVecs.Add(vec);
                reflectionPoints.Add(hit.point);
                reflectionNormals.Add(hit.normal);
            }
        }
    }
    private void Update()
    {
        if (interrupt_Movement)
        {
            return;
        }
        if (lifeTime > 0)
        {
            lifeTime -= Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, reflectionPoints[numberOfCollisionHit], Time.deltaTime * bulletSpeed / 4);
            transform.LookAt(reflectionPoints[numberOfCollisionHit]);

            if (transform.position == reflectionPoints[numberOfCollisionHit])
            {
                GameObject traceGO = Instantiate(trace, gc.tracesParent.transform);
                traceGO.transform.position = reflectionPoints[numberOfCollisionHit];
                traceGO.transform.up = reflectionNormals[numberOfCollisionHit];

                m_PropertyBlock.SetFloat("_fresnalPow", m_PropertyBlock.GetFloat("_fresnalPow") + 2);
                modelRender.SetPropertyBlock(m_PropertyBlock);

                numberOfCollisionHit++;
                if (numberOfCollisionHit == mostHitCanBeDone)
                {
                    interrupt_Movement = true;
                    startDesttroyObject();
                }
                dmg += 25;
            }

        }
        else
        {
            interrupt_Movement = true;
            startDesttroyObject();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (interrupt_Movement)
        {
            return;
        }
        hitSound.Play();
        if (other.transform.CompareTag("EnemyColl"))
        {
            gc.ComboVombo(numberOfCollisionHit);

            interrupt_Movement = true;
            other.GetComponent<ColliderParenter>().targetOBJ.transform.parent.GetComponent<EnemyHealth>().EnemyHealthUpdate(-dmg, firedBy);
            gc.dmgDoneFeedBack();
            interrupt_Movement = true;
            startDesttroyObject();
        }
        else if (other.gameObject.layer == 9)//bossCollider
        {
            //Manuel Adding
            if (other.transform.parent.parent.TryGetComponent<DummyMummyFunc>(out DummyMummyFunc dmf))
            {
                dmf.GetDamage(dmg);
            }
            gc.HandleDmgGiven();

            interrupt_Movement = true;
            startDesttroyObject();
        }
        else if (other.transform.gameObject.TryGetComponent<wallriser>(out wallriser w))
        {
            w.decreaseHitNumber();
        }
    }



    private void startDesttroyObject()
    {
        //onetimeEvent
        modelRender.sharedMaterial = modelMat[1];
        m_PropertyBlock = new MaterialPropertyBlock();

        m_PropertyBlock.SetColor("_BaseColor", baseColor);
        m_PropertyBlock.SetFloat("_noiseScale", 50f);
        m_PropertyBlock.SetFloat("_NoiseStrength", 12.5f);
        modelRender.SetPropertyBlock(m_PropertyBlock);


        GetComponentInChildren<CapsuleCollider>().enabled = false;
        StartCoroutine(disolveEffectRoutine());
    }
    IEnumerator disolveEffectRoutine()
    {
        //Assume that is shader has cahnged to disolve
        while (m_PropertyBlock.GetFloat("_NoiseStrength") > 0f)
        {
            m_PropertyBlock.SetFloat("_NoiseStrength", m_PropertyBlock.GetFloat("_NoiseStrength") - 0.2f);
            modelRender.SetPropertyBlock(m_PropertyBlock);
            yield return new WaitForSeconds(0.001f);
        }
        Destroy(gameObject);
    }



}
