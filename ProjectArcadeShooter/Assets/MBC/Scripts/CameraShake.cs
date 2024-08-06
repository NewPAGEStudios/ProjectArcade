using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public AnimationCurve curve;
    public float duration;

    public void StartCamShake()
    {
        StartCoroutine(CameraShakeEffect());
    }
    IEnumerator CameraShakeEffect()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime/duration);
            transform.localPosition = Vector3.zero + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.localPosition = Vector3.zero;
    }

}
