using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class zzzz : MonoBehaviour
{

    [SerializeField] LayerMask RaycastMask;
    [SerializeField] float distance;
    [SerializeField] UnityEvent<Vector2> OnCursorInput = new UnityEvent<Vector2>();
    [SerializeField] Camera mainCam;
    bool screenIsWorking = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!screenIsWorking)
        {
            return;
        }

        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if(Physics.Raycast(mouseRay,out hit, distance, RaycastMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject != gameObject) return;
            OnCursorInput.Invoke(hit.textureCoord);
//            Debug.Log(hit.textureCoord);
        }
    }
    public void open()
    {
        screenIsWorking = true;
    }
    public void close()
    {
        screenIsWorking = false;
    }

}
