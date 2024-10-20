using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : BaseState
{
    private float moveTimer;
//    private float losePlayerTimer;
    private float shotTimer;
    private float footstepTimer;

    private Quaternion targetRotation;
    private NavMeshAgent agent;
    private GameObject Player;

    //OTEvent
    private bool oteventForanimator_id3 = true;

    public override void Enter()
    {
        // stateMachine.agentControl.AllAgentsAttack();//
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
        // stateMachine.agentControl.AllAgentsAttack();//
        if(enemy.e_type.isFlyable)//uçabilen
        {
            if (!enemy.e_type.isRanged)//yakın
            {
                enemy.Agent.SetDestination(enemy.Player.transform.position);
                BaseOffsetValueControl();
            }
            else//uzak
            {
                BaseOffsetValueControl();
                if(Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.e_type.rangeDistance)
                {
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                }
                enemy.transform.LookAt(enemy.Player.transform.position);

                moveTimer += Time.deltaTime;
                shotTimer += Time.deltaTime;

                if (shotTimer > enemy.fireRate)
                {
                    if(Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.e_type.rangeDistance)
                    {
                        Shoot();
                    }
                }
                if (moveTimer > Random.Range(3, 7))
                {
                    enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 15));
                    moveTimer = 0;
                }

            }
        }
        else//uçamayan
        {
            if (!enemy.e_type.isRanged)//yakın
            {
                shotTimer += Time.deltaTime;
                if (shotTimer > enemy.fireRate)
                {
                    if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.e_type.rangeDistance)
                    {
                        //Attack();
                    }
                }
                if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.e_type.rangeDistance - 1f)
                {
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                    enemy.animator.SetBool("isWalking", true);

                    if (footstepTimer <= 0)
                    {
                        enemy.soundController.PlaySound("Footsteps", 0);
                        switch (enemy.e_type.EnemyTypeID)
                        {
                            case 0:
                                footstepTimer = 0.75f;
                                break;
                            case 1:
                                footstepTimer = 0.75f;
                                break;
                            case 2:
                                footstepTimer = 0.35f;
                                break;
                            case 3:
                                footstepTimer = 0.35f;
                                break;
                            default: break;
                        }

                    }
                    else
                    {
                        footstepTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    enemy.Agent.SetDestination(enemy.transform.position);

                    enemy.animator.SetBool("isWalking", false);

                    enemy.soundController.StopSound("Footsteps", 0f);
                    switch (enemy.e_type.EnemyTypeID)
                    {
                        case 0:
                            footstepTimer = 0.12f;
                            break;
                        case 1:
                            footstepTimer = 0.11f;
                            break;
                        case 2:
                            footstepTimer = 0.1f;
                            break;
                        case 3:
                            footstepTimer = 0.1f;
                            break;
                        default: break;
                    }

                    targetRotation = Quaternion.LookRotation(enemy.Player.transform.position - enemy.transform.position);
                    enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation, targetRotation, .4f);
                    enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, enemy.transform.eulerAngles.z);

                }
            }
            else//uzak
            {
                moveTimer += Time.deltaTime;
                shotTimer += Time.deltaTime;
                //move
                if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.e_type.rangeDistance - 2)
                {
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                    enemy.animator.SetBool("isWalking", true);
                    switch (enemy.e_type.EnemyTypeID)
                    {
                        case 0:
                            enemy.soundController.PlaySound("Footsteps", 0.12f);
                            break;
                        case 1:
                            enemy.soundController.PlaySound("Footsteps", 0.11f);
                            break;
                        case 2:
                            enemy.soundController.PlaySound("Footsteps", 0.10f);
                            break;
                        case 3:
                            enemy.soundController.PlaySound("Footsteps", 0.10f);
                            break;
                        case 4:
                            break;
                        case 5:
                            break;
                        default: break;
                    }

                }
                else
                {
                    enemy.animator.SetBool("isWalking", false);
                    enemy.soundController.StopSound("Footsteps", 0f);
                }
                //Shoot
                if (enemy.e_type.EnemyTypeID == 3)//FüzeAtarSpecialState
                {
                    if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) < enemy.e_type.rangeDistance)
                    {
                        if (oteventForanimator_id3)
                        {
                            enemy.animator.SetTrigger("Attack");
                            enemy.animator.SetBool("AttackEnd", false);
                        }
                        if (shotTimer > enemy.fireRate)
                        {
                            if(Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.e_type.rangeDistance)
                            Shoot();
                        }
                    }
                    else
                    {
                        enemy.animator.SetBool("AttackEnd", true);
                    }
                }
                else//gerisi
                {
                    if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) < enemy.e_type.rangeDistance && shotTimer > enemy.fireRate)
                    {
                        Shoot();
                        if (moveTimer > Random.Range(3, 7))
                        {
                            enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 15));
                            moveTimer = 0;
                        }
                    }
                }

            }
        }
    }
    //Functions
    public void Shoot()
    {
        //  enemy.animator.SetTrigger("Throw");/

        if (enemy.e_type.EnemyTypeID != 5)
        {
            enemy.transform.LookAt(enemy.Player.transform);
            enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, 0);
        }
        else//uçan olduğu için daha iyi gözükür
        {
            enemy.transform.LookAt(enemy.Player.transform);
        }

        Transform[] gunbarrel = enemy.gunBarrel.ToArray();

        for (int c = 0; c < gunbarrel.Length; c++)
        {

            GameObject bullet = new();

            bullet.name = "bullet";

            bullet.transform.position = gunbarrel[c].transform.position;
            bullet.transform.LookAt(enemy.Player.transform.position);
            if (enemy.e_type.EnemyTypeID == 3)
            {
                bullet.transform.eulerAngles = gunbarrel[c].transform.eulerAngles;
            }


            System.Type scriptAmmo = System.Type.GetType(enemy.e_type.ammo.functionName + ",Assembly-CSharp");
            bullet.AddComponent(scriptAmmo);


            //ManuelAdding
            if (bullet.TryGetComponent<ReflectBulletFunctions>(out ReflectBulletFunctions rbf))
            {
                //Şuan yok
            }
            else if (bullet.TryGetComponent<FuseFunction>(out FuseFunction ff))
            {
                ff.baseAmmo = enemy.e_type.ammo;
                ff.firedBy = enemy.gameObject;
                ff.simulatedPos = GameObject.Instantiate(Resources.Load("Prefabs/FuzeIndicator") as GameObject, Vector3.zero, Quaternion.identity);
            }
            else if (bullet.TryGetComponent<NormalBulletFunction>(out NormalBulletFunction nbf))
            {
                nbf.baseAmmo = enemy.e_type.ammo;
                nbf.firedBy = enemy.gameObject;
            }
            GameObject go = GameObject.Instantiate(enemy.e_type.ammo.modelGO, bullet.transform);
            go.name = "Model";
            go.layer = 7;
        }
        if (enemy.e_type.EnemyTypeID != 3)
        {
            enemy.animator.SetTrigger("shoot");
        }

        shotTimer = 0;

    }
    //public void Attack()
    //{
    //    enemy.MeleeAttack();
    //}
    public void BaseOffsetValueControl()
    {
        agent = enemy.Agent;
        Player = enemy.Player;
        agent.radius = 0.6f;// nedense 0.5 değerinde player yüksekteyken takip etmiyor
        float distance = Vector3.Distance(Player.transform.position, enemy.transform.position);
        float playerHight = Player.transform.position.y - enemy.transform.position.y;

        // Debug.Log(playerHight);
        RaycastHit hitInfo = new RaycastHit();
        //duvar ile karşılaştığında
        if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hitInfo, 3) && hitInfo.collider.gameObject.tag != "PlayerColl" && hitInfo.collider.gameObject.tag != "EnemyColl")
        {
            // Debug.Log("nesne:" + hitInfo.collider.name+ "tag:"+ hitInfo.collider.gameObject.tag);
            agent.speed = 0.1f;
            agent.baseOffset += 0.07f;
        }
        else
        {
            agent.speed = 5;
        }

        //rampada
        Ray downwardRay = new Ray(enemy.transform.position, Vector3.down);
        RaycastHit hitDownInfo = new RaycastHit();
        if (Physics.Raycast(downwardRay, out hitDownInfo))
        {
            if (hitDownInfo.distance < 0.9)
            {
                agent.baseOffset += 0.04f;
            }
        }
        if (!enemy.e_type.isRanged)//uçan yakın
        {
            if (distance <= 15 && agent.baseOffset >= agent.baseOffset + playerHight)
            {
                for (float i = distance + 1f; i >= distance; i -= 0.5f)//dronun yavaş yavaş indiği izlenimini vermek için for döngüsü kullanılır.
                {
                    agent.baseOffset -= 0.01f;
                }
            }
            if (agent.baseOffset < enemy.currentBaseOffeset + playerHight /*&& distance >=15)
            {
                agent.baseOffset += 0.01f;
            }
        }
        else//uçan uzak
        {

            playerHight += enemy.currentBaseOffeset - 2;

            if(0 < playerHight /*&& distance >=15*/)
            {
                agent.baseOffset += 0.02f;
            }
            else
            {
                agent.baseOffset -= 0.02f;
            }
        }
    }
}
