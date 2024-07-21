using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shotTimer;

    private Quaternion targetRotation;

    private bool isWalking;

    //NonRanged e_type
    public override void Enter()
    {
        enemy.Agent.SetDestination(enemy.transform.position);
        //Animation Handling
        isWalking = false;
        enemy.animator.SetBool("isWalking", isWalking);

    }

    public override void Exit()
    {
        
    }

    public override void Perform()
    {
        if(enemy.CanSeePlayer())
        {
            if (!enemy.e_type.isRanged)
            {
                losePlayerTimer = 0;
                shotTimer += Time.deltaTime;
                if (shotTimer > enemy.fireRate)
                {
                    if(Vector3.Distance(enemy.Player.transform.position,enemy.transform.position) <= enemy.e_type.rangeDistance)
                    {
                        Attack();
                    }
                }
                if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.e_type.rangeDistance - 1f)
                {
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                    isWalking = true;
                    enemy.animator.SetBool("isWalking", isWalking);

                }
                else
                {
                    enemy.Agent.SetDestination(enemy.transform.position);

                    isWalking = false;
                    enemy.animator.SetBool("isWalking", isWalking);
                    
                    targetRotation = Quaternion.LookRotation(enemy.Player.transform.position - enemy.transform.position);
                    enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation, targetRotation, .4f);
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
                    isWalking = true;
                    enemy.animator.SetBool("isWalking", isWalking);
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
    public void Shoot()
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
        enemy.animator.SetTrigger("Attack");
        enemy.animator.SetBool("AttackEnd",true);

        enemy.transform.LookAt(enemy.Player.transform);
        enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, enemy.transform.eulerAngles.z);

    }
    IEnumerator AttackAnim_Routine()
    {
        while(true)
        {

            yield return new WaitForEndOfFrame();

            if(enemy.animator.GetCurrentAnimatorStateInfo(1).IsName("AttackEnd"))
            {
                if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.e_type.rangeDistance)
                {
                    enemy.Player.GetComponent<PController>().TakeDMG(20,enemy.gameObject);
                }
                break;
            }

        }

        enemy.animator.SetBool("AttackEnd", false);

        shotTimer = 0;

    }

}
