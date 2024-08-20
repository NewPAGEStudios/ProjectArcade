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
            if(obj.TryGetComponent<ParticleSystem>(out ParticleSystem objps))
            {
                objps.Play();
            }
            else
            {
                obj.SetActive(true);
            }
        }
    }
    private void OnDisable()
    {
        foreach (GameObject obj in referanced)
        {
            if (obj.TryGetComponent<ParticleSystem>(out ParticleSystem objps))
            {
                objps.Stop();
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }


}
