using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    [SerializeField] GameObject missile;
    [SerializeField] Transform missilePortA;
    [SerializeField] Transform missilePortB;

    Vector3 lookVec;
    Vector3 tauntVec;

    bool isLook;
        
    void Awake()
    {
        Init();
        nav.isStopped = true;
        StartCoroutine(Thick());
    }
        
    void Update()
    {
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }

        if(isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0,v) * 5f;

            transform.LookAt(target.position + lookVec);
        }
        else
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Thick()
    {
        yield return new WaitForSeconds(0.1f);        

        int randAction = Random.Range(0, 5);

        switch(randAction)
        {
            case 0:               
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:                
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        animator.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject missileInstA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossBullet bossMissileA = missileInstA.GetComponent<BossBullet>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject missileInstB = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossBullet bossMissileB = missileInstB.GetComponent<BossBullet>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Thick());
    }

    IEnumerator RockShot()
    {
        isLook = false;
        animator.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Thick());
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;
        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        animator.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Thick());
    }
}
