using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMummyFunc : MonoBehaviour
{
    private float maxHP = 100;//TODO DEGGISTIR
    private float currentHP;
    private int bossSearchID = 0;

    public enum BossState
    {
        interrupted,
        inIdle,
        inAttack,
        inDeath,
    }
    public BossState bossState;

    private float idleTimer;



    private Vector3 middle;
    
    private GameController gc;
    private GameObject player;

    public Boss boss;
    public float meleeDMG = 30f;
    public bool meleeDMG_Activated = false;
    public GameObject mapParent;
    private GameObject soundParent;

    private GameObject effectParent;

    public GameObject bossStartMenu;
    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        player = GameObject.FindGameObjectWithTag("Player");
        effectParent = gameObject.transform.Find("Effects").gameObject;

        for(int c = 0; c < effectParent.transform.childCount; c++)
        {
            effectParent.transform.GetChild(c).gameObject.SetActive(false);
        }
        for(int i = 0; i < gc.boss.Length; i++)
        {
            if (gc.boss[i].BossID == bossSearchID)
            {
                maxHP = gc.boss[i].maxHP;
                break;
            }
        }
        currentHP = maxHP;
        gc.BossHPChange(currentHP / maxHP);
        bossState = BossState.interrupted;
        middle = mapParent.transform.Find("Middle").position;
        soundParent = transform.GetChild(6).gameObject;

        GameObject firePos = gameObject.transform.Find("FirePosR").gameObject;
        firePos.transform.Find("AmmoSimVis").gameObject.SetActive(true);
        firePos = gameObject.transform.Find("FirePosL").gameObject;
        firePos.transform.Find("AmmoSimVis").gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (bossState == BossState.interrupted)
        {
            return;
        }
        else if (bossState == BossState.inIdle)
        {
            if (idleTimer <= 0)
            {
                AttackOpenner();
            }
            else
            {
                gameObject.transform.LookAt(player.transform.position);
                gameObject.transform.eulerAngles = new Vector3(0, gameObject.transform.eulerAngles.y, 0);
                idleTimer -= Time.deltaTime;
            }
        }
        else if(bossState == BossState.inAttack)
        {

        }
        else if(bossState == BossState.inDeath)
        {

        }
    }
    public void toIdle()//attack süreleri
    {
        bossState = BossState.inIdle;
        idleTimer = Random.Range(2, 4);
    }
    public void GetDamage(float dmg)
    {
        currentHP -= dmg;
        gc.BossHPChange(currentHP / maxHP);
        if (currentHP <= 0)
        {
            bossState = BossState.inDeath;
            StopAllCoroutines();
            NormalEverything();
            gc.endBoss(gameObject);

        }
    }
    private void AttackOpenner()
    {
        bossState = BossState.inAttack;
        int[] attackPath = new int[3];
        int[] ids = { 0, 1, 3 };
        int id = ids[Random.Range(0, ids.Length)];//3 ATTACK Openner TYPE 0-dash to middle 1 -dash to player 2- above Attack
        attackPath[0] = id;
        if (id == 0)
        {
            ids[0] = 1;
            ids[1] = 2;
            ids[2] = 3;
            id = ids[Random.Range(0, ids.Length)];
            attackPath[1] = id;
            switch (id)
            {
                case 1:
                    ids[0] = 1;
                    ids[1] = 3;
                    id = ids[Random.Range(0, ids.Length - 1)];
                    attackPath[2] = id;
                    break;
                case 2:
                    ids[0] = 1;
                    ids[1] = 3;
                    id = ids[Random.Range(0, ids.Length - 1)];
                    attackPath[2] = id;
                    break;
                case 3:
                    ids[0] = 1;
                    ids[1] = 3;
                    id = ids[Random.Range(0, ids.Length - 1)];
                    attackPath[2] = id;
                    break;
                default:
                    break;
            }
        }
        else if (id == 1)
        {
            ids[0] = 0;
            ids[1] = 1;
            ids[2] = 3;
            id = ids[Random.Range(0, ids.Length)];
            attackPath[1] = id;
            switch (id)
            {
                case 0:
                    ids[0] = 2;
                    ids[1] = 3;
                    id = ids[Random.Range(0, ids.Length - 1)];
                    attackPath[2] = id;
                    break;
                case 1:
                    ids[0] = 1;
                    ids[1] = 3;
                    id = ids[Random.Range(0, ids.Length - 1)];
                    attackPath[2] = id;
                    break;
                case 3:
                    ids[0] = 1;
                    ids[1] = 3;
                    id = ids[Random.Range(0, ids.Length - 1)];
                    attackPath[2] = id;
                    break;
                default:
                    break;
            }
        }
        else if (id == 3)
        {
            ids[0] = 0;
            ids[1] = 1;
            ids[2] = 3;
            id = ids[Random.Range(0, ids.Length)];
            attackPath[1] = id;
            switch (id)
            {
                case 0:
                    ids[0] = 2;
                    ids[1] = 3;
                    id = ids[Random.Range(0, ids.Length - 1)];
                    attackPath[2] = id;
                    break;
                case 1:
                    ids[0] = 1;
                    ids[1] = 3;
                    id = ids[Random.Range(0, ids.Length - 1)];
                    attackPath[2] = id;
                    break;
                case 3:
                    ids[0] = 1;
                    ids[1] = 3;
                    id = ids[Random.Range(0, ids.Length - 1)];
                    attackPath[2] = id;
                    break;
                default:
                    break;
            }
        }
        Debug.Log("attack Path: " + attackPath);
        switch (attackPath[0])
        {
            case 0:
                StartCoroutine(moveWithAttack(middle, attackPath, 0));
                break;
            case 1:
                StartCoroutine(moveWithAttack(player.transform.position, attackPath, 0));
                break;
            case 3:
                StartCoroutine(aboveAttack(attackPath, 0));
                break;
            default : break;
        }
    }

    IEnumerator moveWithAttack(Vector3 targetPos,int[] attackPath, int attackID)
    {
        while (bossState == BossState.inIdle) 
        {
            yield return new WaitForEndOfFrame();
        }
        gc.SpawnCons(findClosestYTNKZZMN(), 2, -1, 4, true);

        targetPos = new Vector3(targetPos.x, middle.y, targetPos.z);
        gameObject.transform.LookAt(targetPos);
        gameObject.transform.eulerAngles = new Vector3(60, gameObject.transform.eulerAngles.y, 0);
        GameObject rotateObject = gameObject.transform.GetChild(0).gameObject;
        effectParent.transform.Find("HandsTrail").gameObject.SetActive(true);


        //TODO: Attack started feedback to player with sound fx
        soundParent.transform.Find("Whoosh").GetComponent<AudioSource>().Play();
        while (true)
        {
            meleeDMG_Activated = true;
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPos, Time.deltaTime * 30f);
            rotateObject.transform.localEulerAngles += 150 * Time.deltaTime * new Vector3(0, 10, 0);
            Debug.Log(targetPos);
            yield return new WaitForEndOfFrame();
            if (gameObject.transform.position == targetPos)
            {
                break;
            }
        }
        rotateObject.transform.localEulerAngles = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;
        meleeDMG_Activated = false;

        soundParent.transform.Find("Whoosh").GetComponent<AudioSource>().Stop();

        attackID += 1;
        if (attackID >= attackPath.Length)
        {
            toIdle();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            switch (attackPath[attackID])
            {
                case 0:
                    StartCoroutine(moveWithAttack(middle, attackPath, attackID));
                    break;
                case 1:
                    StartCoroutine(moveWithAttack(player.transform.position, attackPath, attackID));
                    break;
                case 2:
                    StartCoroutine(middleRangedAttack(attackPath, attackID));
                    break;
                case 3:
                    StartCoroutine(aboveAttack(attackPath, attackID));
                    break;
                default: break;
            }
        }
        effectParent.transform.Find("HandsTrail").gameObject.SetActive(false);
    }
    IEnumerator middleRangedAttack(int[] attackPath, int attackID)
    {
        while (bossState == BossState.inIdle)
        {
            yield return new WaitForEndOfFrame();
        }

        gc.SpawnCons(findClosestYTNKZZMN(), 2, -1, 4,true);

        float timeRemainingTOAttack = Random.Range(1, 4);

        GameObject firePos = gameObject.transform.Find("FirePosR").gameObject;
        firePos.transform.Find("AmmoSimVis").gameObject.SetActive(true);
        for(int s = 0; s< firePos.transform.Find("AmmoSimVis").childCount; s++)
        {
            Transform traansform = firePos.transform.Find("AmmoSimVis").GetChild(s);
            traansform.localScale = new Vector3(0.1f, 0.1f, 50f);
            traansform.localPosition = new Vector3(traansform.localPosition.x, traansform.localPosition.y, 25f);
        }
        while (true)
        {
            timeRemainingTOAttack -= Time.deltaTime;
            gameObject.transform.LookAt(player.transform.position);
            gameObject.transform.eulerAngles = new Vector3(0, gameObject.transform.eulerAngles.y - 90, 0);

            firePos.transform.LookAt(player.transform.position);
            firePos.transform.localEulerAngles = new Vector3(firePos.transform.localEulerAngles.x, 90, 0);

            
            yield return new WaitForEndOfFrame();
            if (timeRemainingTOAttack <= 0)
            {
                break;
            }
        }

        int i = 0;
        for (i = 0; i < gc.ammos.Length; i++)
        {
            if (gc.ammos[i].AmmoTypeID == 1)
            {
                break;
            }
        }


        soundParent.transform.Find("Schwing").GetComponent<AudioSource>().Play();
        effectParent.transform.Find("ShootParticleR").gameObject.SetActive(true);
        for(int c = 0; c < 3; c++)
        {
            GameObject go = new();

            go.transform.parent = firePos.transform;

            go.transform.localPosition = new Vector3(0.75f * (c - 1), 0, 0);
            go.transform.localEulerAngles = new Vector3(0, 25 * (c - 1), 0);

            go.name = "BossAmmoR";
            go.layer = 7;
            System.Type scriptMB = System.Type.GetType(gc.ammos[i].functionName + ",Assembly-CSharp");
            go.AddComponent(scriptMB);

            go.GetComponent<DummyMummyAmmo>().ammo = gc.ammos[i];

            go.GetComponent<DummyMummyAmmo>().targetTag = "Player";

            go.AddComponent<Rigidbody>();
            go.GetComponent<Rigidbody>().useGravity = false;
            go.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
            go.GetComponent<Rigidbody>().freezeRotation = true;
        }
        effectParent.transform.Find("ShootParticleR").gameObject.SetActive(false);

        for (int s = 0; s < firePos.transform.Find("AmmoSimVis").childCount; s++)
        {
            Transform traansform = firePos.transform.Find("AmmoSimVis").GetChild(s);
            traansform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            traansform.localPosition = new Vector3(traansform.localPosition.x, traansform.localPosition.y, 0f);
        }
        firePos.transform.Find("AmmoSimVis").gameObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);
        //Left

        firePos = gameObject.transform.Find("FirePosL").gameObject;
        firePos.transform.Find("AmmoSimVis").gameObject.SetActive(true);

        for (int s = 0; s < firePos.transform.Find("AmmoSimVis").childCount; s++)
        {
            Transform traansform = firePos.transform.Find("AmmoSimVis").GetChild(s);
            traansform.localScale = new Vector3(0.1f, 0.1f, 50f);
            traansform.localPosition = new Vector3(traansform.localPosition.x, traansform.localPosition.y, 25f);
        }

        timeRemainingTOAttack = 0.5f;
        while (true)
        {
            timeRemainingTOAttack -= Time.deltaTime;
            gameObject.transform.LookAt(player.transform.position);
            gameObject.transform.eulerAngles = new Vector3(0, gameObject.transform.eulerAngles.y + 90, 0);

            firePos.transform.LookAt(player.transform.position);
            firePos.transform.localEulerAngles = new Vector3(firePos.transform.localEulerAngles.x, -90, 0);

            yield return new WaitForEndOfFrame();
            if (timeRemainingTOAttack <= 0)
            {
                break;
            }
        }

        soundParent.transform.Find("Schwing").GetComponent<AudioSource>().Play();

        effectParent.transform.Find("ShootParticleL").gameObject.SetActive(true);
        for (int c = 0; c < 3; c++)
        {
            GameObject go = new();

            go.transform.parent = firePos.transform;

            go.transform.localPosition = new Vector3(0.75f * (c - 1), 0, 0);
            go.transform.localEulerAngles = new Vector3(0, 25 * (c - 1), 0);

            go.name = "BossAmmoL";
            go.layer = 7;
            System.Type scriptMB = System.Type.GetType(gc.ammos[i].functionName + ",Assembly-CSharp");
            go.AddComponent(scriptMB);

            go.GetComponent<DummyMummyAmmo>().ammo = gc.ammos[i];
            go.GetComponent<DummyMummyAmmo>().targetTag = "Player";
            go.AddComponent<Rigidbody>();
            go.GetComponent<Rigidbody>().useGravity = false;
            go.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
            go.GetComponent<Rigidbody>().freezeRotation = true;
        }
        effectParent.transform.Find("ShootParticleL").gameObject.SetActive(false);

        firePos.transform.Find("AmmoSimVis").gameObject.SetActive(false);
        for (int s = 0; s < firePos.transform.Find("AmmoSimVis").childCount; s++)
        {
            Transform traansform = firePos.transform.Find("AmmoSimVis").GetChild(s);
            traansform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            traansform.localPosition = new Vector3(traansform.localPosition.x, traansform.localPosition.y, 0f);
        }

        attackID += 1;
        if (attackID >= attackPath.Length)
        {
            toIdle();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            switch (attackPath[attackID])
            {
                case 0:
                    StartCoroutine(moveWithAttack(middle, attackPath, attackID));
                    break;
                case 1:
                    StartCoroutine(moveWithAttack(player.transform.position, attackPath, attackID));
                    break;
                case 2:
                    StartCoroutine(middleRangedAttack(attackPath, attackID));
                    break;
                case 3:
                    StartCoroutine(aboveAttack(attackPath, attackID));
                    break;
                default: break;
            }
        }
    }
    IEnumerator aboveAttack(int[] attackPath, int attackID)
    {
        while (bossState == BossState.inIdle)
        {
            yield return new WaitForEndOfFrame();
        }

        gc.SpawnCons(findClosestYTNKZZMN(), 2, -1, 4,true);

        while (true)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(gameObject.transform.position.x, middle.y + 50f, gameObject.transform.position.z),Time.deltaTime * 30f);
            yield return new WaitForEndOfFrame();
            if (gameObject.transform.position.y >= middle.y + 50f)
            {
                break;
            }
        }
        float timeRemainingTOAttack = Random.Range(1, 4);

        GameObject indicator = gameObject.transform.Find("CylinderIndicator").gameObject;


        while (true)
        {
            timeRemainingTOAttack -= Time.deltaTime;
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(player.transform.position.x, middle.y + 50f, player.transform.position.z), Time.deltaTime * 30f);
            yield return new WaitForEndOfFrame();
            if (timeRemainingTOAttack <= 0)
            {
                break;
            }
        }
        indicator.SetActive(true);

        soundParent.transform.Find("Whoosh").GetComponent<AudioSource>().Play();

        effectParent.transform.Find("toDownTrail").gameObject.SetActive(true);
        while (true)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(gameObject.transform.position.x, middle.y, gameObject.transform.position.z), Time.deltaTime * 30f);
            if (Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit hit ,50, 524288,QueryTriggerInteraction.Ignore))
            {
                indicator.transform.position = hit.point;
            }
            yield return new WaitForEndOfFrame();
            if (gameObject.transform.position.y <= middle.y)
            {
                break;
            }
        }
        effectParent.transform.Find("toDownTrail").gameObject.SetActive(false);
        indicator.SetActive(false);
        indicator.transform.localPosition = new Vector3(0, -50, 0);
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 7.5f)
        {
            player.GetComponent<PController>().TakeDMG(30f, gameObject);
            player.GetComponent<PController>().ThrowPlayer(gameObject.transform.position);

            if (Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit hit, 50, 524288, QueryTriggerInteraction.Ignore))
            {
                indicator.transform.position = hit.point;
            }
        }

        soundParent.transform.Find("Whoosh").GetComponent<AudioSource>().Stop();
        attackID += 1;
        if (attackID >= attackPath.Length)
        {
            toIdle();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            switch (attackPath[attackID])
            {
                case 0:
                    StartCoroutine(moveWithAttack(middle, attackPath, attackID));
                    break;
                case 1:
                    StartCoroutine(moveWithAttack(player.transform.position, attackPath, attackID));
                    break;
                case 2:
                    StartCoroutine(middleRangedAttack(attackPath, attackID));
                    break;
                case 3:
                    StartCoroutine(aboveAttack(attackPath, attackID));
                    break;
                default: break;
            }
        }

    }
    private void NormalEverything()
    {
        meleeDMG_Activated = false;
        gameObject.transform.position = middle;
        gameObject.transform.eulerAngles = Vector3.zero;
        
        gameObject.transform.Find("Model").localEulerAngles = Vector3.zero;
        gameObject.transform.Find("Model").localPosition = Vector3.zero;

        GameObject firePos;
        for (int c = 0; c < 2; c++)
        {
            if (c == 0)
            {
                firePos = gameObject.transform.Find("FirePosR").gameObject;
            }
            else
            {
                firePos = gameObject.transform.Find("FirePosL").gameObject;
            }
            firePos.transform.Find("AmmoSimVis").gameObject.SetActive(false);            
        }

        for(int c = 0; c< 5; c++)
        {
            try
            {
                GameObject gos = GameObject.Find("BossAmmoL");
                Destroy(gos);
            }
            catch 
            {
                Debug.Log("catchL");
                break;
            }
        }

        for (int c = 0; c < 5; c++)
        {
            try
            {
                GameObject gos = GameObject.Find("BossAmmoR");
                Destroy(gos);
            }
            catch
            {
                Debug.Log("catchR");
                break;
            }
        }

        GameObject indicator = gameObject.transform.Find("CylinderIndicator").gameObject;
        indicator.SetActive(false);

        for (int i = 0; i < gc.ConsParents.transform.childCount; i++)
        {
            if (gc.ConsParents.transform.GetChild(i).GetChild(0).TryGetComponent<GetWeapon>(out GetWeapon gw))
            {
                if (gw.cameInBoss)
                {
                    gw.closeCons();
                }
            }
            else if (gc.ConsParents.transform.GetChild(i).GetChild(0).TryGetComponent<GetActiveSkill>(out GetActiveSkill gas))
            {
                if (gas.cameInBoss)
                {
                    gas.closeCons();
                }
            }
            else if (gc.ConsParents.transform.GetChild(i).GetChild(0).TryGetComponent<PerformPassiveSkill>(out PerformPassiveSkill pps))
            {
                if (pps.cameInBoss)
                {
                    pps.closeCons();
                }
            }
        }
    }
    private int findClosestYTNKZZMN()
    {
        int holdingIndex = 0;
        if(mapParent.GetComponent<Map>().ConsSpawnPointParent.transform.childCount==0 || mapParent.GetComponent<Map>().ConsSpawnPointParent.transform.childCount == 1)
        {
            return holdingIndex;
        }
        for(int c = 1; c < mapParent.GetComponent<Map>().ConsSpawnPointParent.transform.childCount ; c++)
        {
            if(Vector3.Distance(player.transform.position,mapParent.GetComponent<Map>().ConsSpawnPointParent.transform.GetChild(c).position) < Vector3.Distance(player.transform.position, mapParent.GetComponent<Map>().ConsSpawnPointParent.transform.GetChild(holdingIndex).position))
            {
                holdingIndex = c;
            }
        }
        return holdingIndex;
    }

}