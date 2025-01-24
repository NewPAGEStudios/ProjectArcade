using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeFunctionss : MonoBehaviour
{
    public float dmg;
    private GameObject gamePlayer;
    private List<GameObject> gameObjects = new();
    private void Start()
    {
        gamePlayer = GameObject.FindGameObjectWithTag("Player").gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyColl"))
        {
            if (!gameObjects.Contains(other.GetComponent<ColliderParenter>().targetOBJ.transform.parent.gameObject))
            {
                other.GetComponent<ColliderParenter>().targetOBJ.transform.parent.GetComponent<EnemyHealth>().EnemyHealthUpdate(-dmg, gamePlayer);
                gameObjects.Add(other.GetComponent<ColliderParenter>().targetOBJ);
            }
        }
    }
    public void ClearGOS()
    {
        gameObjects.Clear();
    }
}
