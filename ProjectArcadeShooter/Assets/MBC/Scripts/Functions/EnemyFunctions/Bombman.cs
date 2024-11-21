using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bombman : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject Player;

    public Enemy enemy;
    private float explodeTimer = 0f;
    private float explodeTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.eState == Enemy.enemyState.dead || enemy.state == Enemy.ccState.stun)
        {
            return;
        }
        if(Vector3.Distance(enemy.transform.position,enemy.Player.transform.position) < 2f)
        {
            agent.speed = 0f;
            enemy.animator.SetBool("isWalking", false);

            enemy.fxController.work("GetReadyToExplode");
            if (!enemy.soundController.SoundPlayState("ExplodeGettingReady"))
            {
                enemy.soundController.PlaySound("ExplodeGettingReady", 0);
            }
            explodeTimer += Time.deltaTime;
        }
        else if(Vector3.Distance(enemy.transform.position,enemy.Player.transform.position) < enemy.e_type.rangeDistance)
        {
            agent.speed = enemy.e_type.moveSpeed * 0.5f;
            agent.SetDestination(enemy.Player.transform.position);
            enemy.animator.SetBool("isWalking", true);
            explodeTimer += Time.deltaTime;

            enemy.fxController.work("GetReadyToExplode");
            if (!enemy.soundController.SoundPlayState("ExplodeGettingReady"))
            {
                enemy.soundController.PlaySound("ExplodeGettingReady", 0);
            }
        }
        else
        {
            agent.speed = enemy.e_type.moveSpeed;
            agent.SetDestination(enemy.Player.transform.position);
            enemy.animator.SetBool("isWalking", true);
            explodeTimer = 0f;

            enemy.fxController.stop("GetReadyToExplode");
            enemy.soundController.StopSound("ExplodeGettingReady", 0);
        }

        if (explodeTimer >= explodeTime)
        {
            explode();
        }
    }
    public void explode()
    {
        enemy.soundController.StopSound("ExplodeGettingReady", 0);
        enemy.soundController.PlaySound("Explosion", 0);
        if(Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) < enemy.e_type.rangeDistance)
        {
            enemy.Player.GetComponent<PController>().TakeDMG(enemy.e_type.attackDMG, enemy.transform.gameObject);
            enemy.Player.GetComponent<PController>().ThrowPlayer(enemy.transform.position);
        }
        enemy.ehp.Die();
    }
}
