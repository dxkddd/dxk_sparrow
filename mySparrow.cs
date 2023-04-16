using Assets.dxk_Scirpt;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class mySparrow : MonoBehaviour, IPlayer
{
    private CharacterController controller;
    private Animator animator;
    public new Camera camera;

    //playerMovement
    private Vector3 playerVelocity;
    private bool onTheGround;
    private float angleY;
    public float playerSpeed;
    public float jumpHeight;
    public float gravityValue;
    private float horizontal;
    private float vertical;
    private bool hasHorizontalInput;
    private bool hasVerticalInput;
    private bool isWalking;
    public int power;
    private int powerReduceValue = 10;
    private bool isReducing = false;

    //playerAttack
    private bool isAttacking;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        onTheGround = isGrounded();
    }
    void Update()
    {
        playerMovement();
        playerAttack();
    }
    private void playerMovement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        isWalking = hasHorizontalInput || hasVerticalInput;
        animator.SetBool("isWalking", isWalking);

        if (onTheGround && playerVelocity.y < 0)
        {
            playerVelocity.y = -0.5f;
            animator.SetBool("Fly", false);
        }

        Vector3 move = new Vector3(horizontal, 0, vertical);
        angleY = Camera.main.transform.rotation.eulerAngles.y;
        move = Quaternion.Euler(0, angleY, 0) * move;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move.normalized;
        }

        Fly();

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    private void Fly()
    {
        if (Input.GetButtonDown("Jump") && onTheGround)
        {
            animator.SetBool("Fly", true);
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
        if (Input.GetButtonDown("Jump") && !onTheGround)
        {
            animator.SetBool("Fly", true);
            if (power > 0)
            {
                gravityValue = 0f;
                playerVelocity.y += 0.5f;
                if (!isReducing)
                {
                    InvokeRepeating("powerReduce", 1.0f, 1.0f);
                    isReducing = true;
                }
            }
        }
        if (Input.GetButtonUp("Jump"))
        {
            gravityValue = -9.81f;
        }
    }
    private void powerReduce()
    {
        Debug.Log("doit");
        power -= powerReduceValue;
        if (power <= 0) gravityValue=-9.81f;
    }
    private void playerAttack()
    {
        isAttacking = isAttacked();
        animator.SetBool("isAttacking", isAttacking);
    }
    private bool isGrounded()
    {
        if (Physics.Raycast(this.transform.position, -Vector3.up, 0.1f))
        {
            if (isReducing)
            {
                CancelInvoke("powerReduce");
                isReducing= false;
            }
            power = 100;
            gravityValue = (float)-9.81;
            animator.SetBool("Fly", false);
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool isAttacked()
    {
        if (Input.GetMouseButton(0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
