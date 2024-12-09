using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialCheck : MonoBehaviour
{
    public Camera mainCam;
    public GameObject panelTutorial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(mouseRay,out RaycastHit hit, 10))
        {
            if (hit.transform.CompareTag("duvar"))
            {
                panelTutorial.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    hit.transform.GetComponent<TutorialButton>().startTutorial();
                }
            }
        }
    }
}
