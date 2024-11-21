using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stunInstanSkill : MonoBehaviour
{
    public Skill thisSkilll;
    GameController gc;

    List <GameObject> affectedGOs = new List <GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<Rigidbody>();
        GetComponent<Rigidbody>().isKinematic = true;
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        StartCoroutine(endEffect());
    }
    IEnumerator endEffect()
    {
        MaterialPropertyBlock m_propertyBlock = new MaterialPropertyBlock();
        m_propertyBlock.SetColor("_BaseColor", new Color(0, 0, 0, 1));
        gameObject.GetComponent<Renderer>().SetPropertyBlock(m_propertyBlock);
        while (m_propertyBlock.GetColor("_BaseColor").a > 0)
        {
            m_propertyBlock.SetColor("_BaseColor", new Color(0, 0, 0, m_propertyBlock.GetColor("_BaseColor").a - 0.04f));
            gameObject.GetComponent<Renderer>().SetPropertyBlock(m_propertyBlock);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("EnemyColl"))
        {
            if (!affectedGOs.Contains(other.gameObject))
            {
                affectedGOs.Add(other.gameObject);
                other.GetComponent<ColliderParenter>().targetOBJ.transform.parent.GetComponent<Enemy>().Stun(0.5f);
            }
        }
    }
}
