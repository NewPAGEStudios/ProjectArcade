using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shotTimer;

    private Quaternion targetRotation;

    //NonRanged EnemyType
    public override void Enter()
    {
        enemy.Agent.SetDestination(enemy.transform.position);
    }

    public override void Exit()
    {
        
    }

    public override void Perform()
    {
        if(enemy.CanSeePlayer())
        {
            if (!enemy.enemyType.isRanged)
            {
                losePlayerTimer = 0;
                shotTimer += Time.deltaTime;
                if (shotTimer > enemy.fireRate)
                {
                    if(Vector3.Distance(enemy.Player.transform.position,enemy.transform.position) <= enemy.enemyType.rangeDistance)
                    {
                        Attack();
                    }
                }
                if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.enemyType.rangeDistance)
                {
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                }
                else
                {
                    enemy.Agent.SetDestination(enemy.transform.position);
                    targetRotation = Quaternion.LookRotation(enemy.Player.transform.position - enemy.transform.position);
                    enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation, targetRotation, .7f);
                    enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, enemy.transform.eulerAngles.z);
                }
                enemy.LastKnowPos = enemy.Player.transform.position;
            }
            else //Ranged A
            {
                losePlayerTimer = 0;
                moveTimer += Time.deltaTime;
                shotTimer += Time.deltaTime;
                enemy.transform.LookAt(enemy.Player.transform);
                if (shotTimer > enemy.fireRate)
                {
                    Shoot();
                }
                if (moveTimer > Random.Range(3, 7))
                {
                    enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                    moveTimer = 0;
                }
            }
            enemy.LastKnowPos = enemy.Player.transform.position;
        }
        else
        {
            losePlayerTimer += Time.deltaTime;
            if(losePlayerTimer > 2)
            {
                stateMachine.ChangesState(new SearchState());
            }
        }
    }
    public void  Shoot()
    {
//        enemy.animator.SetTrigger("Throw");
        
        Transform gunbarrel = enemy.gunBarrel;

        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet")as GameObject, gunbarrel.position, enemy.transform.rotation);

        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.transform.position).normalized;

        bullet.layer = 7;

        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-3f,3f),Vector3.up) *  shootDirection*40;
        Debug.Log("Shoot");
//        enemy.animator.SetTrigger("Idle");

        shotTimer =0;
    }

    public void Attack()
    {
        Debug.Log("Attack");
    }
    IEnumerator AttackAnim_Routine()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();
//            if(cartcurt){break;}
        }
        shotTimer = 0;
    }

}
