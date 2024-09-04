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

    private void Awake()
    {
        agentControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<AgentControl>();
        enemy = GetComponent<Enemy>();
    }

    public void Initialise(){
        if(enemy.e_type.isFlyable && !enemy.e_type.isRanged)
        {
            ChangesState(new AttackState());
        }
        else
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
