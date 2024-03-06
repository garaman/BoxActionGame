using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBullet : Bullet
{
    [SerializeField] public Transform target;
    NavMeshAgent nav;
        
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();    
    }
        
    void Update()
    {
        nav.SetDestination(target.position);
    }
}
