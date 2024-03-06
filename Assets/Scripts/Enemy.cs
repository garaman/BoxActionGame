using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A,B,C,D };

    [SerializeField] Type type;
    [SerializeField] protected Transform target;
    [SerializeField] protected BoxCollider meleeArea;
    [SerializeField] protected GameObject bullet;
    public int maxHealth;
    public int curHealth;

    bool isChase = false;
    bool isAttack = false;
    protected bool isDead = false;

    protected Rigidbody rigid;
    protected BoxCollider boxCollider;
    protected MeshRenderer[] meshs;
    protected NavMeshAgent nav;
    protected Animator animator;

    private void Awake()
    {
        Init();
        
    }

    protected void Init()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        animator = GetComponentInChildren<Animator>();

        nav = GetComponent<NavMeshAgent>();

        if (type != Type.D)
        {
            Invoke("ChaseStart", 2);
        }
    }

    void ChaseStart()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    private void Update()
    {
        if (nav.enabled && type != Type.D)
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
        if(!isDead && type != Type.D)
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
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }



            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            if(rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
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
                yield return new WaitForSeconds(0.5f);
                GameObject bulletInst = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody bulletRigid = bulletInst.GetComponent<Rigidbody>();
                bulletRigid.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
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
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        reactVec = reactVec.normalized;

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }
            if (isGrenade)
            {
                reactVec = reactVec * 5;
            }            
            rigid.AddForce(reactVec*2, ForceMode.Impulse);
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }
            gameObject.layer = 14;
            isChase = false;
            isDead = true;
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

            if(type != Type.D)
            {
                Destroy(gameObject, 4f);
            }            
        }
    }

}
