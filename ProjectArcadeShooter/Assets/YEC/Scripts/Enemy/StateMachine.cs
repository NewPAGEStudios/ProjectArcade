 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class StateMachine : MonoBehaviour
{

    public BaseState activeState;
    public AgentControl agentControl;
    private Enemy enemy;
    private GameController gc;

    private void Awake()
    {
        agentControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<AgentControl>();
        enemy = GetComponent<Enemy>();
    }

    public void Initialise(){
        
        ChangesState( new AttackState());
    }
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gc.state == GameController.GameState.pause || gc.state == GameController.GameState.inSkillMenu)
        {
            return;
        }
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
