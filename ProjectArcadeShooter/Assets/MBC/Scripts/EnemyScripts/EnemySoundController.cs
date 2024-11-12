using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundController : MonoBehaviour
{
    public GameObject soundParent;
    private void Awake()
    {
        soundParent = gameObject.transform.GetChild(0).Find("Sounds").gameObject;
    }

    public void PlaySound(string name,float delay)
    {
        soundParent.transform.Find(name).GetComponent<AudioSource>().Play();
//        StartCoroutine(delayApplyP(soundParent.transform.Find(name).GetComponent<AudioSource>(), delay));
    }
    public void StopSound(string name, float delay)
    {
        StopAllCoroutines();
        StartCoroutine(delayApplyS(soundParent.transform.Find(name).GetComponent<AudioSource>(), delay));
    }
    public bool SoundPlayState(string name)
    {
        return soundParent.transform.Find(name).GetComponent<AudioSource>().isPlaying;
    }


    IEnumerator delayApplyP(AudioSource audioS,float delay)
    {
        yield return new WaitForSeconds(delay);
        audioS.Play();
    }
    IEnumerator delayApplyS(AudioSource audioS, float delay)
    {
        yield return new WaitForSeconds(delay);

        audioS.Stop();
    }
}
