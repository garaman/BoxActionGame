using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;

    float hAxis;
    float vAxis;
    bool isRun;

    Vector3 moveVec;

    Animator animator;
        
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
        
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        isRun = Input.GetButton("Run"); // left shift

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * (isRun ? 1.8f : 1f) *Time.deltaTime;

        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isRun", isRun);

        transform.LookAt(transform.position + moveVec);
    }
}
