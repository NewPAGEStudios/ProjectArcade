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
        InvokeRepeating("agentListAdd",1f,10f);
    }

    // Update is called once per frame
    
    public void AllAgentsAttack()
    {
        foreach (var agent in allAgents)
        {

            var enemy = agent.GetComponent<Enemy>();
            if (enemy.CanSeePlayer() == false)
            {
                
                //Her düşmanın playeri görüp görmediğini kontrol ederiz.
                if(enemy.e_type.isFlyable && !enemy.e_type.isRanged)//uçan yakın
                {
                    enemy.stateMachine.ChangesState(new AttackState());
                }
                else
                {
                    enemy.stateMachine.ChangesState(new SearchState());
                }
            }
            else
            {
                anybodySee = true;
            }
        }
    }
    private void agentListAdd()
    {
        allAgents = Object.FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None).ToList();
    }
}
