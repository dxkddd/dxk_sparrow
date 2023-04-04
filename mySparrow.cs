using Assets.dxk_Scirpt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mySparrow : MonoBehaviour,IPlayer
{
    private CharacterController controller;
    private Animator animator;
    public new Camera camera;
    
    //playerMovement
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float angleY;
    public float playerSpeed;
    public float jumpHeight;
    private int jumpCount = 1;
    public float gravityValue;
    float horizontal;
    float vertical;
    bool hasHorizontalInput;
    bool hasVerticalInput;
    bool isWalking;
    bool isHeightest=false;
    
    //playerAttack
    bool isAttacking;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        groundedPlayer = isGrounded();
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

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -0.5f;
            animator.SetBool("Fly", false);
        }

        Vector3 move = new Vector3(horizontal, 0, vertical);
        angleY =Camera.main.transform.rotation.eulerAngles.y;
        move = Quaternion.Euler(0, angleY, 0) * move;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move.normalized;
        }

        //·É
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            animator.SetBool("Fly", true);
        }
        if (animator.GetBool("Fly"))
        {
            if (Input.GetButtonDown("Jump") && jumpCount <= 3)
            {
                if ((isHeightest==false) && jumpCount==3)
                {
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -1.0f * gravityValue);
                    isHeightest= true;
                }
                else
                {
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -1.0f * gravityValue);
                }
                animator.SetBool("Fly", true);
                jumpCount++;
            }
            else if (Input.GetButtonDown("Jump") && isHeightest==true)
            {
                gravityValue = (float)0;
                playerVelocity.y -= 0.2f;
            }
            else if(Input.GetButtonUp("Jump") && isHeightest==true)
            {
                gravityValue= (float)-9.81;
            }
            if(jumpCount>3&&isGrounded()){
                jumpCount = 1;
                gravityValue = (float)-9.81;
                isHeightest= false;
            }
            
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    private void playerAttack() {
        isAttacking = isAttacked();
        animator.SetBool("isAttacking", isAttacking);
    }
    private bool isGrounded()
    {
        if (Physics.Raycast(this.transform.position, -Vector3.up, 0.1f))
        {
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
