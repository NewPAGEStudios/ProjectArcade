using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMummyFunc : MonoBehaviour
{
    private float maxHP = 500;
    private float currentHP;


    private enum BossState
    {
        interrupted,
        inIdle,
        inAttack,
        inDeath,
    }
    private BossState bossState;

    private float idleTimer;



    private Vector3 middle;
    
    private GameController gc;
    private GameObject player;

    public Boss boss;
    public float meleeDMG = 30f;
    public bool meleeDMG_Activated = false;
    public GameObject mapParent;

    private GameObject effectParent;


    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        player = GameObject.FindGameObjectWithTag("Player");
        effectParent = gameObject.transform.Find("Effects").gameObject;

        for(int c = 0; c < effectParent.transform.childCount; c++)
        {
            effectParent.transform.GetChild(c).gameObject.SetActive(false);
        }

        currentHP = maxHP;
        gc.BossHPChange(currentHP / maxHP);
        bossState = BossState.interrupted;
        middle = new Vector3(-10, transform.position.y, 30);

        GameObject firePos = gameObject.transform.Find("FirePosR").gameObject;
        firePos.transform.Find("AmmoSimVis").gameObject.SetActive(true);
        firePos = gameObject.transform.Find("FirePosL").gameObject;
        firePos.transform.Find("AmmoSimVis").gameObject.SetActive(true);

        toIdle();
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
            gc.endBoss();
            bossState = BossState.inDeath;
            Destroy(gameObject);
        }
    }
    private void AttackOpenner()
    {
        bossState = BossState.inAttack;
        int[] attackPath = new int[3];
        int[] ids = { 0, 1, 3 };
        int id = ids[Random.Range(0, ids.Length)];//3 ATTACK Openner TYPE 0-dash to middle 1 -dash to player 2- above Attack
        attackPath[0] = 0;//TODO: Change to normal "id"
        if (id == 0)
        {
            ids[0] = 1;
            ids[1] = 2;
            ids[2] = 3;
            id = ids[Random.Range(0, ids.Length)];
            attackPath[1] = 2;//TODO: Change to "id"
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
        gc.SpawnCons(findClosestYTNKZZMN(), 2, -1, 4);

        targetPos = new Vector3(targetPos.x, 100.5f, targetPos.z);
        gameObject.transform.LookAt(targetPos);
        gameObject.transform.eulerAngles = new Vector3(60, gameObject.transform.eulerAngles.y, 0);
        GameObject rotateObject = gameObject.transform.Find("Model").gameObject;
        effectParent.transform.Find("HandsTrail").gameObject.SetActive(true);


        //TODO: Attack started feedback to player with sound fx
        while (true)
        {
            meleeDMG_Activated = true;
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPos, Time.deltaTime * 30f);
            rotateObject.transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * 150);
            yield return new WaitForEndOfFrame();
            if (gameObject.transform.position == targetPos)
            {
                break;
            }
        }
        rotateObject.transform.localEulerAngles = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;
        meleeDMG_Activated = false;

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

        gc.SpawnCons(findClosestYTNKZZMN(), 2, -1, 4);

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

            firePos.transform.LookAt(player.transform.position);
            firePos.transform.localEulerAngles = new Vector3(firePos.transform.localEulerAngles.x, 90, 0);

            gameObject.transform.eulerAngles = new Vector3(0, gameObject.transform.eulerAngles.y - 90, 0);
            
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


        //TODO: Attack started feedback to player with sound fx
        effectParent.transform.Find("ShootParticleR").gameObject.SetActive(true);
        for(int c = 0; c < 3; c++)
        {
            GameObject go = new();

            firePos.transform.localPosition = new Vector3(firePos.transform.localPosition.x, firePos.transform.localPosition.y, 0 - 0.18f * c);
            firePos.transform.LookAt(player.transform.position);

            go.transform.SetPositionAndRotation(firePos.transform.position, firePos.transform.rotation);
            go.name = "BossAmmo";
            go.layer = 7;
            System.Type scriptMB = System.Type.GetType(gc.ammos[i].functionName + ",Assembly-CSharp");
            go.AddComponent(scriptMB);

            go.GetComponent<DummyMummyAmmo>().ammo = gc.ammos[i];
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

        yield return new WaitForSeconds(0.1f);
        //Left

        firePos = gameObject.transform.Find("FirePosL").gameObject;
        firePos.transform.Find("AmmoSimVis").gameObject.SetActive(true);

        for (int s = 0; s < firePos.transform.Find("AmmoSimVis").childCount; s++)
        {
            Transform traansform = firePos.transform.Find("AmmoSimVis").GetChild(s);
            traansform.localScale = new Vector3(0.1f, 0.1f, 50f);
            traansform.localPosition = new Vector3(traansform.localPosition.x, traansform.localPosition.y, 25f);
        }

        timeRemainingTOAttack = 0.2f;
        while (true)
        {
            timeRemainingTOAttack -= Time.deltaTime;
            gameObject.transform.LookAt(player.transform.position);
            gameObject.transform.eulerAngles = new Vector3(0, gameObject.transform.eulerAngles.y + 90, 0);

            yield return new WaitForEndOfFrame();
            if (timeRemainingTOAttack <= 0)
            {
                break;
            }
        }

        effectParent.transform.Find("ShootParticleL").gameObject.SetActive(true);
        for (int c = 0; c < 3; c++)
        {
            GameObject go = new();

            firePos.transform.localPosition = new Vector3(firePos.transform.localPosition.x, firePos.transform.localPosition.y, 0 - 0.18f * c);
            firePos.transform.LookAt(player.transform.position);

            go.transform.SetPositionAndRotation(firePos.transform.position, firePos.transform.rotation);
            go.name = "BossAmmo";
            go.layer = 7;
            System.Type scriptMB = System.Type.GetType(gc.ammos[i].functionName + ",Assembly-CSharp");
            go.AddComponent(scriptMB);

            go.GetComponent<DummyMummyAmmo>().ammo = gc.ammos[i];
            go.GetComponent<DummyMummyAmmo>().targetTag = "PlayerColl";
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

        gc.SpawnCons(findClosestYTNKZZMN(), 2, -1, 4);

        while (true)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(gameObject.transform.position.x, 150, gameObject.transform.position.z),Time.deltaTime * 30f);
            yield return new WaitForEndOfFrame();
            if (gameObject.transform.position.y >= 150)
            {
                break;
            }
        }
        float timeRemainingTOAttack = Random.Range(1, 4);
        GameObject indicator = gameObject.transform.Find("CylinderIndicator").gameObject;


        indicator.SetActive(true);
        while (true)
        {
            timeRemainingTOAttack -= Time.deltaTime;
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(player.transform.position.x, 150, player.transform.position.z), Time.deltaTime * 30f);
            yield return new WaitForEndOfFrame();
            if (timeRemainingTOAttack <= 0)
            {
                break;
            }
        }
        effectParent.transform.Find("toDownTrail").gameObject.SetActive(true);
        while (true)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(gameObject.transform.position.x, 100.5f, gameObject.transform.position.z), Time.deltaTime * 30f);
            indicator.transform.position = Vector3.MoveTowards(indicator.transform.position, gameObject.transform.position, Time.deltaTime * 30f);
            yield return new WaitForEndOfFrame();
            if (gameObject.transform.position.y <= 100.5f)
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

    private int findClosestYTNKZZMN()
    {
        int holdingIndex = 0;
        if(mapParent.transform.Find("ConsumableCreatePos").childCount==0 || mapParent.transform.Find("ConsumableCreatePos").childCount == 1)
        {
            return holdingIndex;
        }
        for(int c = 1; c < mapParent.transform.Find("ConsumableCreatePos").childCount ; c++)
        {
            if(Vector3.Distance(player.transform.position,mapParent.transform.Find("ConsumableCreatePos").GetChild(c).position) < Vector3.Distance(player.transform.position, mapParent.transform.Find("ConsumableCreatePos").GetChild(holdingIndex).position))
            {
                holdingIndex = c;
            }
        }
        return holdingIndex;
    }

}