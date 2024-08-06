using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectBulletFunctions : MonoBehaviour
{

    //effects
    MaterialPropertyBlock m_PropertyBlock;
    Renderer modelRender;

    private Vector3 tempFWD;
    public GameObject firedBy;

    //shader
    public Material[] modelMat;
    Color baseColor;

    //ObjectProperties
    public float bulletSpeed;
    public int numberOfCollisionHit;
    public int mostHitCanBeDone;
    public float dmg;
    private float lifeTime = 10f;


    private GameController gc;

    private GameObject trail;

    public TrailType trailType;

    public Trail3D trail3D;


    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        modelRender = GetComponentInChildren<MeshRenderer>();
        m_PropertyBlock = new MaterialPropertyBlock();

        baseColor = modelRender.sharedMaterial.GetColor("_BaseColor");

        m_PropertyBlock.SetFloat("_fresnalPow", 2f);
        modelRender.SetPropertyBlock(m_PropertyBlock);
        tempFWD = transform.forward;

        trail = Instantiate(trail3D.trail, transform);
        trail.transform.localEulerAngles = new Vector3(0,180,0);
        /*
        //Trail Renderer
        trail = new();
        trail.transform.parent = transform;
        trail.transform.localPosition = Vector3.zero;

        trail.AddComponent<TrailRenderer>();
        trail.GetComponent<TrailRenderer>().material = trailType.TrailMaterail;
        trail.GetComponent<TrailRenderer>().colorGradient = trailType.gradient;
        trail.GetComponent<TrailRenderer>().widthCurve = trailType.curve;
        trail.GetComponent<TrailRenderer>().time = trailType.Time;
        */

        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
    }

    private void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
        if(trail.transform.localScale.z < trail3D.maxZScale)
        {
            trail.transform.localScale = new Vector3(10, 10, trail.transform.localScale.z + (Time.deltaTime * GetComponent<Rigidbody>().velocity.magnitude * bulletSpeed));
//            trail.transform.lossyScale.z *= Time.deltaTime * GetComponent<Rigidbody>().velocity;
        }
        else
        {

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        //if collision tag is not functionable than reflect it
        Vector3 newfront = Vector3.Reflect(tempFWD, collision.contacts[0].normal);
        transform.forward = newfront;
        tempFWD = transform.forward;
        if (collision.transform.CompareTag("EnemyColl"))
        {
            collision.transform.parent.parent.GetComponent<EnemyHealth>().EnemyHealthUpdate(-dmg);
            gc.ComboVombo(numberOfCollisionHit);
            startDesttroyObject();
        }
        else if (collision.gameObject.layer == 9 )//bossCollider
        {
            //Manuel Adding
            if (collision.transform.parent.parent.TryGetComponent<DummyMummyFunc>(out DummyMummyFunc dmf))
            {
                dmf.GetDamage(dmg);
            }
            gc.ComboVombo(numberOfCollisionHit);
            gc.HandleDmgGiven();
            startDesttroyObject();
        }
        else if (numberOfCollisionHit >= mostHitCanBeDone)
        {
            startDesttroyObject();
        }
        else
        {
            m_PropertyBlock.SetFloat("_fresnalPow", m_PropertyBlock.GetFloat("_fresnalPow") + 2);
            modelRender.SetPropertyBlock(m_PropertyBlock);

            GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
            dmg += 50;
            numberOfCollisionHit += 1;
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


        GetComponentInChildren<SphereCollider>().enabled = false;
        StartCoroutine(disolveEffectRoutine());
    }
    IEnumerator disolveEffectRoutine()
    {
        //Assume that is shader has cahnged to disolve
        while(m_PropertyBlock.GetFloat("_NoiseStrength") > 0f)
        {
            m_PropertyBlock.SetFloat("_NoiseStrength", m_PropertyBlock.GetFloat("_NoiseStrength") - 0.2f);
            modelRender.SetPropertyBlock(m_PropertyBlock);
            yield return new WaitForSeconds(0.001f);
        }
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
//        trail.transform.parent = null;
    }
}
