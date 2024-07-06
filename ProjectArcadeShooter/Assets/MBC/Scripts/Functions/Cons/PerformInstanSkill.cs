using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformInstantSkill : MonoBehaviour
{
    private GameController gc;
    public Skill thisSkill;
    private GameObject player;
    GameObject go;
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
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.AddComponent(thisSkill.function.GetClass());
            //manuelHandling
            if(gameObject.TryGetComponent<stunInstanSkill>(out stunInstanSkill sis))
            {
                sis.thisSkilll = thisSkill;
            }
        }
        else
        {
            return;
        }
    }

}
