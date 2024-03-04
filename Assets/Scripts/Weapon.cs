using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        Melee,
        Range
    }
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int currentAmmo;


    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    [SerializeField] Transform bulletPos;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletCasePos;
    [SerializeField] GameObject bulletCase;

    Coroutine attackCoroutin;

    public void Use()
    {
        if(type == Type.Melee)
        {
            if(attackCoroutin == null) 
            {
                attackCoroutin = StartCoroutine(Swing());
            }                        
        }
        else if (type == Type.Range && currentAmmo > 0)
        {
            currentAmmo--;
            StartCoroutine(Shot());
        }
    }

    IEnumerator Swing()
    {        
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
                
        attackCoroutin = null;
    }

    IEnumerator Shot()
    { 
        GameObject bulletInst = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = bulletInst.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        GameObject caseInst = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = caseInst.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up*Random.Range(2,3);

        caseRigid.AddForce(caseVec,ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
        
    }
}
