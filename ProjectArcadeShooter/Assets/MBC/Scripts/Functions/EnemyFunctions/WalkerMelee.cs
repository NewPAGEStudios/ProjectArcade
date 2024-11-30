using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkerMelee : MonoBehaviour
{
    private float moveTimer;
    private float shotTimer;
    private float shotTime;
    private float footstepTimer;

    private Quaternion targetRotation;
    private NavMeshAgent agent;
    private GameObject Player;

    public Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = enemy.GetComponent<NavMeshAgent>();
        shotTime = enemy.fireRate / enemy.animator.speed;
        Debug.Log(shotTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.eState == Enemy.enemyState.dead || enemy.state == Enemy.ccState.stun)
        {
            return;
        }
        shotTimer += Time.deltaTime;
        if (shotTimer > shotTime)
        {
            if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.e_type.rangeDistance)
            {
                MeleeAttack();
            }
        }
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.e_type.rangeDistance - 0.05f)
        {
            enemy.Agent.SetDestination(enemy.Player.transform.position);
            enemy.animator.SetBool("isWalking", true);

            if (footstepTimer <= 0)
            {
                enemy.soundController.PlaySound("Footsteps", 0);
                footstepTimer = 0.75f;
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
            footstepTimer = 0.12f;

            targetRotation = Quaternion.LookRotation(enemy.Player.transform.position - enemy.transform.position);
            enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation, targetRotation, .4f);
            enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, enemy.transform.eulerAngles.z);

        }
    }

    public void MeleeAttack()
    {
        shotTimer = 0f;
        if (enemy.animator == null)
        {
            return;
        }
        Debug.Log("Attack Starts");

        enemy.animator.SetTrigger("Attack");
        enemy.animator.SetBool("AttackEnd", true);

        transform.LookAt(enemy.Player.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        StartCoroutine(AttackAnim_Routine());
    }
    IEnumerator AttackAnim_Routine()
    {
        enemy.meleeObj.GetComponent<MeleeDmg>().attackAvaible = true;

        while (true)
        {

            yield return new WaitForEndOfFrame();

            if (enemy.animator == null)
            {
                break;
            }
            if (enemy.animator.GetCurrentAnimatorStateInfo(1).IsName("AttackEnd"))
            {
//                enemy.meleeObj.GetComponent<MeleeDmg>().attackAvaible = false;
                break;
            }

        }
        if (enemy.animator != null)
        {
            enemy.animator.SetBool("AttackEnd", false);
        }


    }

}
