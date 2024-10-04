using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetActiveSkill : MonoBehaviour
{
    public int skillId;
    private GameController gc;
    private Skill thisSkill;
    private GameObject player;
    GameObject go;

    public int consPosID;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        int i;
        for (i = 0; i < gc.skills.Length; i++)
        {
            if (skillId == gc.skills[i].skillTypeID)
            {
                thisSkill = gc.skills[i];
                break;
            }
        }


        go = Instantiate(gc.skills[i].modelShow,gameObject.transform);
        go.transform.localScale = go.transform.localScale/4;

        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        go.transform.forward = (transform.GetChild(1).position - player.transform.position).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerColl"))
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
        player.GetComponent<WeaponManager>().getSkill(thisSkill);
        gameObject.transform.parent.parent = gc.consumableSpawnPointParent.transform;
        int id = gc.activeCons.IndexOf(consPosID);
        if (gc.pState != GameController.PlayState.inBoss)
        {
            gc.activeCons.RemoveAt(id);
            gc.activeConsID.RemoveAt(id);
            gc.activeConsSkill.RemoveAt(id);
            gc.activeConsWeapID.RemoveAt(id);
        }
        gc.decreseCompassObject(gameObject);
        Destroy(gameObject);
        return;
    }
    public static void perform_WOUTObjected(GameObject player, GameController gc, int skillIndex)
    {
        player.GetComponent<WeaponManager>().getSkill(gc.skills[skillIndex]);
        return;
    }
}
