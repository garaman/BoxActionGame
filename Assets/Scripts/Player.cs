using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Camera followCamera;

    [SerializeField] GameObject[] weapons;
    [SerializeField] public bool[] hasWeapon;
    [SerializeField] GameObject[] grenades;
    [SerializeField] GameObject granadePrefab;
    public int hasGrenades = 0;

    public int ammo = 0;
    public int coin = 0;
    public int score = 0;
    public int health = 100;

    public int maxAmmo = 999;
    public int maxCoin = 99999;
    public int maxHealth = 100;
    public int maxHasGrenades = 4;

    float hAxis;
    float vAxis;

    bool rDown;
    bool jDown;
    bool fDown;
    bool gDown;
    bool reDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
        

    bool isJump = false;
    bool isDodge = false;
    bool isSwap = false;
    bool isReload = false;
    bool isFireReady = true;
    bool isBorder = false;
    bool isDamage = false;
    bool isShop = false;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator animator;
    new Rigidbody rigidbody;
    MeshRenderer[] meshs;
    
    
    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay = 0;




    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }
        
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Grenade();
        Reload();
        Dodge();
        Interaction();
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        rDown = Input.GetButton("Run"); // left shift
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        reDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        

    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge) { moveVec = dodgeVec; }
        if(isSwap || !isFireReady || isReload) { moveVec = Vector3.zero; }
        
        if(!isBorder)
        {
            transform.position += moveVec * speed * (rDown ? 1.8f : 1f) * Time.deltaTime;
        }

        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isRun", rDown);
    }

    void Turn()
    {
        if(fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
        else
        {
            transform.LookAt(transform.position + moveVec);
        }
        
    }

    void Jump()
    {
        if(jDown && !isJump && moveVec==Vector3.zero && !isDodge && !isSwap)
        {
            rigidbody.AddForce(Vector3.up * 20 , ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if(equipWeapon == null) { return; }

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap && !isShop) 
        {
            equipWeapon.Use();
            animator.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0) { return; }

        if (gDown && !isReload && !isSwap)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;
                
                GameObject grenadeInst = Instantiate(granadePrefab, transform.position, transform.rotation);
                Rigidbody granadeRigid = grenadeInst.GetComponent<Rigidbody>();

                granadeRigid.AddForce(nextVec, ForceMode.Impulse);
                granadeRigid.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);

            }
        }
    }
    void Reload()
    {
        if (equipWeapon == null) { return; }
        if (equipWeapon.type == Weapon.Type.Melee) { return; }
        if (ammo == 0) { return; }

        if(reDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            animator.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2.0f);
        }

    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.currentAmmo = reAmmo;
        ammo -= reAmmo;

        isReload = false;
    }

    void Dodge()
    {
        if (jDown && !isDodge && !isJump && moveVec != Vector3.zero && !isSwap)
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

    void Swap()
    {
        if(sDown1 && (!hasWeapon[0] || equipWeaponIndex == 0)) { return; }
        if(sDown2 && (!hasWeapon[1] || equipWeaponIndex == 1)) { return; }
        if(sDown3 && (!hasWeapon[2] || equipWeaponIndex == 2)) { return; }

        int weaponIndex = -1;

        if(sDown1) weaponIndex = 0;
        if(sDown2) weaponIndex = 1;
        if(sDown3) weaponIndex = 2;

        if ( (sDown1 || sDown2 || sDown3) && !isJump && !isDodge )
        {
            if(equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            animator.SetTrigger("doSwap");
            isSwap = true;

            Invoke("SwapOut", 0.3f);
        }
    }

    void SwapOut()
    {        
        isSwap = false;
    }

    void Interaction()
    {
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }

        }
    }

    void FreezeRotation()
    {
        rigidbody.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward*5 , Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        { 
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if(ammo > maxAmmo) { ammo = maxAmmo; }
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin) { coin = maxCoin; }
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth) { health = maxHealth; }
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades >= maxHasGrenades) { hasGrenades = maxHasGrenades; }
                    break;
            }
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {            
            if(!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
                if(health <= 0) { health = 0; } 
                StartCoroutine(OnDamage());
            }

            if (other.GetComponent<Rigidbody>() != null)
            {
                Destroy(other.gameObject);
            }

        }

    }

    IEnumerator OnDamage()
    {
        isDamage = true;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(1.0f);
                
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }   


    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObject = null;
        }
        else if( other.tag == "Shop")
        {            
            Shop shop = other.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
}
