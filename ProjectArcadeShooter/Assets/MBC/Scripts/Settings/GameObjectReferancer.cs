using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectReferancer : MonoBehaviour
{
    [SerializeField]
    private GameObject[] referanced;

    private void OnEnable()
    {
        foreach (GameObject obj in referanced)
        {
            obj.SetActive(true);
        }
    }
    private void OnDisable()
    {
        foreach (GameObject obj in referanced)
        {
            obj.SetActive(false);
        }
    }


}
