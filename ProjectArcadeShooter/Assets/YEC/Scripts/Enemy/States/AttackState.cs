using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shotTimer;

    private Quaternion targetRotation;


    public override void Enter()
    {
        stateMachine.agentControl.AllAgentsAttack();//
    }

    public override void Exit()
    {

    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            stateMachine.agentControl.AllAgentsAttack();//

            if (!enemy.e_type.isRanged)
            {
                losePlayerTimer = 0;
                shotTimer += Time.deltaTime;
                if (shotTimer > enemy.fireRate)
                {
                    if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.e_type.rangeDistance)
                    {
                        Attack();
                    }
                }
                if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.e_type.rangeDistance - 1f)
                {
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                    enemy.animator.SetBool("isWalking", true);

                }
                else
                {
                    enemy.Agent.SetDestination(enemy.transform.position);

                    enemy.animator.SetBool("isWalking", false);

                    targetRotation = Quaternion.LookRotation(enemy.Player.transform.position - enemy.transform.position);
                    enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation, targetRotation, .4f);
                    enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, enemy.transform.eulerAngles.z);

                }
            }
            else
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
                    enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 15));
                    moveTimer = 0;
                }
                enemy.LastKnowPos = enemy.Player.transform.position;
            }
        }
        else
        {
            stateMachine.agentControl.LastKnowPos = enemy.Player.transform.position;//playerin son bulunduğu konumu diğer enemylere aktarmak için
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > 8)
            {
                stateMachine.ChangesState(new SearchState());
            }
        }
    }
    //Functions
    public void Shoot()
    {
        //  enemy.animator.SetTrigger("Throw");//*

        Transform gunbarrel = enemy.gunBarrel;

        Debug.Log(gunbarrel.name);

        Debug.Log(enemy.transform.name);

        Debug.Log(Resources.Load("Prefabs/Bullet").name);


        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunbarrel.position, enemy.transform.rotation);

        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.transform.position).normalized;

        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-3f, 3f), Vector3.up) * shootDirection * 40;
        Debug.Log("Shoot");
         enemy.animator.SetTrigger("shoot");//*
         enemy.animator.SetBool("isWalking",true);//*


        shotTimer = 0;
    }
    public void Attack()
    {
        //TODO: PATLAT DMG AT ardından : gamecontrollerdan düşman sayısını azalt
        enemy.animator.SetTrigger("Attack");
        enemy.animator.SetBool("AttackEnd", true);

        enemy.transform.LookAt(enemy.Player.transform);
        enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, enemy.transform.eulerAngles.z);

    }
}
