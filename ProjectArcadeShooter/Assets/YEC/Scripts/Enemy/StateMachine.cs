 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class StateMachine : MonoBehaviour
{

    public BaseState activeState;
    public AgentControl agentControl;//

    private void Awake()
    {
        agentControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<AgentControl>();
        Debug.Log(agentControl.gameObject.name);
    }

    public void Initialise(){
        ChangesState( new PatrolState());
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activeState != null){
            activeState.Perform();
        }
    }
    public void ChangesState(BaseState newState){
        if(activeState != null){
            activeState.Exit();
        }
        activeState = newState;
        if(activeState != null){
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
    }
}
