using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Game Main Init
    private enum GameState
    {
        pause,
        inGame
    }
    private GameState state;
    public Ammo[] ammos;
    public Weapon[] weapons;
    public Enemy[] enemies;
    public Consumable[] consumables;
    //UI Ref
    public GameObject playerPanel;
    //Map parent Ref
    public GameObject levelDesign;
    //spawnPoint Variables
    public GameObject spawnPointParent;
    private float spawnTimer;


    public GameObject inActiveWeapon;

    private void Awake()
    {
        ammos = Resources.LoadAll<Ammo>("Ammo");
        weapons = Resources.LoadAll<Weapon>("Weapon");
        enemies = Resources.LoadAll<Enemy>("Enemy");
        consumables = Resources.LoadAll<Consumable>("Consumable");

        //leftHandTargetPosIn�t



        for(int i = 0; i < weapons.Length; i++)
        {
            GameObject weaponGO = Instantiate(weapons[i].modelGameObject, inActiveWeapon.transform);
            weaponGO.name = weapons[i].WeaponName;
            weaponGO.SetActive(false);
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
        SpawnCons(0);
    }

    //    spawners
    private void SpawnCons(int consID)
    {
        int r = Random.Range(0, spawnPointParent.transform.childCount);

        Vector3 vec = spawnPointParent.transform.GetChild(r).position;

        Vector3 posOFC = new(vec.x, vec.y + 1f, vec.z);

        GameObject consumableobject = new()
        {
            name = consumables[consID].nameOfC
        };

        consumableobject.transform.parent = spawnPointParent.transform.GetChild(r);

        consumableobject.transform.position = posOFC;

        consumableobject.AddComponent(consumables[consID].function.GetClass());
        //Manuel adding
        if (consumableobject.TryGetComponent<GetWeapon>(out GetWeapon gw))
        {
            gw.weaponID = Random.Range(0, 1);// max = weapons.Length
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
    }




    public void ResumeGame()
    {
        state = GameState.inGame;
    }

    public void StopGame()
    {
        state= GameState.pause;
    }
    //UI Events
    //player based UI Events
    public void ChangeAmmoText(int newAmmo)
    {
        //0 = curAmmoDisplay
        playerPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newAmmo.ToString();
    }
    public void ChangefullAmmoText(int newAmmo)
    {
        //0 = curAmmoDisplay
        playerPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = newAmmo.ToString();

    }


    public void SpawnEnemy()
    {
        Debug.Log("Bomba");
        GameObject enemy = new();
        enemy.AddComponent<EnemyController>();
        enemy.GetComponent<EnemyController>().m_Enemy = enemies[0];
        enemy.name = "enemy";
//        Instantiate(enemy,new Vector3(2,0,5),new Quaternion(0,0,0,0));

    }

    public void MainMenu()
    {
        //SceneManagement.LoadScene(mainMenuint)
    }

    //GETTER

}