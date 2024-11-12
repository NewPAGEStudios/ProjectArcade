using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class zzz : MonoBehaviour
{

    [SerializeField] RectTransform canvasTransform;

    GraphicRaycaster raycaster;

    //dragging
    List<GameObject> DragTargets = new List<GameObject>();

    GameObject last_enterGO;
    GameObject last_exitGO;

    void Start()
    {
        raycaster=GetComponent<GraphicRaycaster>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCursorInput(Vector2 normalisedPostion)
    {

        Vector3 mousePos = new Vector3(canvasTransform.sizeDelta.x * normalisedPostion.x, canvasTransform.sizeDelta.y * normalisedPostion.y, 0f);

        PointerEventData mouseEvent = new PointerEventData(EventSystem.current);
        mouseEvent.position = mousePos;

        
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(mouseEvent, results);

        bool sendMouseDown = Input.GetMouseButtonDown(0);
        bool sendMouseUp = Input.GetMouseButtonUp(0);

        //dragging
        bool isMouseDown = Input.GetMouseButton(0);

        //dragging
        if (sendMouseUp)
        {
            foreach(var target in DragTargets)
            {
                if (ExecuteEvents.Execute(target, mouseEvent, ExecuteEvents.endDragHandler)) break;
            }
            DragTargets.Clear();
        }

        foreach(var result in results)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePos;
            eventData.pointerCurrentRaycast = eventData.pointerPressRaycast = result;

            if (last_exitGO != result.gameObject)
            {
                ExecuteEvents.Execute(last_exitGO, eventData, ExecuteEvents.pointerExitHandler);
                last_exitGO = result.gameObject;
            }
            if (last_enterGO != result.gameObject)
            {
                ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
                last_enterGO = result.gameObject;
            }

            //Dragging
            if (isMouseDown)
            {
                eventData.button = PointerEventData.InputButton.Left;
            }

            //new Dragging
            if (sendMouseDown)
            {
                if(ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.beginDragHandler)) DragTargets.Add(result.gameObject);
            }
            else if(DragTargets.Contains(result.gameObject))
            {
                eventData.dragging = true;
                ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.dragHandler);
            }

            //click/clickUp
            if (sendMouseDown)
            {
                Debug.Log(result);
                if (ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerDownHandler)) break;
            }
            else if (sendMouseUp)
            {
                bool didRun = ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerUpHandler);
                didRun |= ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerClickHandler);
                if (didRun) break;
            }

            return;
        }
    }
}
