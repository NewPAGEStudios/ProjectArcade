using Cinemachine;
using SlimUI.ModernMenu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour//TODO: Compass add cons
{
    //Game Main Init
    public enum GameState
    {
        pause,
        inShop,
        inSkillMenu,
        inGame
    }
    public enum PlayState
    {
        inStart,
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
    public Boss[] boss;
    public Perk[] perks;
    public WaveEnemyData[] waveEnemyDatas;
    [Tooltip("Drag/Release")]
    public WeaponGetAmmoData weaponGetAmmoData;


    private List<Skill> activeSkills = new List<Skill>();
    private List<Skill> passiveSkills = new List<Skill>();
    private List<Skill> instantSkills = new List<Skill>();
    
    //to visualise skills for indicatng parent obj
    public GameObject skillIndicatorParent;
    public GameObject skillObject;
    public Material skillIndicatorMaterial;
    public GameObject dashVisualized;

    [Header("Global Volume Profiles")]
    [SerializeField] private VolumeProfile mainEffect;
    [SerializeField] private VolumeProfile noiseEffect;
    [SerializeField] private VolumeProfile pauseEffect;
    [SerializeField] private Volume globalProfile;

    [Header(header: "Cameras")]
    public GameObject mainCam;
    public GameObject UIOverlayCam;
    public GameObject HandOverlayCam;
    //UI Ref
    [Header(header: "UIReference")]
    public GameObject playerPanel;
    public GameObject gamePanel;
    public GameObject bossIntroPanel;
    public GameObject bossPanel;
    public GameObject shopPanel;
    public GameObject damagePanel;

    [Header(header: "MapReference")]
    public GameObject startMap;
    public GameObject mainLevel;
    public GameObject mapParentM;
    private GameObject currentLevel;
    //spawnPoint Variables
    [HideInInspector]
    public GameObject consumableSpawnPointParent;
    [HideInInspector]
    public GameObject enemySpawnPointParent;
    [HideInInspector]
    public GameObject playerTeleportPoint;

    //Save Holders
    [HideInInspector]
    public List<int> activeCons = new List<int>();
    [HideInInspector]
    public List<int> activeConsID = new List<int>();
    [HideInInspector]
    public List<int> activeConsWeapID = new List<int>();
    [HideInInspector]
    public List<int> activeConsSkill = new List<int>();


    private float spawnTimer;//For Spawning Consumables
    private int remainingConsToSpawn = 0;
    private int remainingAmmoConsToSpawn = 0;

    [Header(header: "ControllerReferances")]
    public GameObject inActiveWeapon;
    public GameObject player;
    private LccalApplierToScriptable localizer_scp;
    [HideInInspector]
    public InputManager IManager;
    public AudioMixer audioM;


    public int waveNumber;
    private int enemyCount;

    private int comboCount;
    private float mon;
    public float money {  get { return mon; } set { mon = value; } }

    private bool newGame;

    [Header(header: "GameSettings")]
    public float waitTime;
    private float waitTimer;
    public float comboDuration;
    public float fadeSpeedDMGVisualizetion;
    public float cameraShakeIntensity;
    public float cameraShakeDuration;
    private float baseFixedUpdate;
    public int bossTimePerWave;
    public int bossTimePerWaveLoop;
    [Tooltip("PercentageOfSpawningWeapon")]
    public int ammoSpawn = 50;

    [Header(header: "UI Prefab Referances")]
    public GameObject shopButton;
    public GameObject shopTXT;
    public GameObject enemiesIndicator;
    public GameObject consSkillIndicator;
    public GameObject consAmmoIndicator;

    //IEnumerators
    private Coroutine comboDisplayRoutine;
    private Coroutine[] fWayDMGVisualize = new Coroutine[4];
    private Coroutine dmgGivenUICoroutine;
    private Coroutine dmgTakenEffectCoroutine;
    private Coroutine dashEffectCoroutine;
    private Coroutine shopInfoVis;


    //PostProcessing Settings
    AmbientOcclusion ao;

    //Compass
    float cUnit;
    List<CompassObject> cObjects = new List<CompassObject>();
    private void Awake()
    {

        globalProfile.profile = mainEffect;

        //update base
        baseFixedUpdate = Time.fixedDeltaTime;


        //MainMenu Referances

        mainCam.GetComponent<PostProcessVolume>().profile.TryGetSettings(out ao);
        if (PlayerPrefs.GetInt("AmbientOcclusion", 1) == 0)
        {
            ao.active = false;
        }
        else if (PlayerPrefs.GetInt("AmbientOcclusion", 1) == 1)
        {
            ao.active = true;
        }


        ammos = Resources.LoadAll<Ammo>("Ammo");
        weapons = Resources.LoadAll<Weapon>("Weapon");
        enemies = Resources.LoadAll<EnemyType>("Enemy");
        consumables = Resources.LoadAll<Consumable>("Consumable");
        skills = Resources.LoadAll<Skill>("Skill");
        boss = Resources.LoadAll<Boss>("Boss");
        perks = Resources.LoadAll<Perk>("Perks");
        waveEnemyDatas = Resources.LoadAll<WaveEnemyData>("WaveEnemyData");

        for (int i = 0; i < weapons.Length; i++)
        {
            //weapon GO �nstantiate
            GameObject weaponGO = Instantiate(weapons[i].modelGameObject, inActiveWeapon.transform);
            weaponGO.name = weapons[i].WeaponName;
            weaponGO.SetActive(false);

            //UI Instanttiate
            GameObject imaj = Instantiate(shopButton, shopPanel.transform.GetChild(0).GetChild(0).GetChild(0));
            imaj.AddComponent<Image>();
            imaj.GetComponent<Image>().sprite = weapons[i].UIRef;
            imaj.name = weapons[i].WeaponName + "Shop";
            imaj.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);
            //button click event
            Button but = imaj.AddComponent<Button>();
            but.targetGraphic=imaj.GetComponent<Image>();

            but.onClick.AddListener(() => weaponButtonPressed(but.gameObject.name) );
            //button hover event
            EventTrigger evtrigger = but.AddComponent<EventTrigger>();
            EventTrigger.Entry hoverEvent = new()
            {
                eventID = EventTriggerType.PointerEnter
            };
            hoverEvent.callback.AddListener((functionIwant) => { weaponButtonHover(but.gameObject.name); });
            evtrigger.triggers.Add(hoverEvent);
            //button hover out event
            EventTrigger evtrigger1 = but.AddComponent<EventTrigger>();
            EventTrigger.Entry hoveroutEvent = new()
            {
                eventID = EventTriggerType.PointerExit
            };
            hoveroutEvent.callback.AddListener((functionIwant) => { shopNameReset(); });
            evtrigger1.triggers.Add(hoverEvent);


            GameObject imajChild = Instantiate(shopTXT, imaj.transform);
            TextMeshProUGUI tmp = imajChild.AddComponent<TextMeshProUGUI>();
            tmp.color = Color.white;
            tmp.text = weapons[i].toBuyMoney.ToString();
            tmp.fontSize = 36;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.MidlineGeoAligned;
        }
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].st == Skill.skillType.active)
            {
                activeSkills.Add(skills[i]);
            }
            else if (skills[i].st == Skill.skillType.passive)
            {
                passiveSkills.Add(skills[i]);

            }
            //UI Instanttiate
            GameObject imaj = Instantiate(shopButton, shopPanel.transform.GetChild(0).GetChild(0).GetChild(0));
            imaj.AddComponent<Image>();
            imaj.GetComponent<Image>().sprite = skills[i].sprite_HUD;
            imaj.name = "id_" + skills[i].skillTypeID + "Shop";
            imaj.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);
            //button click event
            Button but = imaj.AddComponent<Button>();
            but.targetGraphic = imaj.GetComponent<Image>();

            but.onClick.AddListener(() => skillButtonPressed(but.gameObject.name));
            //button hover event
            EventTrigger evtrigger = but.AddComponent<EventTrigger>();
            EventTrigger.Entry hoverEvent = new()
            {
                eventID = EventTriggerType.PointerEnter
            };
            hoverEvent.callback.AddListener((functionIwant) => { skillButtonHover(but.gameObject.name); });
            evtrigger.triggers.Add(hoverEvent);
            //button hover out event
            EventTrigger evtrigger1 = but.AddComponent<EventTrigger>();
            EventTrigger.Entry hoveroutEvent = new()
            {
                eventID = EventTriggerType.PointerExit
            };
            hoveroutEvent.callback.AddListener((functionIwant) => { shopNameReset(); });
            evtrigger1.triggers.Add(hoverEvent);

            GameObject imajChild = Instantiate(shopTXT, imaj.transform);
            TextMeshProUGUI tmp = imajChild.AddComponent<TextMeshProUGUI>();
            tmp.color = Color.white;
            tmp.text = skills[i].toBuyMoney.ToString();
            tmp.fontSize = 36;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.MidlineGeoAligned;
        }
        //spawnMaps;
        for (int i = 0; i < boss.Length; i++)
        {
            GameObject go = Instantiate(boss[i].mapParent,mapParentM.transform);
            go.name = boss[i].mapName;
            go.SetActive(false);
        }

        localizer_scp = GetComponent<LccalApplierToScriptable>();

        //Runtime Infor holder Init
        player.GetComponent<WeaponManager>().holder = new WeaponRuntimeHolder[weapons.Length];


        for (int i = 0; i < player.GetComponent<WeaponManager>().holder.Length; i++)
        {
            player.GetComponent<WeaponManager>().holder[i] = new WeaponRuntimeHolder(weapons[i].WeaponTypeID, weapons[i].magSize);
        }


        newGame = PlayerPrefs.GetInt("newGame") == 1 ? true : false;

    }

    private void Start()
    {
        //optionInitializition;
        gamePanel.transform.GetChild(4).GetChild(1).GetComponent<InGameSettings>().initValues();
        GetComponent<CheckMusicVolume>().UpdateVolume();

        if (newGame)
        {
            state = GameState.inGame;
            pState = PlayState.inStart;

            //startLevel
            currentLevel = startMap;
            changeMap(currentLevel);

            player.transform.position = playerTeleportPoint.transform.position;
            player.GetComponent<PController>().HealDMG(1, gameObject);

            SpawnCons(0, 0, 0, -1);
            waveVisualzie("Get the weapon");
            waveNumber = 0;

            waitTimeVisualize(-1);

            //UI INIT
            ChangeAmmoText(-1, false);
            ChangeVisibilityofSlash(false);
            ChangefullAmmoText(-1,false);

            ComboBG(0);
            ComboVisualize(0);


            //Money Handling
            money = 0;
            MoneyDisplay();
        }
        else
        {
            state = GameState.inGame;
            pState = PlayState.inStart;
    
            currentLevel = startMap;
            DefaultMap();

            ComboBG(0);
            ComboVisualize(0);

            Invoke(nameof(LoadElements), Time.deltaTime);
        }

        cUnit = gamePanel.transform.GetChild(6).GetChild(1).GetComponent<RawImage>().rectTransform.rect.width / 360f;

        //Cursor Handling
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        /* Save on waitState
        check save and load it save elements =  
        player,char's ammo,weapon,ability,hp,dashMeter,Money,,
        gameController, pickable cons and weapons pos andID, waveNumber, 
         
         
         */
    }

    //spawners
    public void SpawnCons(int pos_childID,int consID,int weaponID,int skillID)
    {
        if (consumableSpawnPointParent.transform.childCount == 0)
        {
            return;
        }

        Vector3 vec = Vector3.zero;
        int r;
        int pos_Holder = -1;
        if (pos_childID == -1 || activeCons.Contains(pos_childID))
        {
            r = UnityEngine.Random.Range(0, consumableSpawnPointParent.transform.childCount);
            if(pState != PlayState.inBoss)
            {
                activeCons.Add(consumableSpawnPointParent.transform.GetChild(r).GetComponent<PosIDStorage>().posID);
            }
            pos_Holder = consumableSpawnPointParent.transform.GetChild(r).GetComponent<PosIDStorage>().posID;
            vec = consumableSpawnPointParent.transform.GetChild(r).position;
        }
        else
        {
            r = pos_childID;
            if(consumableSpawnPointParent.transform.childCount==0)
            {
                return;
            }
            for(int count = 0; count < consumableSpawnPointParent.transform.childCount; count++)
            {
                if (consumableSpawnPointParent.transform.GetChild(count).GetComponent<PosIDStorage>().posID == pos_childID)
                {
                    if (pState != PlayState.inBoss)
                    {
                        activeCons.Add(pos_childID);
                    }
                    pos_Holder = pos_childID;
                    vec = consumableSpawnPointParent.transform.GetChild(count).position;
                }
            }
            
        }



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

        consumableobject.transform.parent = consumableSpawnPointParent.transform.GetChild(r);

        consumableobject.transform.position = posOFC;

        System.Type scriptMB = System.Type.GetType(consumables[i].functionName + ",Assembly-CSharp");

        if (pState != PlayState.inBoss)
        {
            activeConsID.Add(consID);
        }

        consumableobject.AddComponent(scriptMB);

        
        //Manuel adding
        if (consumableobject.TryGetComponent<GetWeapon>(out GetWeapon gw))
        {
            if (weaponID == -1)
            {
                gw.weaponID = UnityEngine.Random.Range(0, weapons.Length);
            }
            else
            {
                gw.weaponID = weaponID;
            }
            gw.consPosID = pos_Holder;
            if (pState != PlayState.inBoss)
            {
                activeConsWeapID.Add(gw.weaponID);
                activeConsSkill.Add(-1);
            }
            //compassVisualition
            CompassSpawnAmmo(gw.transform.gameObject);
        }
        //skills
        else if(consumableobject.TryGetComponent<GetActiveSkill>(out GetActiveSkill gas))
        {
            if (skillID == -1)
            {
                gas.skillId = activeSkills[UnityEngine.Random.Range(0, activeSkills.Count)].skillTypeID;
            }
            else
            {
                gas.skillId = skillID;
            }
            gas.consPosID = pos_Holder;
            if (pState != PlayState.inBoss)
            {
                activeConsWeapID.Add(-1);
                activeConsSkill.Add(gas.skillId);
            }
            //compassVisualition
            CompassSpawnSkill(gas.transform.gameObject,skillID);
        }
        else if(consumableobject.TryGetComponent<PerformPassiveSkill>(out PerformPassiveSkill pps))
        {
            if (skillID == -1)
            {
                pps.thisSkill = passiveSkills[UnityEngine.Random.Range(0, passiveSkills.Count)];
            }
            else
            {
                for (int s = 0; s < passiveSkills.Count; s++)
                {
                    if (skillID == passiveSkills[s].skillTypeID)
                    {
                        pps.thisSkill = passiveSkills[s];
                    }
                }
            }
            pps.consPosID = pos_Holder;
            if (pState != PlayState.inBoss)
            {
                activeConsWeapID.Add(-1);
                activeConsSkill.Add(pps.thisSkill.skillTypeID);
            }
            //compassVisualition
            CompassSpawnSkill(pps.transform.gameObject,skillID);
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


        consumableSpawnPointParent.transform.GetChild(r).parent = null;
        return;
    }
    public void SpawnEnemy(int enemyID)
    {
        GameObject enemy = new();
        enemyCount += 1;

        int indexOfID = 0;

        for(int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].EnemyTypeID == enemyID)
            {
                indexOfID = i;
                break;
            }
        }


        Transform p = enemySpawnPointParent.transform.GetChild(UnityEngine.Random.Range(0, enemySpawnPointParent.transform.childCount));
        
        float x = UnityEngine.Random.Range(p.transform.Find("min").position.x, p.transform.Find("max").position.x);
        float y = 0;

        if (enemies[indexOfID].isFlyable)
        {
            y = 0;
        }
        else
        {
            y = p.position.y + enemies[indexOfID].modelGameObject.transform.localScale.y;
        }
        
        float z = UnityEngine.Random.Range(p.transform.Find("min").position.z, p.transform.Find("max").position.z);

        enemy.transform.position = new Vector3(x, y, z);

        enemy.AddComponent<Enemy>();
        enemy.GetComponent<Enemy>().e_type = enemies[indexOfID];
        enemy.GetComponent<Enemy>().parentSelectedPosition = p;



        enemy.name = "enemy";
        enemy.tag = "Enemy";
        CompassSpawnEnemy(enemy);
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(state == GameState.pause)
            {
                ResumeGame();
            }
            else if(state == GameState.inShop)
            {
                shopCloserfunc();
            }
            else if(state == GameState.inGame)
            {
                StopGame();
            }
        }
        if(state == GameState.inGame)
        {
            if (pState == PlayState.inStart)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    player.GetComponent<PController>().TakeDMG(5, gameObject);
                }
                return;
            }
            else if (pState == PlayState.inWave)
            {
                //ConsuambleSpawn
                if (spawnTimer <= 0f)
                {
                    if (remainingConsToSpawn > 0)
                    {
                        if (UnityEngine.Random.Range(0, 100) < ammoSpawn && remainingAmmoConsToSpawn > 0)
                        {

                            SpawnCons(-1, 0, UnityEngine.Random.Range(0, weapons.Length), -1);
                            remainingAmmoConsToSpawn -= 1;
                        }
                        else if (remainingAmmoConsToSpawn == remainingConsToSpawn)
                        {

                            SpawnCons(-1, 0, UnityEngine.Random.Range(0, weapons.Length), -1);
                            remainingAmmoConsToSpawn -= 1;
                        }
                        else
                        {
                            SpawnCons(-1, UnityEngine.Random.Range(1, consumables.Length), -1, -1);
                        }
                        remainingConsToSpawn -= 1;
                        spawnTimer = UnityEngine.Random.Range(4f, 8f);
                    }
                }
                else
                {
                    spawnTimer -= Time.deltaTime;
                }
                if (enemyCount == 0)
                {
                    toWait();
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
                    waitTimeVisualize(waitTimer);
                }
            }
            else if(pState == PlayState.inBoss)
            {

            }
            else if(pState == PlayState.inCinematic)
            {

            }
        }
    }

    //PlayStateChanger
    public void escapeStart()
    {
        DefaultMap();
        toWave();
    }
    public void endBoss(GameObject bossObject)
    {
        bossPanel.SetActive(false);
        toCinematic(pState, PlayState.inWaiting, bossObject.GetComponent<Animator>(), bossObject.GetComponentInChildren<Camera>(),bossObject);
    }
    private void toWave()
    {
        waveNumber += 1;
        if (waveNumber % bossTimePerWave == bossTimePerWaveLoop)
        {
            toBoss(0);
            return;
        }
        else
        {
            pState = PlayState.inWave;
        }

        waitTimeVisualize(waitTimer);
        waveVisualzie("Wave " + waveNumber);


        for (int i = 0; i < currentLevel.transform.Find("Shops").childCount; i++)
        {
            currentLevel.transform.Find("Shops").GetChild(i).GetComponent<Shop>().close();
        }

        //WaveConfigiration
        //EnemySpawn
        foreach (WaveEnemyData wed in waveEnemyDatas)
        {
            if (wed.waveNumber == waveNumber)
            {
                for(int c = 0; c < wed.enemyTypes.Length; c++)//searching Enemy Typs
                {
                    for(int k = 0; k < wed.enemyTypes[c].enemyPiece; k++)//Indexing EnemyCounts
                    {
                        SpawnEnemy(wed.enemyTypes[c].enemyId);
                    }
                }
                //ConsSpawn
                remainingConsToSpawn = wed.maxConsSpawn;
                remainingAmmoConsToSpawn = wed.maxAmmoSpawn;
                break;
            }
        }
    }
    public void callToWait()
    {
        toWait();
    }
    private void toWait()
    {
        SaveElements();

        waitTimer = waitTime;
        pState = PlayState.inWaiting;


        for (int i = 0; i < currentLevel.transform.Find("Shops").childCount; i++)
        {
            currentLevel.transform.Find("Shops").GetChild(i).GetComponent<Shop>().open();
        }

        waitTimeVisualize(waitTimer);
        waveVisualzie("Wait");

        player.transform.GetChild(1).Find("Musics").Find("MusicWait").GetComponent<AudioSource>().Play();
    }

    private void toBoss(int bossID)
    {
        pState = PlayState.inBoss;
        int i;
        for (i = 0; i < boss.Length; i++)
        {
            if (boss[i].BossID == bossID)
            {
                break;
            }
        }
        GameObject map = mapParentM.transform.Find(boss[i].mapName).gameObject;
        
        changeMap(map);

        GameObject go = Instantiate(boss[i].boss, map.transform.position + new Vector3(0,.5f,0), Quaternion.identity);

        System.Type script = System.Type.GetType(boss[i].bossControllerName + ",Assembly-CSharp");
        go.AddComponent(script);

        //ManuelAdding
        if(go.TryGetComponent<DummyMummyFunc>(out DummyMummyFunc dmf))
        {
            dmf.boss = boss[i];
            dmf.mapParent = map;     
        }
        bossIntroPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = boss[i].bossName;
        bossPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = boss[i].bossName;


        toCinematic(pState, PlayState.inBoss, go.GetComponent<Animator>(), go.GetComponentInChildren<Camera>(),null);

    }
    private void toCinematic(PlayState from,PlayState to,Animator anim,Camera animCam,GameObject targetting)
    {
        pState = PlayState.inCinematic;
        //DisableUI
        playerPanel.SetActive(false);
        for(int i = 0; i < gamePanel.transform.childCount; i++)
        {
            gamePanel.transform.GetChild(i).gameObject.SetActive(false);
        }


        //cam Change to cinematic Camera
        mainCam.GetComponent<Camera>().enabled = false;
        animCam.enabled = true;
        
        
        if(from == PlayState.inBoss && to == PlayState.inBoss)
        {
            anim.SetTrigger("Start");
            StartCoroutine(bossStartAnim(anim,animCam));
        }
        else if(from == PlayState.inBoss && to == PlayState.inWaiting)
        {
            anim.SetTrigger("Finish");
            StartCoroutine(bossEndAnim(anim, animCam,targetting));
        }
        //Addable
    }

    //MapChanges
    public void changeMap(GameObject mapParent)
    {
        currentLevel = mapParent;
        currentLevel.SetActive(true);
        consumableSpawnPointParent = currentLevel.transform.Find("ConsumableCreatePos").gameObject;
        playerTeleportPoint = currentLevel.transform.Find("PlayerTeleportPoint").gameObject;

        player.transform.position = playerTeleportPoint.transform.position;
        mainCam.transform.localPosition = Vector3.zero;
    }
    public void DefaultMap()
    {
        currentLevel.SetActive(false);
        currentLevel = mainLevel;
        consumableSpawnPointParent = mainLevel.transform.Find("ConsumableCreatePos").gameObject;
        enemySpawnPointParent = mainLevel.transform.Find("EnemySpawnPosParent").gameObject;
        playerTeleportPoint = mainLevel.transform.Find("PlayerTeleportPoint").gameObject;

        player.transform.position = playerTeleportPoint.transform.position;

    }

    //GameStatesChanger
    public void ResumeGame()
    {
        if (gamePanel.transform.GetChild(4).GetChild(0).gameObject.activeInHierarchy)
        {
            state = GameState.inGame;

            gamePanel.transform.GetChild(4).gameObject.SetActive(false);
            Time.timeScale = 1f;

            if (pState == PlayState.inWaiting)
            {
                player.transform.GetChild(1).Find("Musics").Find("MusicWait").GetComponent<AudioSource>().UnPause();
            }

            globalProfile.profile = mainEffect;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            gamePanel.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
            gamePanel.transform.GetChild(4).GetChild(1).gameObject.SetActive(false);
        }
    }

    public void StopGame()
    {
        state = GameState.pause;
        gamePanel.transform.GetChild(4).gameObject.SetActive(true);
        Time.timeScale = 0f;

        if(pState == PlayState.inWaiting)
        {
            player.transform.GetChild(1).Find("Musics").Find("MusicWait").GetComponent<AudioSource>().Pause();
        }

        globalProfile.profile = pauseEffect;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void EndGame()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime *= Time.timeScale;
        
        state = GameState.pause;
        pState = PlayState.inPlayerInterrupt;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(endGameEffect());
    }

    //SaveSystem
    public void SaveElements()
    {
        SaveSystem.SavePlayer(player.GetComponent<PController>(), player.GetComponent<WeaponManager>(), GetComponent<GameController>());
    }
    public void LoadElements()
    {
        DataElem de = SaveSystem.LoadPlayer();
        if (de != null)
        {
            //gameController
            for(int c = 0; c < de.consID.Length; c++)
            {
                SpawnCons(de.consPosID[c], de.consID[c], de.cons_weap_ID[c], de.cons_skill_ID[c]);
            }

            money = 0;
            AddMainCurrency(de.money);

            waveNumber = de.wave_Number;

            //PController
            player.GetComponent<PController>().currentHP = de.currentHP;
            player.GetComponent<PController>().setMaxHP(de.maxHP , -1);//TODO will be changed with perk sysytem
            changeHPOfPlayer(de.maxHP, de.currentHP);

            player.GetComponent<PController>().currentdashMeter = de.dashMeter;
            DashIndicator(de.dashMeter);

            bool ot_event = true;
            //weaponManager
            for(int w = 0; w < player.GetComponent<WeaponManager>().holder.Length; w++)
            {
                player.GetComponent<WeaponManager>().holder[w].weaponTypeID = de.weap_wrh_id[w];
                player.GetComponent<WeaponManager>().holder[w].sum_ammoAmount = de.weap_wrh_sumAmmo[w];
                player.GetComponent<WeaponManager>().holder[w].inWeapon_ammoAmount = de.weap_wrh_inWeaponAmmo[w];
                player.GetComponent<WeaponManager>().holder[w].maxMagAmount = de.weap_wrh_maxMagAmount[w];
                if (de.weap_wrh_isOwned[w] == 1)
                {
                    player.GetComponent<WeaponManager>().holder[w].isOwned = true;
                    if (ot_event)
                    {
                        player.GetComponent<WeaponManager>().ChangeWeapon(de.weap_wrh_id[w]);
                        ot_event = false;
                    }
                }
                else if (de.weap_wrh_isOwned[w] == 0)
                {
                    player.GetComponent<WeaponManager>().holder[w].isOwned = false;
                }
            }
            if (de.activeSkill_ID != -1)
            {
                for (int sk = 0; sk < skills.Length; sk++)
                {
                    if (de.activeSkill_ID == skills[sk].skillTypeID)
                    {
                        player.GetComponent<WeaponManager>().getSkill(skills[sk]);
                    }
                }
            }
            else
            {
                player.GetComponent<WeaponManager>().active_Skill = null;
            }
            List<Skill> lst = new();
            lst = skills.ToList();
            for(int count = 0; count < de.stockedSkill_ID.Length; count++)
            {
                player.GetComponent<WeaponManager>().stocked_Skills.Add(lst.Find(x => x.skillTypeID == de.stockedSkill_ID[count]));
            }
        }
        toWait();
    }



    //Animations
    IEnumerator waveStartAnim()//UI//TODO:Düzenle
    {
        TextMeshProUGUI tmp = gamePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Vector3 tmp_truePos = tmp.rectTransform.localPosition;
        tmp.rectTransform.localPosition = Vector3.zero;
        tmp.fontSize = 144;
        float speedAnim = 250f;
        while (true)
        {
            tmp.rectTransform.localPosition = Vector3.MoveTowards(tmp.rectTransform.localPosition, tmp_truePos,Time.deltaTime * speedAnim);
            tmp.fontSize = Mathf.MoveTowards(tmp.fontSize, 36, Time.deltaTime * speedAnim);
            yield return new WaitForEndOfFrame();
            if (tmp.rectTransform.localPosition == tmp_truePos && tmp.fontSize == 36)
            {
                break;
            }
        }
    }
    IEnumerator bossStartAnim(Animator selectedAnim,Camera selectedCam)
    {
        while (true) 
        {
            yield return new WaitForEndOfFrame();
            if (selectedAnim.GetCurrentAnimatorStateInfo(0).IsName("end_start"))
            {
                bossIntroPanel.SetActive(true);
                break;
            }
        }
        yield return new WaitForSeconds(0.5f);
        //StopAnimation

        selectedAnim.SetBool("end", true);
        yield return new WaitForEndOfFrame();
        selectedAnim.SetBool("end", false);

        //UI Normalize
        bossIntroPanel.SetActive(false);
        for (int i = 0; i < gamePanel.transform.childCount; i++)
        {
            if(i == 2 || i == 3 || i == 4 || i == 5)
            {
                continue;
            }
            gamePanel.transform.GetChild(i).gameObject.SetActive(false);
        }
        playerPanel.SetActive(true);
        bossPanel.SetActive(true);

        //camNormalize
        mainCam.GetComponent<Camera>().enabled = true;
        selectedCam.enabled = false;

        //change GameState
        pState = PlayState.inBoss;
    }
    IEnumerator bossEndAnim(Animator selectedAnim, Camera selectedCam,GameObject willDestroyObj)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (selectedAnim.GetCurrentAnimatorStateInfo(0).IsName("end_finish"))
            {
                break;
            }
        }
        yield return new WaitForSeconds(0.3f);
        //StopAnimation

        selectedAnim.SetBool("FinishEnd", true);
        yield return new WaitForEndOfFrame();
        selectedAnim.SetBool("FinishEnd", false);

        for (int i = 0; i < gamePanel.transform.childCount; i++)
        {
            if (i == 2 || i == 3 || i == 4 || i == 5)
            {
                continue;
            }
            gamePanel.transform.GetChild(i).gameObject.SetActive(true);
        }
        playerPanel.SetActive(true);
        bossPanel.SetActive(false);

        //camNormalize
        mainCam.GetComponent<Camera>().enabled = true;
        selectedCam.enabled = false;

        Destroy(willDestroyObj);
        currentLevel.transform.Find("MapChangerDoor").gameObject.SetActive(true);

        //change GameState
        pState = PlayState.inBoss;
    }
    //UI Events
    //GameBasedUI Evvents
    public void waitTimeVisualize(float timer)
    {
        if (timer < 0)
        {
            gamePanel.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            gamePanel.transform.GetChild(0).gameObject.SetActive(true);
            timer = Mathf.Ceil(timer);
            gamePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = timer.ToString();
        }
    }
    public void waveVisualzie(string waveIndicator)
    {
        gamePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = waveIndicator;
        StartCoroutine(waveStartAnim());
    }
    public void ComboVisualize(int combo)
    {
        if (comboCount <= 0)
        {
            gamePanel.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            gamePanel.transform.GetChild(2).gameObject.SetActive(true);
            gamePanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = comboCount + " Combo";
        }
    }
    public void ComboBG(float fa)
    {
        gamePanel.transform.GetChild(2).GetComponent<Image>().fillAmount = fa;
    }
    public void ShopOpener()
    {
        if (pState == PlayState.inWaiting)
        {
            player.transform.GetChild(1).Find("Musics").Find("MusicWait").GetComponent<AudioSource>().Pause();
        }
        playerPanel.SetActive(false);
        gamePanel.SetActive(false);
        shopPanel.SetActive(true);
    }
    public void ShopCloser()
    {
        if (pState == PlayState.inWaiting)
        {
            player.transform.GetChild(1).Find("Musics").Find("MusicWait").GetComponent<AudioSource>().UnPause();
        }
        playerPanel.SetActive(true);
        gamePanel.SetActive(true);
        shopPanel.SetActive(false);
    }
    public void ShopInfo(string infoStr)
    {
        shopPanel.transform.GetChild(0).Find("Info").GetComponent<TextMeshProUGUI>().text = infoStr;
        shopPanel.transform.GetChild(0).Find("Info").GetComponent<TextMeshProUGUI>().color = Color.black;
        shopPanel.transform.GetChild(0).Find("InfoBG").GetComponent<Image>().color = new Color(0.03137255f, 0.09803922f, 0.1098039f, 0.7843137f);
        if (shopInfoVis != null)
        {
            StopCoroutine(shopInfoVis);
        }
        shopInfoVis = StartCoroutine(ShopInfoRoutine());
    }
    IEnumerator ShopInfoRoutine()
    {
        TextMeshProUGUI tmpInfo = shopPanel.transform.GetChild(0).Find("Info").GetComponent<TextMeshProUGUI>();
        Image ImInfo = shopPanel.transform.GetChild(0).Find("InfoBG").GetComponent<Image>();
        while (true)
        {
            tmpInfo.color = new Color(tmpInfo.color.r, tmpInfo.color.g, tmpInfo.color.b, Mathf.MoveTowards(tmpInfo.color.a, 0, 0.002f * 10));
            ImInfo.color = new Color(ImInfo.color.r, ImInfo.color.g, ImInfo.color.b, Mathf.MoveTowards(ImInfo.color.a, 0, 0.002f * 10));
            yield return new WaitForSecondsRealtime(0.002f);
            if(tmpInfo.color.a <= 0 && ImInfo.color.a <= 0)
            {
                break;
            }
        }
    }

    public void CompassVisualize(float value)
    {
        gamePanel.transform.GetChild(6).GetChild(1).GetComponent<RawImage>().uvRect = new Rect(value/360f,0f,1f,1f);

        foreach(CompassObject cobject in cObjects)
        {
            Vector2 pPos = new Vector2(player.transform.position.x, player.transform.position.z);
            Vector2 pFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

            float angle = Vector2.SignedAngle(new Vector2(cobject.irTransform.position.x, cobject.irTransform.position.z) - pPos, pFwd);

            cobject.uiTransform.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(cUnit * angle, 0f);
        }
    }
    public void CompassSpawnEnemy(GameObject enemi)
    {
        GameObject uiGO = Instantiate(enemiesIndicator, gamePanel.transform.GetChild(6).GetChild(1));
        var compassObject = new CompassObject(ir: enemi.transform, ui: uiGO.transform);
        cObjects.Add(compassObject);
    }
    public void CompassSpawnSkill(GameObject skillGO, int skillid)
    {
        GameObject uiGO = Instantiate(consSkillIndicator, gamePanel.transform.GetChild(6).GetChild(1));
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].skillTypeID == skillid)
            {
                uiGO.GetComponent<Image>().sprite = skills[i].sprite_HUD;
            }
        }
        var compassObject = new CompassObject(ir: skillGO.transform, ui: uiGO.transform);
        cObjects.Add(compassObject);
    }
    public void CompassSpawnAmmo(GameObject ammo)
    {
        GameObject uiGO = Instantiate(consAmmoIndicator, gamePanel.transform.GetChild(6).GetChild(1));
        var compassObject = new CompassObject(ir: ammo.transform, ui: uiGO.transform);
        cObjects.Add(compassObject);
    }
    //player based UI Events

    public void changeHPOfPlayer(float maxH, float currentH)
    {
        playerPanel.transform.GetChild(6).GetChild(0).GetComponent<Image>().fillAmount = currentH/maxH;
        playerPanel.transform.GetChild(6).GetChild(1).GetComponent<TextMeshProUGUI>().text = currentH.ToString() + "/" + maxH.ToString();
    }
    public void ChangeAmmoText(int? newAmmo = -1, bool? value = true)
    {
        playerPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newAmmo.ToString();
        playerPanel.transform.GetChild(0).transform.gameObject.SetActive(value.Value);
    }


    
    public void ChangeVisibilityofSlash(bool value)
    {
        playerPanel.transform.GetChild(1).gameObject.SetActive(value);
    }
    public void ChangefullAmmoText(int? newAmmo = -1, bool? value = true)
    {
         playerPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = newAmmo.ToString();
         playerPanel.transform.GetChild(2).transform.gameObject.SetActive(value.Value);
    }

    public void ChangeActiveWeapon(int id)
    {
        for (int c = 0; c < playerPanel.transform.GetChild(9).childCount; c++)
        {
            if(c == id)
            {
                playerPanel.transform.GetChild(9).GetChild(c).gameObject.SetActive(true);
            }
            else
            {
                playerPanel.transform.GetChild(9).GetChild(c).gameObject.SetActive(false);
            }
        }
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
    public void BossHPChange(float fa)
    {
        bossPanel.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = fa;
        RectTransform rt = bossPanel.transform.GetChild(2).GetChild(0).GetComponent<Image>().rectTransform;
        rt.localPosition = new Vector3(rt.localPosition.x, fa * 100, rt.localPosition.z);
    }
    public void MoneyDisplay()
    {
        playerPanel.transform.GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = money.ToString() + " $";
    }
    public void DisplayInstruction(bool display)
    {
        gamePanel.transform.GetChild(3).gameObject.SetActive(display);
    }
    public void HandleDMGtakenUI(int childNumber)//0:back 1:left 2:rigth 3:front
    {
        if (fWayDMGVisualize[childNumber] != null)
        {
            StopCoroutine(fWayDMGVisualize[childNumber]);
        }
        fWayDMGVisualize[childNumber] = StartCoroutine(DMG_UI_Routine(damagePanel.transform.GetChild(childNumber).gameObject));
    }
    IEnumerator DMG_UI_Routine(GameObject targetGo)
    {
        targetGo.SetActive(true);
        Image referanceOFImage = targetGo.GetComponent<Image>();
        referanceOFImage.color = new Color(referanceOFImage.color.r, referanceOFImage.color.g, referanceOFImage.color.b, 0.5f);
        while (true)
        {
            referanceOFImage.color = new Color(referanceOFImage.color.r, referanceOFImage.color.g, referanceOFImage.color.b, Mathf.MoveTowards(referanceOFImage.color.a, 0, Time.deltaTime * fadeSpeedDMGVisualizetion));
            yield return new WaitForEndOfFrame();
            if (referanceOFImage.color.a <= 0)
            {
                break;
            }
        }
        targetGo.SetActive(false);
    }
    public void HandleDmgGiven()
    {
        if (dmgGivenUICoroutine != null)
        {
            StopCoroutine(dmgGivenUICoroutine);
        }
        dmgGivenUICoroutine = StartCoroutine(DMG_UI_Give_Routine());
    }
    IEnumerator DMG_UI_Give_Routine()
    {
        Image[] ima = new Image[playerPanel.transform.GetChild(8).childCount];
        Debug.Log("rAMBABA");
        for (int z = 0; z < playerPanel.transform.GetChild(8).childCount; z++)
        {
            ima[z] = playerPanel.transform.GetChild(8).GetChild(z).GetComponent<Image>();
            ima[z].color = new Color(ima[z].color.r, ima[z].color.g, ima[z].color.b, 0.4F);
        }
        while (true)
        {
            ima[0].color = new Color(ima[0].color.r, ima[0].color.g, ima[0].color.b, Mathf.MoveTowards(ima[0].color.a, 0f, Time.deltaTime * fadeSpeedDMGVisualizetion));
            ima[1].color = new Color(ima[1].color.r, ima[1].color.g, ima[1].color.b, Mathf.MoveTowards(ima[1].color.a, 0f, Time.deltaTime * fadeSpeedDMGVisualizetion));
            ima[2].color = new Color(ima[2].color.r, ima[2].color.g, ima[2].color.b, Mathf.MoveTowards(ima[2].color.a, 0f, Time.deltaTime * fadeSpeedDMGVisualizetion));
            ima[3].color = new Color(ima[3].color.r, ima[3].color.g, ima[3].color.b, Mathf.MoveTowards(ima[3].color.a, 0f, Time.deltaTime * fadeSpeedDMGVisualizetion));
            yield return new WaitForEndOfFrame();
            if (ima[0].color.a <= 0 || ima[1].color.a <= 0 || ima[2].color.a <= 0 || ima[3].color.a <= 0)
            {
                break;
            }
        }
    }
    //Skilll menu bombaaaaaaaaaaaaaa
    public void openSkillMenu()
    {
        state = GameState.inSkillMenu;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gamePanel.transform.GetChild(7).gameObject.SetActive(true);

        Color col = gamePanel.transform.GetChild(7).GetComponent<Image>().color;
        gamePanel.transform.GetChild(7).GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0);
        StartCoroutine(skillMenuOpenAnim(col));
        //TODO: place skills
        skillFullfillSkillMenu(0, 8);
    }
    public void skillFullfillSkillMenu(int startInt, int endInt)
    {
        for (int count=0; count < gamePanel.transform.GetChild(7).GetChild(1).childCount; count++)
        {
            gamePanel.transform.GetChild(7).GetChild(1).GetChild(count).GetComponent<Button>().interactable = false;
            gamePanel.transform.GetChild(7).GetChild(1).GetChild(count).GetChild(0).gameObject.SetActive(false);
        }
        for (int c = startInt; c < endInt; c++)
        {
            if(c == player.GetComponent<WeaponManager>().stocked_Skills.Count)
            {
                break;
            }
            gamePanel.transform.GetChild(7).GetChild(1).GetChild(c - startInt).GetComponent<Button>().interactable = true;
            gamePanel.transform.GetChild(7).GetChild(1).GetChild(c - startInt).GetChild(0).gameObject.SetActive(true);


            gamePanel.transform.GetChild(7).GetChild(1).GetChild(c - startInt).GetChild(0).GetComponent<Image>().sprite = player.GetComponent<WeaponManager>().stocked_Skills[c].sprite_HUD;
        }
    }
    public void changePageSkillMenu(int multiplier)
    {
        multiplier -= 1;
        skillFullfillSkillMenu(0 + (8 * multiplier), 8 + (8 * multiplier));
    }
    IEnumerator skillMenuOpenAnim(Color bgColor)
    {
        float frametime = Time.deltaTime;

        int pageNumber = 1;
        gamePanel.transform.GetChild(7).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

        int pageCount = (player.GetComponent<WeaponManager>().stocked_Skills.Count / 8) + 1;

        while (true)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 0f, frametime * 10);
            Time.fixedDeltaTime *= Time.timeScale;
            gamePanel.transform.GetChild(7).GetComponent<Image>().color = new Color(bgColor.r, bgColor.g, bgColor.b, Mathf.MoveTowards(gamePanel.transform.GetChild(7).GetComponent<Image>().color.a, bgColor.a, frametime * 20));



            if(IManager.getMouseScroll() > 0)
            {
                pageNumber++;
                if(pageNumber >= pageCount + 1)
                {
                    pageNumber = 1;
                }
                changePageSkillMenu(pageNumber);
                gamePanel.transform.GetChild(7).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

                gamePanel.transform.GetChild(7).GetChild(1).GetComponent<skillMenuControll>().pageChangeSkillID();
            }
            else if (IManager.getMouseScroll() < 0)
            {
                pageNumber--;
                if (pageNumber <= 0)
                {
                    pageNumber = pageCount;
                }
                changePageSkillMenu(pageNumber);
                
                gamePanel.transform.GetChild(7).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

                gamePanel.transform.GetChild(7).GetChild(1).GetComponent<skillMenuControll>().pageChangeSkillID();
            }

            if (!IManager.skillMenu)
            {
                closeSKillMenu(gamePanel.transform.GetChild(7).GetChild(1).GetComponent<skillMenuControll>().currentSkill + (8 * (pageNumber - 1)));
                break;
            }

            skillMenuTXT(gamePanel.transform.GetChild(7).GetChild(1).GetComponent<skillMenuControll>().currentSkill + (8 * (pageNumber - 1)));

            if (gamePanel.transform.GetChild(7).GetComponent<Image>().color.a == bgColor.a)
            {

            }

            if (Time.timeScale == 0)
            {
                break;
            }
            yield return new WaitForSecondsRealtime(frametime);
        }
        StartCoroutine(skillMenuWaitInput(pageNumber,frametime));
    }
    IEnumerator skillMenuWaitInput(int pageNumber,float frametime)
    {
        int pageCount = (int)Mathf.Ceil((float)player.GetComponent<WeaponManager>().stocked_Skills.Count / 8f);

        while (true)
        {
            if (IManager.getMouseScroll() > 0)
            {
                pageNumber++;
                if (pageNumber >= pageCount + 1)
                {
                    pageNumber = 1;
                }
                changePageSkillMenu(pageNumber);
                gamePanel.transform.GetChild(7).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

                gamePanel.transform.GetChild(7).GetChild(1).GetComponent<skillMenuControll>().pageChangeSkillID();
            }
            else if (IManager.getMouseScroll() < 0)
            {
                pageNumber--;
                if (pageNumber <= 0)
                {
                    pageNumber = pageCount;
                }
                changePageSkillMenu(pageNumber);
                gamePanel.transform.GetChild(7).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

                gamePanel.transform.GetChild(7).GetChild(1).GetComponent<skillMenuControll>().pageChangeSkillID();
            }

            skillMenuTXT(gamePanel.transform.GetChild(7).GetChild(1).GetComponent<skillMenuControll>().currentSkill + (8 * (pageNumber - 1)));

            if (!IManager.skillMenu)
            {
                if (gamePanel.transform.GetChild(7).GetChild(1).GetComponent<skillMenuControll>().currentSkill == -1)
                {

                }
                else
                {
                    closeSKillMenu(gamePanel.transform.GetChild(7).GetChild(1).GetComponent<skillMenuControll>().currentSkill + (8 * (pageNumber - 1)));
                    break;
                }
            }
            yield return new WaitForSecondsRealtime(frametime);
        }
    }
    public void closeSKillMenu(int indexOfMenu)
    {

        state = GameState.inGame;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = baseFixedUpdate;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        for (int count = 0; count < gamePanel.transform.GetChild(7).GetChild(1).childCount; count++)
        {
            gamePanel.transform.GetChild(7).GetChild(1).GetChild(count).GetComponent<Button>().interactable = false;
            gamePanel.transform.GetChild(7).GetChild(1).GetChild(count).GetChild(0).gameObject.SetActive(false);
        }

        Color col = gamePanel.transform.GetChild(7).GetComponent<Image>().color;
        gamePanel.transform.GetChild(7).GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0.8f);

        gamePanel.transform.GetChild(7).gameObject.SetActive(false);
        int targetID = 0;
        if (indexOfMenu == -1)
        {
            targetID = -1;
        }
        else
        {
            targetID = player.GetComponent<WeaponManager>().stocked_Skills[indexOfMenu].skillTypeID;
        }
        //TODO: Change Skill functionally
        player.GetComponent<WeaponManager>().changeSkills(targetID);
    }
    public void skillMenuTXT(int index)
    {
        if (index == -1)
        {
            gamePanel.transform.GetChild(7).GetChild(2).Find("Desc").GetComponent<TextMeshProUGUI>().text = ";";
            gamePanel.transform.GetChild(7).GetChild(2).Find("Name").GetComponent<TextMeshProUGUI>().text = ";";
            return;
        }
        gamePanel.transform.GetChild(7).GetChild(2).Find("Desc").GetComponent<TextMeshProUGUI>().text = localizer_scp.applySkill(player.GetComponent<WeaponManager>().stocked_Skills[index],"Desc");
        gamePanel.transform.GetChild(7).GetChild(2).Find("Name").GetComponent<TextMeshProUGUI>().text = localizer_scp.applySkill(player.GetComponent<WeaponManager>().stocked_Skills[index], "Name");
    }
    //functions
    public void Interact(int interactID)
    {
        if (interactID == 0) 
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            state = GameState.inShop;
            ShopOpener();
        }
    }
    //function -> interactions closer function
    public void shopCloserfunc()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        state = GameState.inGame;
        ShopCloser();
    }
    public void DeductMainCurrency(float amount)
    {
        if (amount > money)
        {
            //TODO: add inofmation
            return;
        }
        //TODO: Buy sfx
        money -= amount;
        MoneyDisplay();
    }
    public void AddMainCurrency(float amount)
    {
        money += amount;
        MoneyDisplay();
    }

    public void weaponButtonPressed(string nameOFWeaponName)
    {
        int i;
        for (i = 0; i < weapons.Length; i++)
        {
            if(nameOFWeaponName == weapons[i].WeaponName + "Shop")
            {
                break;
            }
        }
        if(money < weapons[i].toBuyMoney)
        {
            ShopInfo(localizer_scp.applyMainMenu("ShopNoMoney"));
            return;
        }
        DeductMainCurrency(weapons[i].toBuyMoney);
        GetWeapon.perform_WOUTObjected(player, GetComponent<GameController>(), weapons[i].WeaponTypeID);
    }
    public void weaponButtonHover(string nameOFWeaponName)
    {
        int i;
        for (i = 0; i < weapons.Length; i++)
        {
            if (nameOFWeaponName == weapons[i].WeaponName + "Shop")
            {
                break;
            }
        }
        shopPanel.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = localizer_scp.applyWeapon(weapons[i], "Name");
    }
    public void shopNameReset()
    {
        shopPanel.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
    }
    public void skillButtonPressed(string nameOFSkillName)
    {
        Debug.Log(nameOFSkillName);
        int i;
        for (i = 0; i < skills.Length; i++)
        {
            if (nameOFSkillName == "id_" + skills[i].skillTypeID + "Shop")
            {
                Debug.Log(i);
                break;
            }
        }
        if (money < skills[i].toBuyMoney)
        {
            ShopInfo(localizer_scp.applyMainMenu("ShopNoMoney"));
            return;
        }
        DeductMainCurrency(skills[i].toBuyMoney);
        if (skills[i].st == Skill.skillType.active)
        {
            GetActiveSkill.perform_WOUTObjected(player, GetComponent<GameController>(), i);
        }
        else if (skills[i].st == Skill.skillType.passive)
        {
            PerformPassiveSkill.perform_WOUTObjected(player, GetComponent<GameController>(), i);
        }
    }
    public void skillButtonHover(string nameOFSkillName) 
    {
        int i;
        for (i = 0; i < skills.Length; i++)
        {
            if (nameOFSkillName == "id_" + skills[i].skillTypeID + "Shop")
            {
                break;
            }
        }
        shopPanel.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = localizer_scp.applySkill(skills[i], "Name");
    }
    public void decreseEnemyCount(GameObject deletedOBJ)
    {
        Destroy(cObjects.Find(x => x.irTransform == deletedOBJ.transform).uiTransform.gameObject);
        cObjects.Remove(cObjects.Find(x => x.irTransform == deletedOBJ.transform));
        enemyCount -= 1;
    }
    public void decreseCompassObject(GameObject deletedOBJ)
    {
        Destroy(cObjects.Find(x => x.irTransform == deletedOBJ.transform).uiTransform.gameObject);
        cObjects.Remove(cObjects.Find(x => x.irTransform == deletedOBJ.transform));
    }
    public void ComboVombo(int comboTime)
    {
        comboCount += comboTime;
        if (comboCount % 5 == 0 && comboCount != 0)
        {
            float[] possibleGainMoney = new float[1000];
            for(int i = 0; i < 1000; i++)
            {
                if (i < 250)
                {
                    possibleGainMoney[i] = 50f;
                }
                else if (i < 500)
                {
                    possibleGainMoney[i] = 70f;
                }
                else if (i < 750)
                {
                    possibleGainMoney[i] = 90f;
                }
                else if (i < 999)
                {
                    possibleGainMoney[i] = 500f;
                }
            }
            float gainMoney = possibleGainMoney[UnityEngine.Random.Range(0, possibleGainMoney.Length)]; 
            AddMainCurrency(gainMoney);
        }
        if (comboTime > 0)
        {
            if (comboDisplayRoutine != null)
            {
                StopCoroutine(comboDisplayRoutine);
            }
            comboDisplayRoutine = StartCoroutine(comboRoutine());
        }
        ComboVisualize(comboCount);
    }
    IEnumerator comboRoutine()
    {
        float duration = comboDuration;
        while (true)
        {
            duration -= Time.deltaTime;
            ComboBG(duration / comboDuration);

            yield return new WaitForEndOfFrame();
            if (duration <= 0)
            {
                break;
            }
        }
        ComboDelete();
    }

    public void ComboDelete()
    {
        comboCount = 0;
        ComboVisualize(comboCount);
    }

    public void DashEffectOpener(float duration)
    {
        if (dashEffectCoroutine != null)
        {
            StopCoroutine(dashEffectCoroutine);

            Camera overlayCam = mainCam.transform.GetChild(0).GetComponent<Camera>();

            mainCam.GetComponent<Camera>().fieldOfView = 60f;
            overlayCam.fieldOfView = 60f;
        }
        dashEffectCoroutine = StartCoroutine(DashEffect(duration));
    }


    //Effects
    IEnumerator DashEffect(float duration)
    {
        Camera overlayCam = mainCam.transform.GetChild(0).GetComponent<Camera>();

        mainCam.GetComponent<Camera>().fieldOfView = 55f;
        overlayCam.fieldOfView = 55f;
        yield return new WaitForSeconds(duration);
        while (true)
        {
            mainCam.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(mainCam.GetComponent<Camera>().fieldOfView, 60, Time.deltaTime * 30);
            overlayCam.fieldOfView = Mathf.MoveTowards(overlayCam.fieldOfView, 60, Time.deltaTime * 30);
            yield return new WaitForEndOfFrame();
            if(mainCam.GetComponent<Camera>().fieldOfView == 60 && overlayCam.fieldOfView == 60)
            {
                break;
            }
        }

        mainCam.GetComponent<Camera>().fieldOfView = 60f;
        overlayCam.fieldOfView = 60f;
    }



    IEnumerator endGameEffect()
    {
        UIOverlayCam.GetComponent<Camera>().GetUniversalAdditionalCameraData().volumeLayerMask = LayerMask.GetMask("Default");
        mainCam.GetComponent<Camera>().nearClipPlane = 0.3f;
        HandOverlayCam.GetComponent<Camera>().nearClipPlane = 0.3f;

        mainCam.GetComponent<Camera>().fieldOfView = 60f;
        float vectorizer = 1.5f;

        float remaining = 1.5f * Mathf.Pow(vectorizer, -1);

        while (true)
        {
            if (mainCam.transform.localEulerAngles.z < 10f)
            {
                if (mainCam.transform.localEulerAngles.z > 6f)
                {
                    mainCam.transform.localEulerAngles = Vector3.MoveTowards(mainCam.transform.localEulerAngles, new Vector3(mainCam.transform.localEulerAngles.x, 0, 10f), 0.02f * vectorizer);
                }
                else
                {
                    mainCam.transform.localEulerAngles = Vector3.MoveTowards(mainCam.transform.localEulerAngles, new Vector3(mainCam.transform.localEulerAngles.x, 0, 10f), 0.1f * vectorizer);
                }
            }
            if (mainCam.GetComponent<Camera>().fieldOfView >= 45f)
            {
                mainCam.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(mainCam.GetComponent<Camera>().fieldOfView, 45f, 0.1f * vectorizer);
            }
            if (remaining < 0f)
            {
                break;
            }
            yield return new WaitForSecondsRealtime(0.01f * vectorizer);
            remaining -= 0.01f * vectorizer;
        }


        while (true)
        {

            mainCam.GetComponent<Camera>().nearClipPlane += 0.5f;
            HandOverlayCam.GetComponent<Camera>().nearClipPlane += 0.5f;
            yield return new WaitForSecondsRealtime(0.01f);
            if (mainCam.GetComponent<Camera>().nearClipPlane >= 50f)
            {
                break;
            }
        }

        for (int i = 0 ; i < bossIntroPanel.transform.childCount; i++)
        {
            bossIntroPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < bossPanel.transform.childCount; i++)
        {
            bossPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < damagePanel.transform.childCount; i++)
        {
            damagePanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < gamePanel.transform.childCount; i++)
        {
            gamePanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < playerPanel.transform.childCount; i++)
        {
            playerPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < shopPanel.transform.childCount; i++)
        {
            shopPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        gamePanel.transform.GetChild(5).gameObject.SetActive(true);

        gamePanel.transform.GetChild(5).GetComponent<Image>().color = new Color(0, 0, 0, 0);

        while (true)
        {
            gamePanel.transform.GetChild(5).GetComponent<Image>().color = new Color(gamePanel.transform.GetChild(5).GetComponent<Image>().color.r, gamePanel.transform.GetChild(5).GetComponent<Image>().color.b, gamePanel.transform.GetChild(5).GetComponent<Image>().color.g, gamePanel.transform.GetChild(5).GetComponent<Image>().color.a + 0.01f * 0.01f);
            new WaitForSecondsRealtime(0.1f);
            if (gamePanel.transform.GetChild(5).GetComponent<Image>().color.a >= 1f)
            {
                break;
            }
        }

        gamePanel.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);

        gamePanel.transform.GetChild(5).GetChild(0).GetChild(0).gameObject.SetActive(true);
        gamePanel.transform.GetChild(5).GetChild(0).GetChild(1).gameObject.SetActive(false);

        string tempText = gamePanel.transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        gamePanel.transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";

        globalProfile.profile = noiseEffect;

        float a = 0f;
        for (int c = 0; c < tempText.Length;)
        {
            gamePanel.transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text += tempText[c];
            c += 1;
            if (tempText.Length - c < 100)
            {
                a += 0.005f;
                if (a >= 0.065f)
                {
                    gamePanel.transform.GetChild(5).GetChild(0).GetChild(1).gameObject.SetActive(! gamePanel.transform.GetChild(5).GetChild(0).GetChild(1).gameObject.activeSelf);
                }
            }
            yield return new WaitForSecondsRealtime(0.005f);
        }

        int changeCount = 0;

        gamePanel.transform.GetChild(5).GetChild(0).GetChild(0).gameObject.SetActive(false);

        while (true)
        {
            gamePanel.transform.GetChild(5).GetChild(0).gameObject.SetActive(!gamePanel.transform.GetChild(5).GetChild(0).gameObject.activeSelf);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.065f,0.085f));
            changeCount += 1;
            if (changeCount == 5)
            {
                break;
            }
        }
        gamePanel.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);

        for (int i = 1; i < gamePanel.transform.GetChild(5).childCount; i++)
        {
            gamePanel.transform.GetChild(5).GetChild(i).gameObject.SetActive(true);
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = baseFixedUpdate;
    }

    public void StartAgain()
    {
        PlayerPrefs.SetInt("newGame", 1);
        StartCoroutine(startAgainLoadingRoutine());
    }
    IEnumerator startAgainLoadingRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene_harbiMain");
        operation.allowSceneActivation = false;
        
        while (!operation.isDone)
        {
            if(operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

    }
}


public class CompassObject
{
    public Transform irTransform;
    public Transform uiTransform;
    public CompassObject(Transform ir , Transform ui)
    {
        irTransform = ir;
        uiTransform = ui;
    }
}
