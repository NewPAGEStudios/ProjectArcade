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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyCol"))
        {
            //other.transform.parent.parent.GetComponent<EnemyHealth>().stun();
            Debug.Log("Enemy Bombed");
        }
    }
}
