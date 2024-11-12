using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLine : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask layerMask;
    public float defaultLength = 50;
    public int maxReflection = 3;
    public Material[] mats;

    private LineRenderer _lineRenderer;
    private RaycastHit hit;

    private Ray ray;

    private GameController gc;

    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _lineRenderer = GetComponent<LineRenderer>();
//        _lineRenderer.material = mats[0];
    }
    private void Update()
    {
        ReflectLaser();
    }
    public void ReflectLaser()
    {
        ray = new Ray(transform.position, transform.forward);

        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0,transform.position);

        float remainLength = defaultLength;

        //_lineRenderer.material = mats[0];
        for (int i = 0; i < maxReflection; i++)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, remainLength, layerMask, QueryTriggerInteraction.Ignore))
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, hit.point);
                remainLength -= Vector3.Distance(ray.origin, hit.point);

                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
                if (hit.transform.gameObject.CompareTag("EnemyColl"))
                {
                    _lineRenderer.material = mats[1];
                    gc.crossColorChange(Color.green);
                    break;
                }
                else
                {
                    _lineRenderer.material = mats[0];
                    gc.crossColorChange(Color.white);
                }
            }
            else
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, ray.origin + (ray.direction * remainLength));
            }
        }
    }
    private void OnDisable()
    {
        gc.crossColorChange(Color.white);
    }
}
