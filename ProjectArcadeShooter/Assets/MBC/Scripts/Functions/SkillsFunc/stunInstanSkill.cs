using System.Collections;
using UnityEngine;

public class stunInstanSkill : MonoBehaviour
{
    public Skill thisSkilll;
    GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        go = Instantiate(thisSkilll.modelPrefab,gameObject.transform);
        go.transform.localPosition = new(0, -1, 0);
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
        while (m_propertyBlock.GetColor("_BaseColor").a > 0)
        {
            m_propertyBlock.SetColor("_BaseColor", new Color(0, 0, 0, m_propertyBlock.GetColor("_BaseColor").a - 0.1f));
            yield return new WaitForSeconds(0.001f);
        }
    }
}
