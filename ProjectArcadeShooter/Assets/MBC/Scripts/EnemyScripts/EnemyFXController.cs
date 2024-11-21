using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class EnemyFXController : MonoBehaviour
{
    public GameObject vfxParent;
    private void Awake()
    {
        vfxParent = gameObject.transform.GetChild(0).Find("Vfxs").gameObject;
    }
    public void work(string vfxName)
    {
        if (!vfxParent.transform.Find(vfxName).GetComponentInChildren<ParticleSystem>().isPlaying)
        {
            vfxParent.transform.Find(vfxName).GetComponentInChildren<ParticleSystem>().Play();
        }
    }
    public void stop(string vfxName)
    {
        if (vfxParent.transform.Find(vfxName).GetComponentInChildren<ParticleSystem>().isPlaying)
        {
            vfxParent.transform.Find(vfxName).GetComponentInChildren<ParticleSystem>().Stop();
        }

    }
}
