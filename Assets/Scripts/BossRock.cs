using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2f;
    float scaleValue = 0.1f;
    bool isShot;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPoworTime());
        StartCoroutine(GainPowor());
    }
        
    IEnumerator GainPoworTime()
    {
        yield return new WaitForSeconds(2.2f);
        isShot = true;
    }

    IEnumerator GainPowor()
    {
        while(!isShot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;

            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null;
        }
    }
}
