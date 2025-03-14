using System.Collections;
using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex;
    public float waitTimer;

    private bool isWalking;

    public override void Enter()
    {
        enemy.Agent.SetDestination(enemy.transform.position);
        // Initialize the walking animation state
        isWalking = false;
        enemy.animator.SetBool("isWalking", isWalking);
    }

    public override void ResetAttack(){}


    public override void Perform()
    {
        PatrolCycle();

        //if (enemy.CanSeePlayer())
        //{
        //    stateMachine.ChangesState(new AttackState());
        //}
        //else
        //{
        //    // Update animator walking state
        //    //enemy.animator.SetBool("isWalking", isWalking);
        //}
    }

    public override void Exit()
    {
        // Ensure walking animation state is reset
        isWalking = false;
        enemy.animator.SetBool("isWalking", isWalking);
    }

    public void PatrolCycle()
    {
        if (enemy.Agent.remainingDistance < 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > 2)
            {
                if (waypointIndex < enemy.path.waypoints.Count - 1)
                    waypointIndex++;
                else
                    waypointIndex = 0;

                enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex]);
                enemy.gameObject.transform.LookAt(enemy.path.waypoints[waypointIndex]);
                enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, enemy.transform.eulerAngles.z);
                waitTimer = 0;

                // Start walking animation
                isWalking = true;
                enemy.animator.SetBool("isWalking", isWalking);
            }
            else
            {
                isWalking = false;
                enemy.animator.SetBool("isWalking", isWalking);
            }
        }
    }
}
