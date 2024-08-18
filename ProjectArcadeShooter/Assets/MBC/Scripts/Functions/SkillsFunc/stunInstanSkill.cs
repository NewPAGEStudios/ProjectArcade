using System;
using System.Collections;
using UnityEngine;

public class stunInstanSkill : MonoBehaviour
{
    public Skill thisSkilll;
    GameObject go;
    GameController gc;

    public int consPosID;
    // Start is called before the first frame update
    void Start()
    {
        go = Instantiate(thisSkilll.modelPrefab,gameObject.transform);
        go.transform.localPosition = new(0, -1, 0);

        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        performfunc();
    }
    void performfunc()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = 0; i < enemies.Length; i++)
        {
            if (Vector3.Distance(gameObject.transform.position, enemies[i].transform.position) < 5)//düþman 5 birim uzaklýktan küçükse stunla
            {
                //enemies[i].actionState = ActionState.stunned;
                Debug.Log(enemies[i].name + " stunned");
            }
        }
        StartCoroutine(endEffect());
    }
    IEnumerator endEffect()
    {
        MaterialPropertyBlock m_propertyBlock = new MaterialPropertyBlock();
        m_propertyBlock.SetColor("_BaseColor", new Color(0, 0, 0, 1));
        go.GetComponent<Renderer>().SetPropertyBlock(m_propertyBlock);
        while (m_propertyBlock.GetColor("_BaseColor").a > 0)
        {
            m_propertyBlock.SetColor("_BaseColor", new Color(0, 0, 0, m_propertyBlock.GetColor("_BaseColor").a - 0.01f));
            go.GetComponent<Renderer>().SetPropertyBlock(m_propertyBlock);
            yield return new WaitForSeconds(0.1f);
        }

    }
}
