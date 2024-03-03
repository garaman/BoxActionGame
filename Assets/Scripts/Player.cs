using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;

    float hAxis;
    float vAxis;
    bool rDown;
    bool jDowm;

    bool isJump = false;
    bool isDodge = false;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator animator;
    Rigidbody rigidbody;
        
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }
        
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        rDown = Input.GetButton("Run"); // left shift
        jDowm = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge) { moveVec = dodgeVec; }

        transform.position += moveVec * speed * (rDown ? 1.8f : 1f) * Time.deltaTime;

        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isRun", rDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if(jDowm && !isJump && moveVec==Vector3.zero && !isDodge)
        {
            rigidbody.AddForce(Vector3.up * 20 , ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (jDowm && !isDodge && !isJump && moveVec != Vector3.zero)
        {
            speed *= 2f;
            dodgeVec = moveVec;
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.3f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }
}
