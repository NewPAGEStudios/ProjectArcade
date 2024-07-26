using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class  AgentControl : MonoBehaviour
{
    public List<NavMeshAgent> allAgents = new List<NavMeshAgent>();
    private StateMachine stateMachine;
    void Start()
    {
        allAgents = Object.FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None).ToList();
    }

    // Update is called once per frame
    
    public void AllAgentsAttack()
    {
        
        foreach (var agent in allAgents)
        {
            var enemy = agent.GetComponent<Enemy>();
            if (enemy.CanSeePlayer() == false){
                enemy.stateMachine.ChangesState(new SearchState());
                Debug.Log($"search state agent: {agent.name}");
            }
        }
    }
    void Update()
    {
    }

    
}
