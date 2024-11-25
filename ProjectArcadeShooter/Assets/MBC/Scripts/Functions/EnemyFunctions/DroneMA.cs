using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneMA : MonoBehaviour
{
    private NavMeshAgent agent;

    public Enemy enemy;

    private float normal_BOffset;
    private float movement_timer;
    private float dash_Time = 1.5f;
    private RaycastHit hit;

    private Vector3 dashPos;
    private void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = enemy.GetComponent<NavMeshAgent>();

        normal_BOffset = agent.baseOffset;
        agent.baseOffset = 2;
    }
    private void Update()
    {
        if (enemy.eState == Enemy.enemyState.dead || enemy.state == Enemy.ccState.stun)
        {
            return;
        }
        if (Physics.Raycast(gameObject.transform.position, -gameObject.transform.position + enemy.Player.transform.position, out hit, Vector3.Distance(gameObject.transform.position, enemy.Player.transform.position)))
        {
            if (!hit.transform.gameObject.CompareTag("Player") && hit.transform.gameObject.layer != 7)
            {
                agent.enabled = true;
                agent.SetDestination(enemy.Player.transform.position);
                if (movement_timer <= 0)
                {
                    StartCoroutine(moveRoutine(Random.Range(1.5f, 2.5f)));
                    movement_timer = 1.5f;
                }
                movement_timer -= Time.deltaTime;
                dash_Time = 1.5f;
            }
            else
            {
                if (dash_Time > 0)
                {
                    agent.enabled = false;
                    transform.LookAt(enemy.Player.transform.position);
                    dashPos = enemy.Player.transform.position;
                    dash_Time -= Time.deltaTime;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, dashPos, Time.deltaTime * 6);
                    if (Vector3.Distance(transform.position, dashPos) < 0.2f)
                    {
                        dash_Time = 1.5f;
                    }
                }
            }
        }
        else
        {

        }
    }
    IEnumerator moveRoutine(float desiredYPos)
    {
        enemy.soundController.PlaySound("Ding", 0f);
        while (true)
        {
            agent.baseOffset = Mathf.MoveTowards(agent.baseOffset, desiredYPos, Time.deltaTime);
            if (agent.baseOffset == desiredYPos)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
