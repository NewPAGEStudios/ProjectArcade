using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeapon : MonoBehaviour
{
    public int weaponID;
    private GameController gc;
    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        int i;
        for(i = 0 ; i < gc.weapons.Length; i++)
        {
            if(weaponID == gc.weapons[i].WeaponTypeID)
            {
                break;
            }
        }
        GameObject go = Instantiate(gc.weapons[i].modelGameObject, gameObject.transform);
        go.layer = 0;
        for(int z = 0; z < go.transform.childCount; z++)
        {
            go.transform.GetChild(z).gameObject.layer = 0;
        }
        go.transform.localScale = gc.transform.localScale / 2;
    }
    private void Update()
    {
        transform.GetChild(0).Rotate(0f, 45f * Time.deltaTime, 0f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            apply(other.transform.parent.gameObject);
        }
        else
        {
            return;
        }
    }


    private void apply(GameObject player)
    {
        player.GetComponent<WeaponManager>().GetWeapon(weaponID);
        gameObject.transform.parent.parent = gc.spawnPointParent.transform;
        Destroy(gameObject);
        return;
    }
}
