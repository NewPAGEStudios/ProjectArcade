using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
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
        inStart,
        inWave,
        inWaiting,
        inBoss,
        inShop,
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
    
    private List<Skill> activeSkills = new List<Skill>();
    private List<Skill> passiveSkills = new List<Skill>();
    private List<Skill> instantSkills = new List<Skill>();
    
    //to visualise skills for indicatng parent obj
    public GameObject skillIndicatorParent;
    public GameObject skillObject;
    public Material skillIndicatorMaterial;
    public GameObject dashVisualized;

    [Header(header: "Main Camera")]
    public GameObject mainCam;
    //UI Ref
    [Header(header: "UIReference")]
    public GameObject playerPanel;
    public GameObject gamePanel;
    public GameObject bossIntroPanel;
    public GameObject bossPanel;
    public GameObject shopPanel;

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

    private float spawnTimer;

    [Header(header: "PlayerConfigiration")]
    public GameObject inActiveWeapon;
    public GameObject player;


    private int waveNumber;
    private int enemyCount;

    private int comboCount;
    private float money;


    [Header(header: "GameSettings")]
    public float waitTime;
    private float waitTimer;
    public float comboDuration;
    [Header(header: "UI Prefab Referances")]
    public GameObject shopButton;
    public GameObject shopTXT;
    //IEnumerators
    private Coroutine comboDisplayRoutine;
    private void Awake()
    {
        ammos = Resources.LoadAll<Ammo>("Ammo");
        weapons = Resources.LoadAll<Weapon>("Weapon");
        enemies = Resources.LoadAll<EnemyType>("Enemy");
        consumables = Resources.LoadAll<Consumable>("Consumable");
        skills = Resources.LoadAll<Skill>("Skill");
        boss = Resources.LoadAll<Boss>("Boss");
        //leftHandTargetPosInýt



        for(int i = 0; i < weapons.Length; i++)
        {
            //weapon GO ýnstantiate
            GameObject weaponGO = Instantiate(weapons[i].modelGameObject, inActiveWeapon.transform);
            weaponGO.name = weapons[i].WeaponName;
            weaponGO.SetActive(false);

            //UI Instanttiate
            GameObject imaj = Instantiate(shopButton, shopPanel.transform.GetChild(0).GetChild(0));
            imaj.AddComponent<Image>();
            imaj.GetComponent<Image>().sprite = weapons[i].UIRef;
            imaj.name = weapons[i].WeaponName + "Shop";
            Button but = imaj.AddComponent<Button>();
            but.targetGraphic=imaj.GetComponent<Image>();

            but.onClick.AddListener(() => weaponButtonPressed(but.gameObject.name) );

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
            else if (skills[i].st == Skill.skillType.instant)
            {
                instantSkills.Add(skills[i]);

            }
            else if (skills[i].st == Skill.skillType.passive)
            {
                passiveSkills.Add(skills[i]);

            }
        }
        //spawnMaps;
        for(int i = 0; i < boss.Length; i++)
        {
            GameObject go = Instantiate(boss[i].mapParent,mapParentM.transform);
            go.name = boss[i].mapName;
            go.SetActive(false);
        }


    }

    private void Start()
    {
        state = GameState.inGame;
        pState = PlayState.inStart;

        //startLevel
        currentLevel = startMap;
        changeMap(currentLevel);

        player.transform.position = playerTeleportPoint.transform.position;
        player.GetComponent<PController>().HealDMG(1, gameObject);

        SpawnCons(0);
        waveVisualzie("Get the weapon");
        waveNumber = 0;

        waitTimeVisualize(-1);



        //UI INIT
        ChangeAmmoText(0);
        ChangefullAmmoText(0);

        ComboBG(0);
        ComboVisualize(0);

        //Cursor Handling
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //Money Handling
        money = 400;
        MoneyDisplay();


        /* Save on waitState
        check save and load it save elements =  
        player,char's ammo,weapon,ability,hp,dashMeter,Money,,
        gameController, pickable cons and weapons pos andID, waveNumber, 
         
         
         */
    }

    //    spawners
    private void SpawnCons(int consID)
    {

        int r = UnityEngine.Random.Range(0, consumableSpawnPointParent.transform.childCount);

        Vector3 vec = consumableSpawnPointParent.transform.GetChild(r).position;

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

        consumableobject.AddComponent(consumables[i].function.GetClass());
        
        
        
        //Manuel adding
        if (consumableobject.TryGetComponent<GetWeapon>(out GetWeapon gw))
        {
            gw.weaponID = UnityEngine.Random.Range(0, weapons.Length);
        }
        //skills
        else if(consumableobject.TryGetComponent<GetActiveSkill>(out GetActiveSkill gas))
        {
            gas.skillId = activeSkills[UnityEngine.Random.Range(0, activeSkills.Count)].skillTypeID;
        }
        else if(consumableobject.TryGetComponent<PerformInstantSkill>(out PerformInstantSkill pis))
        {
            pis.thisSkill = instantSkills[UnityEngine.Random.Range(0, instantSkills.Count)];
        }
        else if(consumableobject.TryGetComponent<PerformPassiveSkill>(out PerformPassiveSkill pps))
        {
            pps.thisSkill = passiveSkills[UnityEngine.Random.Range(0, passiveSkills.Count)];
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
        float y = p.position.y + enemies[indexOfID].modelGameObject.transform.localScale.y;
        float z = UnityEngine.Random.Range(p.transform.Find("min").position.z, p.transform.Find("max").position.z);

        enemy.transform.position = new Vector3(x, y, z);

        enemy.AddComponent<Enemy>();
        enemy.GetComponent<Enemy>().e_type = enemies[indexOfID];
        enemy.GetComponent<Enemy>().parentSelectedPosition = p;

        if (enemies[indexOfID].isRanged)
        {
            GameObject firePosGO = new();
            firePosGO.transform.SetParent(enemy.transform);

            firePosGO.transform.position = enemies[indexOfID].firePos;
            firePosGO.transform.localEulerAngles = Vector3.zero;
            firePosGO.transform.localScale = Vector3.one;

        }

        enemy.name = "enemy";

    }



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
            if (pState == PlayState.inStart)
            {
                return;
            }
            else if (pState == PlayState.inWave)
            {
                if (spawnTimer <= 0f)
                {
                    SpawnCons(UnityEngine.Random.Range(0, consumables.Length));
                    spawnTimer = UnityEngine.Random.Range(7.5f, 12.5f);
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
    public void endBoss()
    {
        bossPanel.SetActive(false);
        changeMap(mainLevel);
        toWait();
    }
    private void toWave()
    {
        waveNumber += 1;
        if (waveNumber % 10 == 2)
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
        //difficulty
        int maxDifficultyNumber = (waveNumber * 4) - 2;
        List<EnemyType> enemyThatCanSpawn = new List<EnemyType>();
        
        for(int a = 0; a < enemies.Length; a++)
        {
            if (enemies[a].difficultyNumber <= maxDifficultyNumber)
            {
                enemyThatCanSpawn.Add(enemies[a]);
            }
        }

        while(maxDifficultyNumber > 0)
        {
            int i = UnityEngine.Random.Range(0, enemyThatCanSpawn.Count);
            SpawnEnemy(enemyThatCanSpawn[i].EnemyTypeID);
            maxDifficultyNumber -= enemyThatCanSpawn[i].difficultyNumber;
            Debug.Log(maxDifficultyNumber);
        }
    }

    private void toWait()
    {
        //Save Game uzantýyý .NP dÝYE KAYDET
        waitTimer = waitTime;
        pState = PlayState.inWaiting;
        waitTimeVisualize(waitTimer);
        waveVisualzie("Wait");
        //Enable Shops
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

        go.AddComponent(boss[i].bossController.GetClass());

        //ManuelAdding
        if(go.TryGetComponent<DummyMummyFunc>(out DummyMummyFunc dmf))
        {
            dmf.boss = boss[i];
        }
        bossIntroPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = boss[i].bossName;
        bossPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = boss[i].bossName;


        toCinematic(pState, PlayState.inBoss, go.GetComponent<Animator>(), go.GetComponentInChildren<Camera>());

    }
    private void toCinematic(PlayState from,PlayState to,Animator anim,Camera animCam)
    {
        pState = PlayState.inCinematic;
        //DisableUI
        playerPanel.SetActive(false);
        gamePanel.SetActive(false);


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
            //bossEnd
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
        state = GameState.inGame;
    }

    public void StopGame()
    {
        state = GameState.pause;
    }


    //SaveSystem
    public void SaveElements()
    {

    }



    //Animations
    IEnumerator waveStartAnim()//UI
    {
        TextMeshProUGUI tmp = gamePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        tmp.rectTransform.localPosition = Vector3.zero;
        tmp.fontSize = 144;
        while (true)
        {
            tmp.rectTransform.localPosition = new Vector3(0, tmp.rectTransform.localPosition.y + 10f, 0);
            tmp.fontSize -= 2.16f;
            yield return new WaitForEndOfFrame();
            if (tmp.fontSize <= 36)
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
            if (selectedAnim.GetCurrentAnimatorStateInfo(0).IsName("end"))
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
        gamePanel.SetActive(true);
        playerPanel.SetActive(true);
        bossPanel.SetActive(true);
        //camNormalize
        mainCam.GetComponent<Camera>().enabled = true;
        selectedCam.enabled = false;
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
        playerPanel.SetActive(false);
        gamePanel.SetActive(false);
        shopPanel.SetActive(true);

    }



    //player based UI Events

    public void changeHPOfPlayer(float maxH, float currentH)
    {
        playerPanel.transform.GetChild(6).GetChild(0).GetComponent<Image>().fillAmount = currentH/maxH;
        playerPanel.transform.GetChild(6).GetChild(1).GetComponent<TextMeshProUGUI>().text = currentH.ToString() + "/" + maxH.ToString();
    }
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
    public void BossHPChange(float fa)
    {
        bossPanel.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = fa;
    }
    public void MoneyDisplay()
    {
        playerPanel.transform.GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = money.ToString() + " $";
    }
    public void MainMenu()
    {
        //SceneManagement.LoadScene(mainMenuint)
    }
    public void DisplayInstruction(bool display)
    {
        gamePanel.transform.GetChild(3).gameObject.SetActive(display);
    }


    //functions
    public void Interact(int interactID)
    {
        if (interactID == 0) 
        {
            Time.timeScale = 0;
            pState = PlayState.inShop;
            shopPanel.SetActive(true);
        }
    }
    public void DeductMainCurrency(float amount)
    {
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
        DeductMainCurrency(weapons[i].toBuyMoney);
        GetWeapon.perform_WOUTObjected(player, GetComponent<GameController>(), weapons[i].WeaponTypeID);
    }

    public void decreseEnemyCount()
    {
        enemyCount -= 1;
    }
    public void ComboVombo(int comboTime)
    {
        comboCount += comboTime;
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
        Debug.Log("kdmtss");
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
}
