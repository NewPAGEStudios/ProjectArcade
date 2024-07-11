using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //Game Main Init
    public enum GameState
    {
        pause,
        inGame
    }
    public enum PlayState
    {
        inWave,
        inWaiting,
        inBoss,
        inCinematic,
        inPlayerInterrupt
    }

    public GameState state;
    public PlayState pState;

    public Ammo[] ammos;
    public Weapon[] weapons;
    public EnemyType[] enemies;
    public Consumable[] consumables;
    public Skill[] skills;
    
    private List<Skill> activeSkills=new List<Skill>();
    private List<Skill> passiveSkills = new List<Skill>();
    private List<Skill> instantSkills = new List<Skill>();
    
    //to visualise skills for indicatng parent obj
    public GameObject skillIndicatorParent;
    public GameObject skillObject;
    public Material skillIndicatorMaterial;
    public GameObject dashVisualized;



    //UI Ref
    public GameObject playerPanel;
    //Map parent Ref
    public GameObject levelDesign;
    //spawnPoint Variables
    public GameObject spawnPointParent;
    private float spawnTimer;


    public GameObject inActiveWeapon;

    public GameObject player;


    private int waveNumber;

    [Header(header: "GameSettings")]
    public float waitTime;
    private float waitTimer;

    private void Awake()
    {
        ammos = Resources.LoadAll<Ammo>("Ammo");
        weapons = Resources.LoadAll<Weapon>("Weapon");
        enemies = Resources.LoadAll<EnemyType>("Enemy");
        consumables = Resources.LoadAll<Consumable>("Consumable");
        skills = Resources.LoadAll<Skill>("Skill");
        //leftHandTargetPosInýt



        for(int i = 0; i < weapons.Length; i++)
        {
            GameObject weaponGO = Instantiate(weapons[i].modelGameObject, inActiveWeapon.transform);
            weaponGO.name = weapons[i].WeaponName;
            weaponGO.SetActive(false);
        }
        for(int i = 0; i < skills.Length; i++)
        {
            if (skills[i].st == Skill.skillType.active)
            {
                activeSkills.Add(skills[i]);
            }
            else if (skills[i].st == Skill.skillType.instant)
            {
                instantSkills.Add(skills[i]);

            }
            else if (skills[i].st == Skill.skillType.passive)
            {
                passiveSkills.Add(skills[i]);

            }
        }


    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < levelDesign.transform.childCount; i++)
        {
            if (levelDesign.transform.GetChild(i).CompareTag("Ground"))
            {
                continue;
            }
            if (i == 15)
            {
                break;
            }
            Gizmos.color = new Color(0, 0, 0, 0.5f);
            try
            {
                Gizmos.DrawCube(levelDesign.transform.GetChild(i).GetComponent<Collider>().bounds.center, levelDesign.transform.GetChild(i).GetComponent<Collider>().bounds.size);
            }
            catch
            {
                Gizmos.DrawCube(levelDesign.transform.GetChild(i).GetComponentInChildren<Collider>().bounds.center, levelDesign.transform.GetChild(i).GetComponentInChildren<Collider>().bounds.size);
            }
        }
    }
    private void Start()
    {
        //Referances



        SpawnCons(1);
        SpawnCons(0);
        SpawnCons(0);
        waveNumber = 0;

        state = GameState.inGame;
        pState = PlayState.inWaiting;



        ChangeAmmoText(0);
        ChangefullAmmoText(0);
    }

    //    spawners
    private void SpawnCons(int consID)
    {
        int r = Random.Range(0, spawnPointParent.transform.childCount);

        Vector3 vec = spawnPointParent.transform.GetChild(r).position;

        Vector3 posOFC = new(vec.x, vec.y + 1f, vec.z);

        GameObject consumableobject = new();
        int i = 0;
        for (i = 0; i < consumables.Length; i++)
        {
            if (consumables[i].id == consID)
            {
                break;
            }
        }

        consumableobject.name = consumables[i].nameOfC;

        consumableobject.transform.parent = spawnPointParent.transform.GetChild(r);

        consumableobject.transform.position = posOFC;

        consumableobject.AddComponent(consumables[i].function.GetClass());
        
        
        
        //Manuel adding
        if (consumableobject.TryGetComponent<GetWeapon>(out GetWeapon gw))
        {
            gw.weaponID = Random.Range(0, weapons.Length);
        }
        //skills
        else if(consumableobject.TryGetComponent<GetActiveSkill>(out GetActiveSkill gas))
        {
            gas.skillId = activeSkills[Random.Range(0, activeSkills.Count)].skillTypeID;
        }
        else if(consumableobject.TryGetComponent<PerformInstantSkill>(out PerformInstantSkill pis))
        {
            pis.thisSkill = instantSkills[Random.Range(0, instantSkills.Count)];
        }
        else if(consumableobject.TryGetComponent<PerformPassiveSkill>(out PerformPassiveSkill pps))
        {
            pps.thisSkill = passiveSkills[Random.Range(0, passiveSkills.Count)];
        }
        else
        {
            Debug.Log("Bomba");
        }


        consumableobject.AddComponent<SphereCollider>();
        consumableobject.GetComponent<SphereCollider>().isTrigger = true;

        consumableobject.AddComponent<MeshFilter>();
        consumableobject.GetComponent<MeshFilter>().mesh = consumables[consID].modelMesh;

        consumableobject.AddComponent<MeshRenderer>();
        consumableobject.GetComponent<MeshRenderer>().materials = consumables[consID].mats;

        //add indicator for location guide


        spawnPointParent.transform.GetChild(r).parent = null;
        return;
    }
    public void SpawnEnemy()
    {
        GameObject enemy = new();
        enemy.AddComponent<EnemyController>();
        enemy.GetComponent<EnemyController>().m_Enemy = enemies[0];
        enemy.name = "enemy";
        //        Instantiate(enemy,new Vector3(2,0,5),new Quaternion(0,0,0,0));

    }//NOtReady



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(state == GameState.pause)
            {
                ResumeGame();
            }
            else
            {
                StopGame();
            }
        }
        if(state == GameState.inGame)
        {
            if (pState == PlayState.inWave)
            {
                if (spawnTimer <= 0f)
                {
                    SpawnCons(Random.Range(0, consumables.Length));
                    spawnTimer = Random.Range(15f, 20f);
                }
                else
                {
                    spawnTimer -= Time.deltaTime;
                }
            }
            else if(pState == PlayState.inWaiting)
            {
                if (waitTimer <= 0)
                {
                    toWave();
                }
                else
                {
                    waitTimer -= Time.deltaTime;
                }
            }
            else if(pState == PlayState.inBoss)
            {
                //getBoss object name and full health blah blah blahsaiþdlasþldþaslþdlþas,dal,sild komik mi ?
            }
            else if(pState == PlayState.inCinematic)
            {

            }
        }
    }

    //PlayStateChanger
    private void toWave()
    {
        waveNumber += 1;
        pState = PlayState.inWave;
        if (waveNumber % 10 == 0)
        {
            toBoss(0);
        }
        //difficulty


        //Spawn Enemies
    }

    private void toWait()
    {
        waitTimer = 20;
        pState = PlayState.inWaiting;
        //Enable Shops
    }

    private void toBoss(int bossID)
    {
        //teleportPlayer Routine
        //start cinematic
    }

    private void toCinematic(PlayState from)
    {
        //will be added later
    }




    //GameStatesChanger
    public void ResumeGame()
    {
        state = GameState.inGame;
    }

    public void StopGame()
    {
        state = GameState.pause;
    }






    //UI Events
    //player based UI Events
    public void ChangeAmmoText(int newAmmo)
    {
        playerPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newAmmo.ToString();
    }
    public void ChangefullAmmoText(int newAmmo)
    {
        playerPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = newAmmo.ToString();

    }
    public void changeSpriteOfActiveSkill(Sprite sprite)
    {
        playerPanel.transform.GetChild(4).GetChild(0).GetComponent<Image>().enabled = true;
        playerPanel.transform.GetChild(4).GetChild(0).GetComponent<Image>().sprite = sprite;
    }
    public void closeSpriteOfActiveSkill(Sprite sprite)
    {
        playerPanel.transform.GetChild(4).GetChild(0).GetComponent<Image>().sprite = null;
        playerPanel.transform.GetChild(4).GetChild(0).GetComponent<Image>().enabled = false;
    }
    public void DashIndicator(float dashMeter)
    {
        float lastMeter = dashMeter % 25;
        int opennedMeters = (int) dashMeter / 25;
        playerPanel.transform.GetChild(5);
        for(int i = playerPanel.transform.GetChild(5).childCount - 1; i >= 0 ; i--)
        {
            if (opennedMeters > 0)
            {
                playerPanel.transform.GetChild(5).GetChild(i).GetComponent<Image>().fillAmount = 1;
            }
            else if(opennedMeters == 0)
            {
                playerPanel.transform.GetChild(5).GetChild(i).GetComponent<Image>().fillAmount = lastMeter / 25;
            }
            else
            {
                playerPanel.transform.GetChild(5).GetChild(i).GetComponent<Image>().fillAmount = 0;
            }
            opennedMeters--;
        }
    }
    public void AddDashIndicator(float dashMeter)
    {
        float dashNumber = Mathf.Round(dashMeter / 25);
        float maxDashN = dashNumber;
        while(dashNumber > 0)
        {
            GameObject go = Instantiate(dashVisualized, playerPanel.transform.GetChild(5));
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(800-(200 * (maxDashN - dashNumber)),400);
            dashNumber--;
        }
    }



    public void MainMenu()
    {
        //SceneManagement.LoadScene(mainMenuint)
    }

    //GETTER

}
