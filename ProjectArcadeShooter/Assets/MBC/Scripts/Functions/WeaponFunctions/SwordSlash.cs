using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    public Animator handAnimator;
    public GameObject slashEffect;
    public WeaponManager weaponManager;

    private bool first;

    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attack()
    {
        if (first)
        {
            first = false;
            secondAttack();
        }
        else
        {
            first = true;
            firstAttack();
        }
    }
    public void firstAttack()
    {
        slashEffect.SetActive(true);
        
    }
    IEnumerator animationRoutine()
    {
        handAnimator.SetBool("fired", true);
        yield return null;
        handAnimator.SetBool("fired", false);

        while (true)
        {
            yield return null;
            if (handAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack1_weap" + 2))
            {
                break;
            }
        }
        weaponManager.AttackEnded();
        yield return null;

        float timer = 0.5f;
        while (true)
        {
            yield return null;

            timer -= Time.deltaTime;

            if(timer < 0)
            {
                handAnimator.SetBool("attackInterruption", true);
                break;
            }
        }
        yield return null;

        handAnimator.SetBool("attackInterruption", false);

    }
    public void secondAttack()
    {

    }
}
