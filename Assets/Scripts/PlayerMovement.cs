using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    
    public CharacterController controller;
    public Animator animator;
    public WeaponSwitching weaponholder;
    
    //Speed and Height
    public float speed = 12f;
    public float walkSpeed = 12f;
    public float sprintSpeed = 30f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    //Ground check
    public Transform Groundcheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
        
    Vector3 velocity;
    Vector3 move;

    bool isGrounded;
    int indexWeapon;

    //Checking position
    private Vector3 lastUpdatePos = Vector3.zero;
    private Vector3 dist;
    private float currentSpeed;

    //public Transform weapon;
    //private Vector3 weaponOrigin;

    //private float movementCounter;
    //private float idleCounter;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        //weaponOrigin = weapon.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        checkAnimator();
        Movement();
        Animation();
        checkSpeed();
        
    }

    void  checkAnimator()
    {
        if (indexWeapon != weaponholder.selectedWeapon)
        {
            indexWeapon = weaponholder.selectedWeapon;
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Movement()
    {
        isGrounded = Physics.CheckSphere(Groundcheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //mouse
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Camera Move
        move = transform.right * x + transform.forward * z;

        //Player Movement
        controller.Move(move * speed * Time.deltaTime);

        //Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //Headbob
        /*
        if(x == 0 && z == 0)
        {
            HeadBob(idleCounter, 0.025f, 0.025f);
            idleCounter += Time.deltaTime;
        }
        else
        {
            HeadBob(movementCounter, 0.035f, 0.035f);
            movementCounter += Time.deltaTime * 3f;
        }
        */
    }

    void Animation()
    {   
        //movement Section
        if (currentSpeed != 0)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }
        
        //Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("Run", true);
            speed = sprintSpeed;
        }
        else
        {
            animator.SetBool("Run", false);
            speed = walkSpeed;
        }

    }

    void checkSpeed()
    {
        dist = transform.position - lastUpdatePos;
        currentSpeed = dist.magnitude / Time.deltaTime;
        lastUpdatePos = transform.position;
        //if (currentSpeed != 0) { Debug.Log(gameObject.name + " movement speed is:" + currentSpeed); }     
    }

    void HeadBob(float z, float x_Intensity, float y_Intensity)
    {
       // weapon.localPosition = new Vector3(Mathf.Sin(z) * x_Intensity, Mathf.Sin(z) * y_Intensity, weaponOrigin.z);
    }
}
