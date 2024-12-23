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

    private IEnumerator Aroutine;
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
        moveTimer += Time.deltaTime;
        shotTimer += Time.deltaTime;

        Debug.DrawRay(enemy.gunBarrel[0].transform.position, (enemy.Player.transform.position - enemy.gunBarrel[0].transform.position) * (Vector3.Distance(enemy.gunBarrel[0].transform.position, enemy.Player.transform.position) + 2f));
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) > enemy.e_type.rangeDistance || Physics.Raycast(enemy.transform.position+new Vector3(0,4,0), enemy.Player.transform.position - enemy.gunBarrel[0].transform.position, Vector3.Distance(enemy.gunBarrel[0].transform.position, enemy.Player.transform.position) + 2f, enemy.e_type.LMask))
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
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) < enemy.e_type.rangeDistance && Physics.Raycast(enemy.gunBarrel[0].transform.position, enemy.Player.transform.position - enemy.gunBarrel[0].transform.position,out RaycastHit hit,Vector3.Distance(enemy.gunBarrel[0].transform.position, enemy.Player.transform.position) + 2f, enemy.e_type.LMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                if (shotTimer >= enemy.e_type.attackRatio)
                {
                    Shoot();
                }
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
        else//uçan olduðu için daha iyi gözükür
        {
            enemy.transform.LookAt(enemy.Player.transform);
        }
        Transform[] gunbarrel = enemy.gunBarrel.ToArray();

        for (int c = 0; c < gunbarrel.Length; c++)
        {
            if (Aroutine != null)
            {
                return;
            }

            shotTimer = 0;
            Aroutine = waitforAnimEnd(gunbarrel[c]);
            StartCoroutine(Aroutine);
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
                StopCoroutine(Aroutine);
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
                StopCoroutine(Aroutine);
                break;
            }

            ori.transform.LookAt(enemy.Player.transform);
            if (ori.transform.eulerAngles.y > 180)
            {
                ori.transform.eulerAngles = new Vector3(ori.transform.eulerAngles.x, ori.transform.eulerAngles.y - 360, ori.transform.eulerAngles.z);
            }
            yRota = Mathf.MoveTowards(yRota, ori.transform.eulerAngles.y, Time.deltaTime * 270);
            transform.eulerAngles = new Vector3(0, yRota, 0);

            if (enemy.animator.GetCurrentAnimatorStateInfo(1).IsName("ShootEnd"))
            {
                enemy.soundController.PlaySound("ShootSound", 0f);
                break;
            }

            yield return null;
        }
        Aroutine = null;
        spawnAmmo(gunbarrel);
    }

    private void spawnAmmo(Transform gunbarrel)
    {
        GameObject bullet = new();
        bullet.name = "bullet";

        bullet.transform.position = gunbarrel.transform.position + new Vector3(0, 0.5f, 0);
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
