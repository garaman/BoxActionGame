using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    private void Start()
    {
        Destroy(gameObject, 30f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor" && !isRock)
        {
            Destroy(gameObject,3f);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall" && !isMelee)
        {
            Destroy(gameObject);
        }
    }

    
}
