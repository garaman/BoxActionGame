using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A,B,C };

    [SerializeField] Type type;
    [SerializeField] Transform target;
    [SerializeField] BoxCollider meleeArea;
    public int maxHealth;
    public int curHealth;

    bool isChase = false;
    bool isAttack = false;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material material;
    NavMeshAgent nav;
    Animator animator;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponentInChildren<MeshRenderer>().material;        
        animator = GetComponentInChildren<Animator>();

        nav = GetComponent<NavMeshAgent>();

        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    private void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FreezeVelocity()
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }        
    }

    void Targeting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        switch (type)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 10f;
                break;
            case Type.C:
                break;
        }



        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);

        switch(type)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                break;
        }


       
        animator.SetBool("isAttack", false);
        isAttack = false;
        isChase = true;
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        int health = curHealth;

        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;   
            Destroy(bullet);
        }

        if(health != curHealth)
        {
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
        }


        //Debug.Log($"{other.tag} : {curHealth}");
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }



    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade = false)
    {
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        reactVec = reactVec.normalized;

        if (curHealth > 0)
        {
            material.color= Color.white;
            if (isGrenade)
            {
                reactVec = reactVec * 5;
            }            
            rigid.AddForce(reactVec*2, ForceMode.Impulse);
        }
        else
        {
            material.color= Color.gray;
            gameObject.layer = 14;
            isChase = false;
            nav.enabled = false;
            animator.SetTrigger("doDie");

            if(isGrenade)
            {
                reactVec += Vector3.up*3;
                rigid.freezeRotation = false;
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec += Vector3.up;
            }

            rigid.AddForce(reactVec*5, ForceMode.Impulse);

            Destroy(gameObject, 4f);
        }
    }

}
