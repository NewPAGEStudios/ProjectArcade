using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeapon : MonoBehaviour
{
    public int weaponID;
    private GameController gc;

    public int consPosID;

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
        transform.GetChild(1).Rotate(0f, 45f * Time.deltaTime, 0f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PlayerColl"))
        {
//            Debug.Log("EscapeStart");
            apply(other.transform.parent.gameObject);
        }
        else
        {
            return;
        }
    }


    private void apply(GameObject player)
    {
        if (gc.pState == GameController.PlayState.inStart)
        {
            player.GetComponent<PController>().HealDMG(100, gameObject);
            gc.escapeStart();
        }
        player.GetComponent<WeaponManager>().GetWeaponR(weaponID);
        gameObject.transform.parent.parent = gc.consumableSpawnPointParent.transform;
        int id = gc.activeCons.IndexOf(consPosID);
        if(gc.pState != GameController.PlayState.inBoss)
        {
            gc.activeCons.RemoveAt(id);
            gc.activeConsID.RemoveAt(id);
            gc.activeConsSkill.RemoveAt(id);
            gc.activeConsWeapID.RemoveAt(id);
        }
        Destroy(gameObject);
        return;
    }

    public static void perform_WOUTObjected(GameObject player, GameController gc, int weaponID)
    {
        player.GetComponent<WeaponManager>().GetWeaponR(weaponID);
        return;
    }

}
