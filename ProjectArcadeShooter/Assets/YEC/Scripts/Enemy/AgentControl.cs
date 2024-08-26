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
     private Vector3 lastKnowPos;
    public bool anybodySee = false;

    public Vector3 LastKnowPos { get => lastKnowPos; set => lastKnowPos = value; }

    
    void Start()
    {
    }

    // Update is called once per frame
    
    public void AllAgentsAttack()
    {
        allAgents = Object.FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None).ToList();
        
        foreach (var agent in allAgents)
        {

            var enemy = agent.GetComponent<Enemy>();
            if (enemy.CanSeePlayer() == false)
            {//Her düşmanın playeri görüp görmediğini kontrol ederiz.
                enemy.stateMachine.ChangesState(new SearchState());
                // Debug.Log($"search state agent: {agent.name}");

            }
            else
            {
                anybodySee = true;
            }
        }
    }
    void Update()
    {
    }

    
}
