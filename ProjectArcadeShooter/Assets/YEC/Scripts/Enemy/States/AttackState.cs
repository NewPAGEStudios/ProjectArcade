using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shotTimer;
    private Quaternion targetRotation;
    private NavMeshAgent agent;
    private GameObject Player;

    public override void Enter()
    {
        stateMachine.agentControl.AllAgentsAttack();//
    }

    public override void Exit()
    {

    }
    public override void ResetAttack()
    {
        shotTimer = 0;
    }

    public override void Perform()
    {
        // if(enemy.e_type.EnemyTypeID == 1)
        // {
        //     enemy.Agent.SetDestination(enemy.Player.transform.position);
        //     BaseOffsetValueControl();

        // }
        if (enemy.CanSeePlayer())
        {
            stateMachine.agentControl.AllAgentsAttack();//
            if(enemy.e_type.isFlyable)//uçabilen
            {
                if (!enemy.e_type.isRanged)//yakın
                {
                    //TODO: uçan ve yakın patlama şeysi yazlıcak
                }
                else//uzak
                {
                    losePlayerTimer = 0;
                    moveTimer += Time.deltaTime;
                    shotTimer += Time.deltaTime;

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
            else//uçamayan
            {
                if (!enemy.e_type.isRanged)//yakın
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
                else//uzak
                {
                    losePlayerTimer = 0;
                    moveTimer += Time.deltaTime;
                    shotTimer += Time.deltaTime;

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
        enemy.transform.LookAt(enemy.Player.transform);

        Transform gunbarrel = enemy.gunBarrel;

        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunbarrel.position, enemy.transform.rotation);

        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.transform.position).normalized;

        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-3f, 3f), Vector3.up) * shootDirection * 40;
        enemy.animator.SetTrigger("shoot");
        enemy.animator.SetBool("isWalking",true);

        shotTimer = 0;
    }
    public void Attack()
    {
        enemy.MeleeAttack();
    }
    public void BaseOffsetValueControl(){
        // enemy.Agent.SetDestination(enemy.Player.transform.position);
        // Debug.Log("aradaki mesafe"+distance+"range"+ e_type.rangeDistance);
        agent = enemy.Agent;
        Player = enemy.Player;
        agent.radius = 0.6f;// nedense 0.5 değerinde player yüksekteyken takip etmiyor
        float distance = Vector3.Distance(Player.transform.position, enemy.transform.position);
        float playerHight = Player.transform.position.y - enemy.transform.position.y;
        // Debug.Log(playerHight);
        RaycastHit hitInfo = new RaycastHit();
        if(Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hitInfo, 3) && hitInfo.collider.gameObject.tag != "PlayerColl" && hitInfo.collider.gameObject.tag != "EnemyColl")
        {
            Debug.Log("nesne:" + hitInfo.collider.name+ "tag:"+ hitInfo.collider.gameObject.tag);
            agent.speed = 0.1f;
            agent.baseOffset += 0.07f;
            // if(hitInfo.distance < 5){

            // }
            // else{
            //     Debug.Log("hızlan");
            //     agent.speed = 5;
            // }
        }
        else
        {
            agent.speed = 5;
        }

        if(distance <= 15 && agent.baseOffset >= agent.baseOffset + playerHight)
        {

            // -8.935724
            // if(Physics.Raycast(transform.position, transform.forward, out hitInfo, 15))
            // {

            //     /////////////////
            //     // if(hitInfo.transform.CompareTag("PlayerColl"))
            //     // {

            //     // }
            //     // else
            //     // {
            //     //     if(player.transform.position.y > transform.position.y)
            //     //     {
                        
            //     //         // Mathf.MoveTowards(agent.baseOffset,transform.position.y,2);

            //     //         agent.baseOffset += playerHight;
            //     //     }
            //     // }
            // }
            for(float i = distance + 1f; i >= distance; i-= 0.5f)
            {
                agent.baseOffset -= 0.01f; 
            }
        }
        
        if(agent.baseOffset < enemy.currentBaseOffeset + playerHight && distance >=15)
        {
            agent.baseOffset += 0.01f;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //rampada
        Ray downwardRay = new Ray(enemy.transform.position, Vector3.down);
        RaycastHit hitDownInfo = new RaycastHit();
        if(Physics.Raycast(downwardRay, out hitDownInfo))
        {
            if(hitDownInfo.distance < 0.9)
            {
                agent.baseOffset += 0.04f;
            }
        }
    }
}
