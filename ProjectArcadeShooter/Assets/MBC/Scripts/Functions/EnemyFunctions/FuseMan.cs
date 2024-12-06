using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FuseMan : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject Player;
    private float shotTimer;

    private bool oteventForanimator_id3 = true;
    private int lastShoot;

    public Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = enemy.GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.eState == Enemy.enemyState.dead || enemy.state == Enemy.ccState.stun || enemy.state == Enemy.ccState.blown)
        {
            return;
        }
        shotTimer += Time.deltaTime;
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.e_type.rangeDistance - 2 || Physics.Raycast(gameObject.transform.position + new Vector3(0, 1, 0), enemy.Player.transform.position - gameObject.transform.position, Vector3.Distance(gameObject.transform.position, enemy.Player.transform.position), 16384))
        {
            enemy.Agent.SetDestination(enemy.Player.transform.position);
            enemy.animator.SetBool("isWalking", true);
            enemy.soundController.PlaySound("Footsteps", 0.11f);

        }
        else
        {
            enemy.animator.SetBool("isWalking", false);
            enemy.Agent.SetDestination(enemy.transform.position);
            enemy.soundController.StopSound("Footsteps", 0f);
        }
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) < enemy.e_type.rangeDistance && !Physics.Raycast(gameObject.transform.position + new Vector3(0, 1, 0), enemy.Player.transform.position - gameObject.transform.position, Vector3.Distance(gameObject.transform.position, enemy.Player.transform.position), 16384))
        {
            if (oteventForanimator_id3)
            {
                enemy.animator.SetTrigger("Attack");
                enemy.animator.SetBool("AttackEnd", false);
                oteventForanimator_id3 = false;
            }
            if (shotTimer > enemy.fireRate)
            {
                if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.e_type.rangeDistance)
                {
                    Shoot();
                    
                }
            }
        }
        else
        {
            enemy.animator.SetBool("AttackEnd", true);
            oteventForanimator_id3 = true;
        }

    }

    public void Shoot()
    {
        enemy.transform.LookAt(enemy.Player.transform);
        enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, 0);
        Transform attackPos;
        if(lastShoot == 0)
        {
            attackPos = enemy.gunBarrel[1];
            lastShoot = 1;
        }
        else
        {
            attackPos = enemy.gunBarrel[0];
            lastShoot = 0;
        }
        spawnAmmo(attackPos);
        enemy.soundController.PlaySound("Fire", 0f);
    }
    private void spawnAmmo(Transform gunbarrel)
    {
        GameObject bullet = new();
        bullet.name = "bullet";

        bullet.transform.position = gunbarrel.transform.position;
        bullet.transform.LookAt(enemy.Player.transform.position);
        if (enemy.e_type.EnemyTypeID == 3)
        {
            bullet.transform.eulerAngles = gunbarrel.transform.eulerAngles;
        }


        System.Type scriptAmmo = System.Type.GetType(enemy.e_type.ammo.functionName + ",Assembly-CSharp");
        bullet.AddComponent(scriptAmmo);


        if (bullet.TryGetComponent<FuseFunction>(out FuseFunction ff))
        {
            ff.baseAmmo = enemy.e_type.ammo;
            ff.firedBy = enemy.gameObject;
            ff.target = enemy.Player;
            ff.speedRatio = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) / enemy.e_type.rangeDistance;
            ff.simulatedPos = GameObject.Instantiate(Resources.Load("Prefabs/FuzeIndicator") as GameObject, Vector3.zero, Quaternion.identity);
        }
        

        GameObject go = GameObject.Instantiate(enemy.e_type.ammo.modelGO, bullet.transform);
        go.name = "Model";
        go.layer = 7;

        shotTimer = 0;

    }


}