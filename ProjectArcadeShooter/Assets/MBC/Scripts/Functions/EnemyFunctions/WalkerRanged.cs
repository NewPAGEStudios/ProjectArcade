using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WalkerRanged : MonoBehaviour
{
    private float moveTimer;
    private float shotTimer;
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
    }


    // Update is called once per frame
    void Update()
    {
        if (enemy.eState == Enemy.enemyState.dead || enemy.state == Enemy.ccState.stun)
        {
            return;
        }
        moveTimer += Time.deltaTime;
        shotTimer += Time.deltaTime;

        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.e_type.rangeDistance - 2 || Physics.Raycast(enemy.gunBarrel[0].transform.position + new Vector3(0,1,0), enemy.Player.transform.position - enemy.gunBarrel[0].transform.position,Vector3.Distance(enemy.gunBarrel[0].transform.position, enemy.Player.transform.position),16384))
        {
            enemy.Agent.SetDestination(enemy.Player.transform.position);
            lookPlayer();
            enemy.animator.SetBool("isWalking", true);
            enemy.soundController.PlaySound("Footsteps", 0.11f);

        }
        else
        {
            enemy.animator.SetBool("isWalking", false);
            enemy.Agent.SetDestination(enemy.transform.position);
            enemy.soundController.StopSound("Footsteps", 0f);
        }
        //Shoot
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) < enemy.e_type.rangeDistance && !Physics.Raycast(enemy.gunBarrel[0].transform.position + new Vector3(0, 1, 0), enemy.Player.transform.position - enemy.gunBarrel[0].transform.position, Vector3.Distance(enemy.gunBarrel[0].transform.position, enemy.Player.transform.position), 16384))
        {
            if(shotTimer >= enemy.e_type.attackRatio)
            {
                Shoot();
                shotTimer = 0;
            }
            if (moveTimer > Random.Range(1.5f, 3))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 15));
                moveTimer = 0;
            }
        }

    }
    public void Shoot()
    {
        //  enemy.animator.SetTrigger("Throw");/

        if (enemy.e_type.EnemyTypeID != 5)
        {
            enemy.transform.LookAt(enemy.Player.transform);
            enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, 0);
        }
        else//u�an oldu�u i�in daha iyi g�z�k�r
        {
            enemy.transform.LookAt(enemy.Player.transform);
        }
        enemy.soundController.PlaySound("ShootSound", 0f);
        Transform[] gunbarrel = enemy.gunBarrel.ToArray();

        for (int c = 0; c < gunbarrel.Length; c++)
        {
            StartCoroutine(waitforAnimEnd(gunbarrel[c]));
        }


    }
    IEnumerator waitforAnimEnd(Transform gunbarrel)
    {
        if (enemy.e_type.EnemyTypeID != 3)
        {
            enemy.animator.SetTrigger("shoot");
        }
        while (true)
        {
            if (enemy.animator == null)
            {
                StopAllCoroutines();
                break;
            }

            if (enemy.animator.GetCurrentAnimatorStateInfo(1).IsName("Shoot"))
            {
                break;
            }
            yield return null;
        }
        float yRota = enemy.transform.eulerAngles.y;
        GameObject ori = gameObject.transform.GetChild(0).Find("Oriantation").gameObject;
        ori.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        while (true)
        {
            if (enemy.animator == null)
            {
                StopAllCoroutines();
                break;
            }

            ori.transform.LookAt(enemy.Player.transform);
            yRota = Mathf.MoveTowards(yRota, ori.transform.eulerAngles.y, Time.deltaTime * 150);
            transform.eulerAngles = new Vector3(0, yRota, 0);

            if (enemy.animator.GetCurrentAnimatorStateInfo(1).IsName("ShootEnd"))
            {
                break;
            }
            yield return null;
        }
        spawnAmmo(gunbarrel);
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


        if (bullet.TryGetComponent<NormalBulletFunction>(out NormalBulletFunction nbf))
        {
            nbf.baseAmmo = enemy.e_type.ammo;
            nbf.firedBy = enemy.gameObject;
        }

        GameObject go = GameObject.Instantiate(enemy.e_type.ammo.modelGO, bullet.transform);
        go.name = "Model";
        go.layer = 7;

        shotTimer = 0;

    }


    private void lookPlayer()
    {
        enemy.transform.LookAt(enemy.Player.transform);
        enemy.transform.eulerAngles = new Vector3(0, enemy.transform.eulerAngles.y, 0);
    }
}
