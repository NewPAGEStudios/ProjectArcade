using Cinemachine;
using SlimUI.ModernMenu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour//TODO: Compass add cons
{
    public ScriptableRendererFeature seeThrougWallHigh;
    public ScriptableRendererFeature seeThrougWallBal;
    public ScriptableRendererFeature seeThrougWallPer;
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

    [Header("SoundRef")]
    public AudioSource cinematicBoomAS;
    public AudioSource endGame;
    public AudioSource endGameK;
    public AudioSource endGameCz;


    //UI Ref
    [Header(header: "UIReference")]
    public GameObject playerPanel;
    public GameObject gamePanel;
    public GameObject bossPanel;
    public GameObject shopPanel;
    public GameObject damagePanel;
    public GameObject settingPanel;

    [Header(header: "MapReference")]
    public GameObject startMap;
    public GameObject mainLevel;
    public GameObject mapParentM;
    public GameObject currentLevel;
    public GameObject tracesParent;
    //spawnPoint Variables
    public GameObject consumableSpawnPointParent;
    public GameObject enemySpawnPointParent;
    public GameObject playerTeleportPoint;
    [HideInInspector]
    public Shop[] shops;

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
    public StatisticManager statisticManager;
    [HideInInspector]
    public InputManager IManager;
    public AudioMixer audioM;
    public GameObject Minimap;
    public GameObject shopUI_Cam;


    public int waveNumber;

    private int comboCount;
    private int mostComboCount = 0;
    private float mon;
    public float money {  get { return mon; } set { mon = value; } }

    private bool newGame;

    [Header(header: "GameSettings")]
    public float waitTime;
    private float waitTimer;
    public float comboDuration;
    private float comboDurationTimer;

    public int EnemyCountInonetime;
    private int[] enemyIDcounts;
    private int enemyCountNow;
    private int enemyCount;
    public float enemySpawnTime;
    private float enemySpawnTimeTimer;

    public float fadeSpeedDMGVisualizetion;
    public float cameraShakeIntensity;
    public float cameraShakeDuration;
    private float baseFixedUpdate;
    public int bossTimePerWave;
    public int bossTimePerWaveLoop;
    public float wallTraceLifeTime;
    [Tooltip("PercentageOfSpawningWeapon")]
    public int ammoSpawn = 50;

    [Header(header: "UI Prefab Referances")]
    public GameObject shopButton;
    public GameObject shopTXT;
    public GameObject enemiesIndicator;
    public GameObject consSkillIndicator;
    public GameObject consAmmoIndicator;
    public List<Sprite> crossSprites = new();

    [Header(header: "UI Prefab Referances Notifications")]
    public GameObject notfsaved;
    public GameObject notfMoneyAdd;
    public GameObject notfSkll;
    public GameObject notfWAadd;
    public GameObject notfSkUsed;
    private float waitMultiplier;

    [Header(header: "Prefab Referances")]
    public GameObject consIndicator;
    public GameObject moneyOBJ_Parent;
    public GameObject moneyOBJ;
    public GameObject ConsMiniMapIndicator;
    public GameObject ConsParents;
    //IEnumerators
    private Coroutine comboDisplayRoutine;
    private Coroutine[] fWayDMGVisualize = new Coroutine[4];
    private Coroutine dmgGivenUICoroutine;
    private Coroutine dmgTakenEffectCoroutine;
    private Coroutine dashEffectCoroutine;
    private Coroutine shopInfoVis;
    private Coroutine moneyBoom;
    private Coroutine dmgDeal;
    private Coroutine comboVisualizeRoutine;
    private Coroutine notfWaitRoutine;
    private Coroutine subroutine;


    [Header("MapSpecifications")]
    [Header("Map1")]
    public RoomManager roomManager;
    public float roomCloseTime;
    public float roomRoutineRate;
    private float roomTimer;
    //PostProcessing Settings
    AmbientOcclusion ao;
    //SoundEffects

    //Tutorials
    public GameObject TutMan;
    [HideInInspector]
    public bool tutOpened;

    private void confirmSettings()
    {
        player.GetComponent<PController>().ChangeSens(PlayerPrefs.GetFloat("YSensitivity", 10f), PlayerPrefs.GetFloat("XSensitivity", 10));
        player.GetComponent<PController>().handleSNB(PlayerPrefs.GetInt("SwayNBobbing", 1) == 1 ? true : false);
        player.GetComponent<PController>().handleDV(PlayerPrefs.GetInt("DMGVibration", 1) == 1 ? true : false);

        Minimap.GetComponent<MiniMapManager>().ChangeSize(50 - PlayerPrefs.GetFloat("MinimapSize", 0f));
        Minimap.GetComponent<MiniMapManager>().smooth = PlayerPrefs.GetInt("MinimapSmoothness") == 1f ? true : false;

        playerPanel.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = crossSprites[PlayerPrefs.GetInt("crossID", 0)];

        changeSizeOfCross(PlayerPrefs.GetFloat("CrossSize", 10) * 10);
        changeSizeOfLaserIndication(PlayerPrefs.GetFloat("CrossSize", 10) * 15);
        Resolution resolution = Screen.currentResolution;

    }
    private void Awake()
    {
        if(TutMan != null)
        {
            tutOpened = true;
            //update base
            baseFixedUpdate = Time.fixedDeltaTime;

            ammos = Resources.LoadAll<Ammo>("Ammo");
            weapons = Resources.LoadAll<Weapon>("Weapon");
            enemies = Resources.LoadAll<EnemyType>("Enemy");
            consumables = Resources.LoadAll<Consumable>("Consumable");
            skills = Resources.LoadAll<Skill>("Skill");
            boss = Resources.LoadAll<Boss>("Boss");
            perks = Resources.LoadAll<Perk>("Perks");
            waveEnemyDatas = Resources.LoadAll<WaveEnemyData>("WaveEnemyData");

            playerPanel.SetActive(false);
            bossPanel.SetActive(false);
            damagePanel.SetActive(false);
            gamePanel.SetActive(false);


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
                but.targetGraphic = imaj.GetComponent<Image>();

                but.onClick.AddListener(() => weaponButtonPressed(but.gameObject.name));
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
                GameObject imaj = Instantiate(shopButton, shopPanel.transform.GetChild(0).GetChild(0).GetChild(1));
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
                evtrigger1.triggers.Add(hoveroutEvent);

                GameObject imajChild = Instantiate(shopTXT, imaj.transform);
            }

            ShopGridChange(0);
            //Runtime Infor holder Init
            player.GetComponent<WeaponManager>().holder = new WeaponRuntimeHolder[weapons.Length];

            for (int i = 0; i < player.GetComponent<WeaponManager>().holder.Length; i++)
            {
                player.GetComponent<WeaponManager>().holder[i] = new WeaponRuntimeHolder(weapons[i].WeaponTypeID, weapons[i].magSize);
            }

            return;
        }

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

        enemyIDcounts = new int[enemies.Length];

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
            but.targetGraphic = imaj.GetComponent<Image>();

            but.onClick.AddListener(() => weaponButtonPressed(but.gameObject.name));
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
            GameObject imaj = Instantiate(shopButton, shopPanel.transform.GetChild(0).GetChild(0).GetChild(1));
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
            evtrigger1.triggers.Add(hoveroutEvent);

            GameObject imajChild = Instantiate(shopTXT, imaj.transform);
        }
        ShopGridChange(0);
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

        statisticManager = GetComponent<StatisticManager>();

        newGame = PlayerPrefs.GetInt("newGame", 1) == 1 ? true : false;
    }

    private void Start()
    {
        changeStateOfWallhackSkill(false);

        confirmSettings();
        GetComponent<CheckMusicVolume>().UpdateVolume();
        if (tutOpened)
        {
            state = GameState.inGame;
            pState = PlayState.inPlayerInterrupt;

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
            ChangeAmmoText(-1, -1, false);

            ComboBG(0);
            ComboVisualize();


            //Money Handling
            money = 0;
            MoneyDisplay();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }
        //optionInitializition;
//        gamePanel.transform.GetChild(4).GetChild(1).GetComponent<InGameSettings>().initValues();

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
            ChangeAmmoText(-1, -1,false);

            ComboBG(0);
            ComboVisualize();


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
            ComboVisualize();

            Invoke(nameof(LoadElements), Time.deltaTime);
        }

        //Cursor Handling
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        roomTimer = roomRoutineRate;
        confirmSettings();
        /* Save on waitState
        check save and load it save elements =  
        player,char's ammo,weapon,ability,hp,dashMeter,Money,,
        gameController, pickable cons and weapons pos andID, waveNumber, 
         
         
         */
        closeSpriteOfActiveSkill(null);
    }

    //spawners
    public void SpawnCons(int pos_childID,int consID,int weaponID,int skillID,bool cameinBoss = false)
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
        consumableobject.transform.localScale = Vector3.one * 2;
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

        GameObject consMinimapInd = Instantiate(ConsMiniMapIndicator,consumableobject.transform);

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
            gw.minimapShocase = consMinimapInd;
            gw.consPosID = pos_Holder;
            if (!cameinBoss)
            {
                activeConsWeapID.Add(gw.weaponID);
                activeConsSkill.Add(-1);
            }
            gw.cameInBoss = cameinBoss;
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
            gas.minimapShocase = consMinimapInd;
            gas.consPosID = pos_Holder;
            if (!cameinBoss)
            {
                activeConsWeapID.Add(-1);
                activeConsSkill.Add(gas.skillId);
            }
            gas.cameInBoss = cameinBoss;
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
            pps.minimapShocase = consMinimapInd;
            pps.consPosID = pos_Holder;
            if (!cameinBoss)
            {
                activeConsWeapID.Add(-1);
                activeConsSkill.Add(pps.thisSkill.skillTypeID);
            }
            pps.cameInBoss = cameinBoss;
        }
        else
        {
            Debug.Log("Cons Spawn Cons type couldn't find");
        }


        consumableobject.AddComponent<SphereCollider>();
        consumableobject.GetComponent<SphereCollider>().isTrigger = true;

        GameObject arrow = Instantiate(consIndicator, consumableobject.transform);
        arrow.transform.localPosition = new Vector3(0, 1f, 0);
        arrow.transform.localEulerAngles = new Vector3(90, 0, 0);


        //consumableobject.AddComponent<MeshFilter>();
        //consumableobject.GetComponent<MeshFilter>().mesh = consumables[consID].modelMesh;

        //consumableobject.AddComponent<MeshRenderer>();
        //consumableobject.GetComponent<MeshRenderer>().materials = consumables[consID].mats;

        //add indicator for location guide


        consumableSpawnPointParent.transform.GetChild(r).parent = ConsParents.transform;
        return;
    }
    public void SpawnEnemy(int enemyID)
    {
        GameObject enemy = new();
        enemyCountNow += 1;
        enemyIDcounts[enemyID]--;
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
                toCinematicShop(GameState.inShop, GameState.inGame, null);
            }
            else if(state == GameState.inGame)
            {
                StopGame();
            }
        }
        if(state == GameState.inGame)
        {
            if (tutOpened)
            {
                return;
            }
            if (pState == PlayState.inStart)
            {
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
                //EnemySpawn
                if(enemySpawnTimeTimer <= 0f && enemyCountNow < EnemyCountInonetime && enemyCountNow < enemyCount)
                {
                    List<int> list = new List<int>();
                    for(int i = 0; i < enemies.Length; i++)
                    {
                        if (enemyIDcounts[i] > 0)
                        {
                            list.Add(i);//add avaible ids to list
                        }
                    }
                    SpawnEnemy(list[Random.Range(0, list.Count)]);
                    enemySpawnTimeTimer = enemySpawnTime;
                }
                else
                {
                    enemySpawnTimeTimer -= Time.deltaTime;
                }

                if (Input.GetKeyDown(KeyCode.H))
                {
                    roomManager.startRoutineOfRoom(roomCloseTime, 0);
                }
                //if (roomTimer <= 0f)
                //{
                //    roomManager.startRoutineOfRoom(roomCloseTime);
                //    roomTimer = roomRoutineRate;
                //}
                //else
                //{
                //    roomTimer -= Time.deltaTime;
                //}
                if (enemyCount == 0)
                {
                    makeMoneyWithMostCombo();
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
        toCinematicBossFight(pState, PlayState.inWaiting, bossObject.GetComponent<Animator>(), bossObject.GetComponentInChildren<Camera>(),bossObject);
    }
    private void toWave()
    {
        shopUI_Cam.SetActive(false);
        waveNumber += 1;
        statisticManager.increaseWaveNumber();
        if (waveEnemyDatas.Length < waveNumber)
        {
            EndBeta();
        }
        if (waveNumber % bossTimePerWave == bossTimePerWaveLoop)
        {
            toBoss(0);
            return;
        }
        else
        {
            pState = PlayState.inWave;
        }

        UpdateMostCombo(true);

        waitTimeVisualize(waitTimer);
        waveVisualzie("Wave " + waveNumber);


        for (int i = 0; i < currentLevel.GetComponent<Map>().shops.Length; i++)
        {
            currentLevel.GetComponent<Map>().shops[i].close();
        }

        //WaveConfigiration
        //EnemySpawn
        UpdateEnemyCount(true);
        foreach (WaveEnemyData wed in waveEnemyDatas)
        {
            if (wed.waveNumber == waveNumber)
            {
                for(int c = 0; c < wed.enemyTypes.Length; c++)//searching Enemy Typs
                {

                    for(int k = 0; k < wed.enemyTypes[c].enemyPiece; k++)//Indexing EnemyCounts
                    {
                        enemyIDcounts[wed.enemyTypes[c].enemyId]++;
                        enemyCount += 1;
//                        SpawnEnemy(wed.enemyTypes[c].enemyId);
                    }
                }
                //ConsSpawn
                remainingConsToSpawn = wed.maxConsSpawn;
                remainingAmmoConsToSpawn = wed.maxAmmoSpawn;
                break;
            }
        }
        UpdateEnemyCount(false);
    }
    public void callToWait()
    {
        toWait();
    }
    private void toWait()
    {
        SaveElements();
        shopUI_Cam.SetActive(true);
        UpdateMostCombo(true);
        statisticManager.updateMostCombo(mostComboCount);
        mostComboCount = 0;
        UpdateEnemyCount(true);

        waitTimer = waitTime;
        pState = PlayState.inWaiting;

        
        for (int i = 0; i < currentLevel.GetComponent<Map>().shops.Length; i++)
        {
            currentLevel.GetComponent<Map>().shops[i].open();
        }

        waitTimeVisualize(waitTimer);
        waveVisualzie("Wait");

        player.transform.GetChild(1).Find("Musics").Find("MusicWait").GetComponent<AudioSource>().Play();
    }

    private void toBoss(int bossID)
    {
        pState = PlayState.inBoss;
        int i;

        UpdateEnemyCount(true);
        UpdateMostCombo(true);

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

        //ManuelAdding
        if(go.TryGetComponent<DummyMummyFunc>(out DummyMummyFunc dmf))
        {
            dmf.bossState = DummyMummyFunc.BossState.interrupted;
            dmf.mapParent = map;
            dmf.bossStartMenu.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = boss[i].bossName;
        }

        
        bossPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = boss[i].bossName;


        toCinematicBossFight(pState, PlayState.inBoss, go.GetComponent<Animator>(), go.GetComponentInChildren<Camera>(),go);

    }
    private void toCinematicBossFight(PlayState from,PlayState to,Animator anim,Camera animCam,GameObject targetting)
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
            //ManuelAdding
            if (targetting.TryGetComponent<DummyMummyFunc>(out DummyMummyFunc dmf))
            {
                StartCoroutine(bossStartAnim(anim, animCam, dmf.bossStartMenu));
            }

        }
        else if(from == PlayState.inBoss && to == PlayState.inWaiting)
        {
            anim.SetTrigger("Finish");
            StartCoroutine(bossEndAnim(anim, animCam,targetting));
        }
        //Addable
    }
    private void toCinematicShop(GameState? from , GameState to, Shop sh)
    {
        pState=PlayState.inCinematic;

        playerPanel.SetActive(false);

        if(from == GameState.inGame && to == GameState.inShop)
        {
            StartCoroutine(ShopStartAnim(sh));
        }
        else if(from == GameState.inShop && to == GameState.inGame)
        {
            StartCoroutine(ShopEndAnim());
        }
    }


    //MapChanges
    public void changeMap(GameObject mapParent)
    {
        currentLevel = mapParent;
        currentLevel.SetActive(true);
        consumableSpawnPointParent = currentLevel.GetComponent<Map>().ConsSpawnPointParent;
        playerTeleportPoint = currentLevel.GetComponent<Map>().playerTeleportPos.gameObject;

        player.transform.position = playerTeleportPoint.transform.position;

        Minimap.GetComponent<MiniMapManager>().teleportMM();
    }
    public void DefaultMap()
    {
        currentLevel.SetActive(false);
        currentLevel = mainLevel;
        consumableSpawnPointParent = mainLevel.GetComponent<Map>().ConsSpawnPointParent;
        enemySpawnPointParent = mainLevel.GetComponent<Map>().enemySpawnPointParent;
        playerTeleportPoint = mainLevel.GetComponent<Map>().playerTeleportPos.gameObject;

        player.transform.position = playerTeleportPoint.transform.position;

        Minimap.GetComponent<MiniMapManager>().teleportMM();
    }

    //GameStatesChanger
    public void ResumeGame()
    {
        if (gamePanel.transform.GetChild(7).GetChild(0).gameObject.activeInHierarchy)
        {
            state = GameState.inGame;

            gamePanel.transform.GetChild(7).gameObject.SetActive(false);
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
            gamePanel.transform.GetChild(7).GetChild(0).gameObject.SetActive(true);
            gamePanel.transform.GetChild(7).GetChild(1).gameObject.SetActive(false);
        }
    }

    public void StopGame()
    {
        state = GameState.pause;
        gamePanel.transform.GetChild(7).gameObject.SetActive(true);
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
        statisticManager.saveDatas();
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
        notification(0,-1,null,null);
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
                player.GetComponent<WeaponManager>().stocked_Skills[count].count = de.stockedSkill_Count[count];
            }
        }
        toWait();
    }



    //Animations
    IEnumerator bossStartAnim(Animator selectedAnim,Camera selectedCam,GameObject panel)
    {
        for (int i = 0; i < gamePanel.transform.childCount; i++)
        {
            if (i == 2 || i == 3 || i == 4 || i == 5)
            {
                continue;
            }
            gamePanel.transform.GetChild(i).gameObject.SetActive(false);
        }
        while (true) 
        {
            yield return new WaitForEndOfFrame();
            if (selectedAnim.GetCurrentAnimatorStateInfo(0).IsName("end_start"))
            {
                break;
            }
        }
        cinematicBoomAS.Play();
        yield return new WaitForSeconds(.5f);
        panel.SetActive(true);
        yield return new WaitForSeconds(1f);
        //StopAnimation

        selectedAnim.SetBool("end", true);
        yield return new WaitForEndOfFrame();
        selectedAnim.SetBool("end", false);

        //UI Normalize
        panel.SetActive(false);
        playerPanel.SetActive(true);
        bossPanel.SetActive(true);

        //camNormalize
        mainCam.GetComponent<Camera>().enabled = true;
        selectedCam.enabled = false;

        //change GameState
        pState = PlayState.inBoss;
        if(selectedAnim.TryGetComponent<DummyMummyFunc>(out DummyMummyFunc dmf))
        {
            selectedAnim.GetComponent<DummyMummyFunc>().toIdle();
        }
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
            if (i == 3 || i == 7 || i == 8 || i == 9)
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

    IEnumerator ShopStartAnim(Shop sh)
    {
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().useGravity = false;

        Vector3 vec = sh.BodyPos.transform.position;
        while (true)
        {
            player.transform.forward = Vector3.MoveTowards(player.transform.forward, sh.BodyPos.transform.forward, 0.0001f * 300f);
            player.transform.position = Vector3.MoveTowards(player.transform.position, vec, 0.0001f * 100f);
            if (player.transform.position == vec)
            {
                player.transform.forward = sh.BodyPos.transform.forward;
                break;
            }
            yield return new WaitForSecondsRealtime(0.0001f);
        }
        vec = sh.HeadPOS.transform.position;
        while (true)
        {
            mainCam.transform.position = Vector3.MoveTowards(mainCam.transform.position, vec, 0.0001f * 150f);
            mainCam.transform.forward = Vector3.MoveTowards(mainCam.transform.forward, sh.HeadPOS.transform.forward, 0.0001f * 150f);
            if (mainCam.transform.position == vec)
            {       
                mainCam.transform.forward = sh.HeadPOS.transform.forward;
                break;
            }
            yield return new WaitForSecondsRealtime(0.0001f);
        }
        float xRot=10f;
        mainCam.transform.localEulerAngles = new Vector3(xRot, 0, 0);
        //startAnim;
        player.GetComponent<WeaponManager>().doubleHandAnimation(0);
        while (true)
        {
            if (player.GetComponent<WeaponManager>().hand_Animator.GetCurrentAnimatorStateInfo(2).IsName("WaitingEnd")) { break; }
            yield return new WaitForSecondsRealtime(0.0001f);
        }
        while (true)
        {
            mainCam.transform.localEulerAngles = Vector3.MoveTowards(mainCam.transform.localEulerAngles, new Vector3(0, 0, 0), 0.0001f * 500f);
            if (mainCam.transform.localEulerAngles.x == 0f)
            {
                break;
            }
            yield return new WaitForSecondsRealtime(0.0001f);
        }

        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        pState = PlayState.inWaiting;
        state = GameState.inShop;
        sh.workScreen();
        ShopOpener();
    }
    IEnumerator ShopEndAnim()
    {
        state = GameState.inGame;
        pState = PlayState.inWaiting;
        Vector3 vec= new Vector3(0, 1.5f, 0);
        foreach(Shop shop in currentLevel.GetComponent<Map>().shops)
        {
//            shop.unWorkScreen();
        }
        while (true)
        {
            mainCam.transform.localPosition = Vector3.MoveTowards(mainCam.transform.localPosition, vec, 0.0001f * 300f);
            if (mainCam.transform.localPosition == vec)
            {
                break;
            }
            yield return new WaitForSecondsRealtime(0.0001f);
        }
        yield return null;
        shopCloserfunc();
    }

    //
    public void changeSub(string stringID,float duration)
    {
        gamePanel.transform.GetChild(10).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = localizer_scp.applySubtitle(stringID);
        gamePanel.transform.GetChild(10).GetChild(0).gameObject.SetActive(true);
        if (subroutine != null)
        {
            StopCoroutine(subroutine);
        }
        subroutine = StartCoroutine(SubRoutine(duration));
    }
    IEnumerator SubRoutine(float timer)
    {
        while (true)
        {
            if (state != GameState.inGame)
            {
                continue;
            }
            if (timer <= 0)
            {
                break;
            }
            yield return null;
            timer -= Time.deltaTime;
        }
        gamePanel.transform.GetChild(10).GetChild(0).gameObject.SetActive(false);
    }

    //Graph Options
    public void changeStateOfWallhackSkill(bool acitve)
    {
        seeThrougWallHigh.SetActive(acitve);
        seeThrougWallBal.SetActive(acitve);
        seeThrougWallPer.SetActive(acitve);
    }

    //UI Events
    //GameBasedUI Evvents

    public void crossColorChange(Color col)
    {
        foreach(RawImage im in playerPanel.transform.GetChild(1).GetComponentsInChildren<RawImage>())
        {
            im.color = col;
        }
    }

    public void dmgDoneFeedBack()
    {
        if (dmgDeal != null)
        {
            StopCoroutine(dmgDeal);
        }
        dmgDeal = StartCoroutine(dmgDoneRoutine());
    }
    IEnumerator dmgDoneRoutine()
    {
        float a = 1f;
        Color col = playerPanel.transform.GetChild(2).GetChild(0).GetComponent<Image>().color;
        for (int c = 0; c < playerPanel.transform.GetChild(2).childCount; c++)
        {
            playerPanel.transform.GetChild(2).GetChild(c).GetComponent<Image>().color = new Color(col.r, col.g, col.b, 1);
        }
        while (true)
        {
            a = Mathf.MoveTowards(a, 0f, Time.deltaTime * 4);
            for(int c = 0; c < playerPanel.transform.GetChild(2).childCount; c++)
            {
                playerPanel.transform.GetChild(2).GetChild(c).GetComponent<Image>().color = new Color(col.r, col.g, col.b, a);
            }
            if (a == 0f)
            {
                break;
            }
            yield return null;
        }
    }


    public void UpdateEnemyCount(bool reset)
    {
        gamePanel.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        if (reset)
        {
            gamePanel.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        }
        gamePanel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = localizer_scp.applyGeneral("EnemyNumber_txt") + " " + enemyCount;
    }
    public void UpdateMostCombo(bool reset)
    {
        gamePanel.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
        if (reset)
        {
            gamePanel.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        }
        gamePanel.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = localizer_scp.applyGeneral("MostCombo_txt") + " " + mostComboCount;
    }

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
        if(pState == PlayState.inWave)
        {
            gamePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = localizer_scp.applyGeneral("Wave_txt") + " " + waveNumber;
        }
        else
        {
            gamePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = waveIndicator;
        }
    }
    public void mostComboVis()
    {
        gamePanel.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = localizer_scp.applyGeneral("MostCombo_txt") + " " + mostComboCount.ToString();
    }
    public void ComboVisualize()
    {
        if (comboCount <= 0)
        {
            gamePanel.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            gamePanel.transform.GetChild(2).gameObject.SetActive(true);
            gamePanel.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = comboCount + " Combo";
        }
    }
    IEnumerator ComboVisRoutine()
    {
        Transform txt = gamePanel.transform.GetChild(2).GetChild(0).GetChild(0);
        Transform txt1 = gamePanel.transform.GetChild(2).GetChild(0).GetChild(1);

        txt.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250, 0);
        txt1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250, -30);

        while (true)
        {
            txt.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(txt.GetComponent<RectTransform>().anchoredPosition, Vector2.zero, Time.deltaTime * 500f);
            txt1.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(txt.GetComponent<RectTransform>().anchoredPosition, new Vector2(0,-30), Time.deltaTime * 500f);
            if (txt.GetComponent<RectTransform>().anchoredPosition == Vector2.zero)
            {
                txt.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                txt1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30);
                break;
            }
            yield return null;
        }
        comboVisualizeRoutine = null;
    }
    public void ComboBG(float fa)
    {
        gamePanel.transform.GetChild(2).GetComponent<Image>().fillAmount = fa;
    }
    public void makeMoneyWithMostCombo()
    {
        AddMainCurrency(mostComboCount * 3);
        mostComboCount = 0;
    }
    public void ShopOpener()
    {
        ShopScreenChange(0);
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
        ShopScreenChange(1);
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
    public void ShopGridChange(int id)
    {
        for(int i = 0; i< shopPanel.transform.GetChild(0).GetChild(0).childCount; i++)
        {
            shopPanel.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
            if (id == i)
            {
                shopPanel.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
        }
    }
    public void ShopScreenChange(int childNum)
    {
        shopPanel.transform.GetChild(0).gameObject.SetActive(false);
        shopPanel.transform.GetChild(1).gameObject.SetActive(false);

        shopPanel.transform.GetChild(childNum).gameObject.SetActive(true);
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
    public void notification(int id, float addedMoney, Skill skill, Weapon weap)
    {
        switch (id)
        {
            case 0:
                GameObject goS = Instantiate(notfsaved, gamePanel.transform.GetChild(6));
                notfWaitRoutine = StartCoroutine(notf_AnimNonVectorized(goS));
                break;
            case 1:
                GameObject goM = Instantiate(notfMoneyAdd, gamePanel.transform.GetChild(6));
                goM.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = addedMoney.ToString() + " " + localizer_scp.applyGeneral("AddCurency_txt");
                notfWaitRoutine = StartCoroutine(notf_Anim(goM));
                break;
            case 2:
                GameObject goSk = Instantiate(notfSkll, gamePanel.transform.GetChild(6));
                goSk.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = localizer_scp.applySkill(skill, "Name") + " " + localizer_scp.applyGeneral("addSkill");
                goSk.transform.GetChild(1).GetComponent<Image>().sprite = skill.sprite_HUD;
                notfWaitRoutine = StartCoroutine(notf_Anim(goSk));
                break;
            case 3:
                GameObject goW = Instantiate(notfWAadd, gamePanel.transform.GetChild(6));
                goW.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = localizer_scp.applyWeapon(weap, "Name") + " " + localizer_scp.applyGeneral("WA_txt");
                notfWaitRoutine = StartCoroutine(notf_Anim(goW));
                break;
            case 4:
                GameObject goSkU = Instantiate(notfSkUsed, gamePanel.transform.GetChild(6));
                goSkU.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = localizer_scp.applyGeneral("usingSkill_txt") + " " + localizer_scp.applySkill(skill, "Name");
                goSkU.transform.GetChild(1).GetComponent<Image>().sprite = skill.sprite_HUD;
                notfWaitRoutine = StartCoroutine(notf_Anim(goSkU));
                break;
        }
    }
    IEnumerator notf_AnimNonVectorized(GameObject notfParent)
    {
        Color col = notfParent.transform.GetChild(1).GetComponent<Image>().color;
        float alph = col.a;
        while (true)
        {
            alph = Mathf.MoveTowards(alph, 0f, Time.deltaTime * 1.5f);
            col = new Color(col.r, col.g, col.b, alph);
            notfParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = col;
            notfParent.transform.GetChild(1).GetComponent<Image>().color = col;
            if (alph == 0) break;
            yield return null;
        }
        yield return null;
        Destroy(notfParent);
        notfWaitRoutine = null;
    }
    IEnumerator notf_Anim(GameObject notfParent)
    {
        Color col = notfParent.transform.GetChild(1).GetComponent<Image>().color;
        float alph = col.a;
        while (true)
        {
            alph = Mathf.MoveTowards(alph, 0f, Time.deltaTime * 1.5f);
            col = new Color(col.r,col.g,col.b,alph);
            notfParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = col;
            notfParent.transform.GetChild(1).GetComponent<Image>().color = col;
            notfParent.GetComponent<RectTransform>().anchoredPosition += 100f * Time.deltaTime * Vector2.up;
            if (alph == 0) break;
            yield return null;
        }
        yield return null;
        Destroy(notfParent);
        notfWaitRoutine = null;
    }


    //player based UI Events
    public void changeSizeOfCross(float size)
    {
        playerPanel.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(size,size);
    }
    public void changeSizeOfLaserIndication(float size)
    {
        playerPanel.transform.GetChild(6).GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
    }


    public void changeHPOfPlayer(float maxH, float currentH)
    {
        playerPanel.transform.GetChild(5).GetChild(0).GetComponent<Image>().fillAmount = currentH/maxH;
        playerPanel.transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = currentH.ToString() + "/" + maxH.ToString();
    }


    public void ChangeAmmoText(int? newAmmo = -1, int? newMaxAmmo = -1, bool? value = true)
    {
        playerPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newAmmo.ToString() + "/" + newMaxAmmo.ToString();

        playerPanel.transform.GetChild(0).transform.gameObject.SetActive(value.Value);
    }


    public void ChangeActiveWeapon(int id)
    {
        for (int c = 0; c < playerPanel.transform.GetChild(4).childCount; c++)
        {
            if(c == id)
            {
                playerPanel.transform.GetChild(4).GetChild(c).gameObject.SetActive(true);
            }
            else
            {
                playerPanel.transform.GetChild(4).GetChild(c).gameObject.SetActive(false);
            }
        }
    }




    public void changeSpriteOfActiveSkill(Sprite sprite)
    {
        playerPanel.transform.GetChild(3).GetChild(0).GetComponent<Image>().enabled = true;
        playerPanel.transform.GetChild(3).GetChild(0).GetComponent<Image>().sprite = sprite;
    }
    public void closeSpriteOfActiveSkill(Sprite sprite)
    {
        playerPanel.transform.GetChild(3).GetChild(0).GetComponent<Image>().sprite = null;
        playerPanel.transform.GetChild(3).GetChild(0).GetComponent<Image>().enabled = false;
    }
    public void LaserIndicator(float maxMeter,float curMeter)
    {
        for(int c = 0; c < playerPanel.transform.GetChild(4).childCount; c++)
        {
            if (playerPanel.transform.GetChild(4).GetChild(c).gameObject.activeInHierarchy)
            {
                playerPanel.transform.GetChild(4).GetChild(c).GetChild(0).GetComponent<Image>().fillAmount = (maxMeter - curMeter) / maxMeter;
            }
        }
    }
    public void laserIndicationCross(bool isOpen = true)
    {
        playerPanel.transform.GetChild(6).gameObject.SetActive(isOpen);
        if (isOpen)
        {
            playerPanel.transform.GetChild(6).localEulerAngles = new Vector3(0, 0, playerPanel.transform.GetChild(6).localEulerAngles.z + 10f);
        }
    }


    public void DashIndicator(float dashMeter)
    {
        float lastMeter = dashMeter % 25;
        int opennedMeters = (int) dashMeter / 25;
        for(int i = 0; i <= playerPanel.transform.GetChild(5).GetChild(1).childCount - 1; i++)
        {
            if (opennedMeters > 0)
            {
                playerPanel.transform.GetChild(5).GetChild(1).GetChild(i).GetComponent<Image>().fillAmount = 1;
            }
            else if(opennedMeters == 0)
            {
                playerPanel.transform.GetChild(5).GetChild(1).GetChild(i).GetComponent<Image>().fillAmount = lastMeter / 25;
            }
            else
            {
                playerPanel.transform.GetChild(5).GetChild(1).GetChild(i).GetComponent<Image>().fillAmount = 0;
            }
            opennedMeters--;
        }
    }
    //public void AddDashIndicator(float dashMeter)//Not in Actual Game
    //{
    //    float dashNumber = Mathf.Round(dashMeter / 25);
    //    float maxDashN = dashNumber;
    //    while(dashNumber > 0)
    //    {
    //        GameObject go = Instantiate(dashVisualized, playerPanel.transform.GetChild(5).GetChild(1));
    //        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-125 + (55.875f * (maxDashN - dashNumber)),0);
    //        dashNumber--;
    //    }
    //}
    public void BossHPChange(float fa)
    {
        bossPanel.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = fa;
    }
    public void MoneyDisplay()//TODO Make it gamePanel
    {
        gamePanel.transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = money.ToString() + " $";
        shopPanel.transform.GetChild(0).GetChild(7).GetComponent<TextMeshProUGUI>().text = money.ToString() + " $";
    }
    public void DisplayInstruction(bool display, int id_text)
    {

        gamePanel.transform.GetChild(3).gameObject.SetActive(display);
        switch (id_text)
        {
            case 0:
                gamePanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = localizer_scp.applyGeneral("Interraction_txt");
                break;
            case 1:
                gamePanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = localizer_scp.applyGeneral("cancelSkill_txt");
                break;
        }
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
        Image[] ima = new Image[playerPanel.transform.GetChild(2).childCount];
        for (int z = 0; z < playerPanel.transform.GetChild(2).childCount; z++)
        {
            ima[z] = playerPanel.transform.GetChild(2).GetChild(z).GetComponent<Image>();
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

        gamePanel.transform.GetChild(9).gameObject.SetActive(true);


        Color col = gamePanel.transform.GetChild(9).GetComponent<Image>().color;
        gamePanel.transform.GetChild(9).GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0);
        StartCoroutine(skillMenuOpenAnim(col));
        //TODO: place skills
        skillFullfillSkillMenu(0, 8);
    }
    public void skillFullfillSkillMenu(int startInt, int endInt)
    {
        for (int count=0; count < gamePanel.transform.GetChild(9).GetChild(1).childCount; count++)
        {
            gamePanel.transform.GetChild(9).GetChild(1).GetChild(count).GetComponent<Button>().interactable = false;
            gamePanel.transform.GetChild(9).GetChild(1).GetChild(count).GetChild(0).gameObject.SetActive(false);
        }
        for (int c = startInt; c < endInt; c++)
        {
            if(c == player.GetComponent<WeaponManager>().stocked_Skills.Count)
            {
                break;
            }
            gamePanel.transform.GetChild(9).GetChild(1).GetChild(c - startInt).GetComponent<Button>().interactable = true;
            gamePanel.transform.GetChild(9).GetChild(1).GetChild(c - startInt).GetChild(0).gameObject.SetActive(true);

            gamePanel.transform.GetChild(9).GetChild(1).GetChild(c - startInt).GetChild(0).GetComponent<Image>().sprite = player.GetComponent<WeaponManager>().stocked_Skills[c].skill.sprite_HUD;
            if (player.GetComponent<WeaponManager>().stocked_Skills[c].count <= 0)
            {
                gamePanel.transform.GetChild(9).GetChild(1).GetChild(c - startInt).GetComponent<Button>().interactable = false;
            }
            gamePanel.transform.GetChild(9).GetChild(1).GetChild(c - startInt).GetChild(1).GetComponent<TextMeshProUGUI>().text = player.GetComponent<WeaponManager>().stocked_Skills[c].count.ToString();
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
        gamePanel.transform.GetChild(9).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

        int pageCount = (player.GetComponent<WeaponManager>().stocked_Skills.Count/ 8) + 1;

        while (true)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 0f, frametime * 10);
            Time.fixedDeltaTime *= Time.timeScale;
            gamePanel.transform.GetChild(9).GetComponent<Image>().color = new Color(bgColor.r, bgColor.g, bgColor.b, Mathf.MoveTowards(gamePanel.transform.GetChild(9).GetComponent<Image>().color.a, bgColor.a, frametime * 20));



            if(IManager.getMouseScroll() > 0)
            {
                pageNumber++;
                if(pageNumber >= pageCount + 1)
                {
                    pageNumber = 1;
                }
                changePageSkillMenu(pageNumber);
                gamePanel.transform.GetChild(9).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

                gamePanel.transform.GetChild(9).GetChild(1).GetComponent<skillMenuControll>().pageChangeSkillID();
            }
            else if (IManager.getMouseScroll() < 0)
            {
                pageNumber--;
                if (pageNumber <= 0)
                {
                    pageNumber = pageCount;
                }
                changePageSkillMenu(pageNumber);
                
                gamePanel.transform.GetChild(9).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

                gamePanel.transform.GetChild(9).GetChild(1).GetComponent<skillMenuControll>().pageChangeSkillID();
            }

            if (!IManager.skillMenu)
            {
                closeSKillMenu(gamePanel.transform.GetChild(9).GetChild(1).GetComponent<skillMenuControll>().currentSkill + (8 * (pageNumber - 1)));
                break;
            }

            skillMenuTXT(gamePanel.transform.GetChild(9).GetChild(1).GetComponent<skillMenuControll>().currentSkill + (8 * (pageNumber - 1)));

            if (gamePanel.transform.GetChild(9).GetComponent<Image>().color.a == bgColor.a)
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
                gamePanel.transform.GetChild(9).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

                gamePanel.transform.GetChild(9).GetChild(1).GetComponent<skillMenuControll>().pageChangeSkillID();
            }
            else if (IManager.getMouseScroll() < 0)
            {
                pageNumber--;
                if (pageNumber <= 0)
                {
                    pageNumber = pageCount;
                }
                changePageSkillMenu(pageNumber);
                gamePanel.transform.GetChild(9).GetChild(3).GetComponent<TextMeshProUGUI>().text = pageNumber.ToString();

                gamePanel.transform.GetChild(9).GetChild(1).GetComponent<skillMenuControll>().pageChangeSkillID();
            }

            skillMenuTXT(gamePanel.transform.GetChild(9).GetChild(1).GetComponent<skillMenuControll>().currentSkill + (8 * (pageNumber - 1)));

            if (!IManager.skillMenu)
            {
                if (gamePanel.transform.GetChild(9).GetChild(1).GetComponent<skillMenuControll>().currentSkill == -1)
                {
                    closeSKillMenu(-1);
                    break;
                }
                else
                {
                    closeSKillMenu(gamePanel.transform.GetChild(9).GetChild(1).GetComponent<skillMenuControll>().currentSkill + (8 * (pageNumber - 1)));
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

        for (int count = 0; count < gamePanel.transform.GetChild(9).GetChild(1).childCount; count++)
        {
            gamePanel.transform.GetChild(9).GetChild(1).GetChild(count).GetComponent<Button>().interactable = false;
            gamePanel.transform.GetChild(9).GetChild(1).GetChild(count).GetChild(0).gameObject.SetActive(false);
        }

        Color col = gamePanel.transform.GetChild(9).GetComponent<Image>().color;
        gamePanel.transform.GetChild(9).GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0.8f);

        gamePanel.transform.GetChild(9).gameObject.SetActive(false);

        int targetID = 0;
        if (indexOfMenu == -1)
        {
            targetID = -1;
        }
        else
        {
            targetID = player.GetComponent<WeaponManager>().stocked_Skills[indexOfMenu].skill.skillTypeID;
        }
        //TODO: Change Skill functionally
        player.GetComponent<WeaponManager>().changeSkills(targetID);
    }
    public void skillMenuTXT(int index)
    {
        if (index == -1)
        {
            gamePanel.transform.GetChild(9).GetChild(2).Find("Desc").GetComponent<TextMeshProUGUI>().text = ";";
            gamePanel.transform.GetChild(9).GetChild(2).Find("Name").GetComponent<TextMeshProUGUI>().text = ";";
            return;
        }
        gamePanel.transform.GetChild(9).GetChild(2).Find("Desc").GetComponent<TextMeshProUGUI>().text = localizer_scp.applySkill(player.GetComponent<WeaponManager>().stocked_Skills[index].skill,"Desc");
        gamePanel.transform.GetChild(9).GetChild(2).Find("Name").GetComponent<TextMeshProUGUI>().text = localizer_scp.applySkill(player.GetComponent<WeaponManager>().stocked_Skills[index].skill, "Name");
    }
    //functions
    public void Interact(int interactID,GameObject interactedOBJ)
    {
        if (interactID == 0) 
        {
            if(interactedOBJ.TryGetComponent<Shop>(out Shop sh))
            {
                toCinematicShop(GameState.inGame, GameState.inShop, sh);
            }
        }
    }
    //function -> interactions closer function
    public void shopCloserfunc()
    {
        Time.timeScale = 1f;

        player.GetComponent<WeaponManager>().doubleHandAnimationStop();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ShopCloser();
    }
    public void DeductMainCurrency(float amount)
    {
        if (amount > money)
        {
            //TODO: add inofmation
            player.transform.GetChild(1).Find("SoundSFX").GetChild(0).Find("SFX_NoMoney").GetComponent<AudioSource>().Play();
            return;
        }
        money -= amount;
        player.transform.GetChild(1).Find("SoundSFX").GetChild(0).Find("SFX_Cash").GetComponent<AudioSource>().Play();

        if (moneyBoom != null)
        {
            StopCoroutine(moneyBoom);
        }
        MoneyDisplay();
    }
    public void AddMainCurrency(float amount)
    {
        money += amount;
        player.transform.GetChild(1).Find("SoundSFX").GetChild(0).Find("SFX_Cash").GetComponent<AudioSource>().Play();

        if (moneyBoom != null)
        {
            StopCoroutine(moneyBoom);
        }
        notification(1, amount, null, null);
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
        shopPanel.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = weapons[i].toBuyMoney.ToString();
        shopPanel.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = "";
        InventorInspectorer(0, i);
    }
    public void shopNameReset()
    {
        shopPanel.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
        shopPanel.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
        shopPanel.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = "";
        shopPanel.transform.GetChild(0).GetChild(9).GetComponent<TextMeshProUGUI>().text = "";
    }
    public void skillButtonPressed(string nameOFSkillName)
    {
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
        shopPanel.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = skills[i].toBuyMoney.ToString();
        shopPanel.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = localizer_scp.applySkill(skills[i], "Desc");
        InventorInspectorer(1,i);
    }
    public void InventorInspectorer(int id,int idOfObject)
    {//9
        StringVariable stringVar = new StringVariable();
        shopPanel.transform.GetChild(0).GetChild(9).GetChild(0).gameObject.SetActive(false);
        shopPanel.transform.GetChild(0).GetChild(9).GetChild(1).gameObject.SetActive(false);
        if (id == 0)//weapon
        {
            shopPanel.transform.GetChild(0).GetChild(9).GetChild(0).gameObject.SetActive(true);

            int count = 0;
            foreach(WeaponRuntimeHolder wrholder in player.GetComponent<WeaponManager>().holder)
            {
                if(wrholder.weaponTypeID == idOfObject)
                {
                    break;
                }
                count++;
            }
            LocalizedString localizedString = shopPanel.transform.GetChild(0).GetChild(9).GetChild(0).GetComponent<LocalizeStringEvent>().StringReference;
            stringVar.Value = (player.GetComponent<WeaponManager>().holder[count].inWeapon_ammoAmount + player.GetComponent<WeaponManager>().holder[count].sum_ammoAmount).ToString();
            localizedString.Add("weaponCount", stringVar);
        }
        else//skill
        {
            shopPanel.transform.GetChild(0).GetChild(9).GetChild(1).gameObject.SetActive(true);
            int count = 0;
            foreach (SkillRuntimeHolder ssholder in player.GetComponent<WeaponManager>().stocked_Skills)
            {
                if (ssholder.skill.skillTypeID == idOfObject)
                {
                    break;
                }
                count++;
            }
            shopPanel.transform.GetChild(0).GetChild(9).GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.Arguments[0] = player.GetComponent<WeaponManager>().stocked_Skills[count].count.ToString();
        }
    }
    public void decreseEnemyCount(GameObject deletedOBJ)
    {
        enemyCount -= 1;
        enemyCountNow -= 1;
        UpdateEnemyCount(false);
    }
    public void ComboVombo(int comboTime)
    {
        comboCount += comboTime;
        statisticManager.incSumOfCombo(comboTime);
        if (mostComboCount < comboCount)
        {
            mostComboCount = comboCount;
        }
        if (comboTime > 0)
        {
            if (comboDisplayRoutine == null)
            {
                comboTimerReset();
                if (comboVisualizeRoutine != null)
                {
                    StopCoroutine(comboVisualizeRoutine);
                }
                comboVisualizeRoutine = StartCoroutine(ComboVisRoutine());
            }
            else
            {
                StopCoroutine(comboDisplayRoutine);
            }
            comboDisplayRoutine = StartCoroutine(comboRoutine());
        }
        UpdateMostCombo(false);
        mostComboVis();
        ComboVisualize();
    }
    IEnumerator comboRoutine()
    {
        while (true)
        {
            comboDurationTimer -= Time.deltaTime;
            ComboBG(comboDurationTimer / comboDuration);

            yield return new WaitForEndOfFrame();
            if (comboDurationTimer <= 0)
            {
                break;
            }
        }
        comboDisplayRoutine = null;
        ComboDelete();
    }
    public void comboTimerReset()
    {
        comboDurationTimer = comboDuration;
    }

    public void ComboDelete()
    {
        comboCount = 0;
        ComboVisualize();
    }

    public void DashEffectOpener(float duration)
    {
    }


    //Effects



    IEnumerator endGameEffect()
    {
        UIOverlayCam.GetComponent<Camera>().GetUniversalAdditionalCameraData().volumeLayerMask = LayerMask.GetMask("Default");
        mainCam.GetComponent<Camera>().nearClipPlane = 0.3f;
        HandOverlayCam.GetComponent<Camera>().nearClipPlane = 0.3f;

        endGame.Play();

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

        gamePanel.transform.GetChild(8).gameObject.SetActive(true);

        gamePanel.transform.GetChild(8).GetComponent<Image>().color = new Color(0, 0, 0, 0);

        while (true)
        {
            gamePanel.transform.GetChild(8).GetComponent<Image>().color = new Color(gamePanel.transform.GetChild(8).GetComponent<Image>().color.r, gamePanel.transform.GetChild(8).GetComponent<Image>().color.b, gamePanel.transform.GetChild(8).GetComponent<Image>().color.g, gamePanel.transform.GetChild(8).GetComponent<Image>().color.a + 0.01f * 0.01f);
            new WaitForSecondsRealtime(0.1f);
            if (gamePanel.transform.GetChild(8).GetComponent<Image>().color.a >= 1f)
            {
                break;
            }
        }

        gamePanel.transform.GetChild(8).GetChild(0).gameObject.SetActive(true);

        gamePanel.transform.GetChild(8).GetChild(0).GetChild(0).gameObject.SetActive(true);
        gamePanel.transform.GetChild(8).GetChild(0).GetChild(1).gameObject.SetActive(false);

        string tempText = gamePanel.transform.GetChild(8).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        gamePanel.transform.GetChild(8).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";

        globalProfile.profile = noiseEffect;

        endGameK.Play();

        float a = 0f;
        bool onetime = true;

        for (int c = 0; c < tempText.Length;)
        {
            gamePanel.transform.GetChild(8).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text += tempText[c];
            c += 1;
            if (tempText.Length - c < 100)
            {
                if (onetime)
                {
                    onetime = false;
                    endGameCz.Play();
                }
                a += 0.005f;
                if (a >= 0.065f)
                {
                    gamePanel.transform.GetChild(8).GetChild(0).GetChild(1).gameObject.SetActive(! gamePanel.transform.GetChild(8).GetChild(0).GetChild(1).gameObject.activeSelf);
                }
            }
            yield return new WaitForSecondsRealtime(0.005f);
        }

        int changeCount = 0;

        gamePanel.transform.GetChild(8).GetChild(0).GetChild(0).gameObject.SetActive(false);


        endGameCz.Play();

        while (true)
        {
            gamePanel.transform.GetChild(8).GetChild(0).gameObject.SetActive(!gamePanel.transform.GetChild(8).GetChild(0).gameObject.activeSelf);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.065f,0.085f));
            changeCount += 1;
            if (changeCount == 5)
            {
                break;
            }
        }
        gamePanel.transform.GetChild(8).GetChild(0).gameObject.SetActive(false);

        for (int i = 1; i < gamePanel.transform.GetChild(8).childCount; i++)
        {
            gamePanel.transform.GetChild(8).GetChild(i).gameObject.SetActive(true);
        }

//        Time.timeScale = 1f;
//        Time.fixedDeltaTime = baseFixedUpdate;
    }

    public void StartAgain()
    {
        statisticManager.saveDatas();

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        PlayerPrefs.SetInt("newGame", 1);
        StartCoroutine(startAgainLoadingRoutine());
    }
    IEnumerator startAgainLoadingRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
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

    #region bestShoot savingBestShoot

    public void saveShoot()
    {

    }









    #endregion
    private void EndBeta()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        PlayerPrefs.SetInt("newGame", 1);
        PlayerPrefs.SetInt("EndBeta", 1);
        SceneManager.LoadScene(0);
    }
    //option
    public void DestroyOnTime(GameObject g,float t)
    {
        Destroy(g, t);
    }
}


