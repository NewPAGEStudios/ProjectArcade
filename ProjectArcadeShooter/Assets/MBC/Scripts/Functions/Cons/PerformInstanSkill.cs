using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformInstantSkill : MonoBehaviour
{
    private GameController gc;
    public Skill thisSkill;
    private GameObject player;
    GameObject go;

    public int consPosID;
    // Start is called before the first frame update
    void Start()
    {
        go = Instantiate(thisSkill.modelPrefab, gameObject.transform);
        go.transform.localScale = go.transform.localScale / 4;

        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        go.transform.forward = (transform.GetChild(0).position - player.transform.position).normalized;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerColl"))
        {
            System.Type scriptMB = System.Type.GetType(thisSkill.functionName + ",Assembly-CSharp");
            gameObject.AddComponent(scriptMB);
            //manuelHandling
            if(gameObject.TryGetComponent<stunInstanSkill>(out stunInstanSkill sis))
            {
                sis.thisSkilll = thisSkill;
                sis.consPosID = consPosID;
            }
        }
        else
        {
            return;
        }
    }

}
