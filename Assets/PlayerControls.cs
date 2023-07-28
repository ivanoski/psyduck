using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public float chargingJumpSpeed = 0.2f;
    public float MaxJumpCharge = 10f;
    private float totalJumpCharge = 0f;


    private bool isGrounded;

    private bool charging = true;
    private bool doJump = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //float moveInput = Input.GetAxis("Horizontal");
        //rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        /*if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }*/
        if(isGrounded)
        {
            if (Input.GetMouseButton(0))
            {
                charging = true;
                totalJumpCharge += chargingJumpSpeed;
                if (totalJumpCharge > MaxJumpCharge) totalJumpCharge = MaxJumpCharge;
            }
            else if(charging == true)
            {
                doJump = true;
                charging = false;
            }

        }



        if (doJump && isGrounded)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         
            Vector2 directionToClick = mousePosition - new Vector2(transform.position.x, transform.position.y);
            directionToClick.Normalize();
            Debug.Log(directionToClick);
            Debug.Log(totalJumpCharge);
            rb.AddForce(directionToClick * totalJumpCharge, ForceMode2D.Impulse);
            totalJumpCharge = 0f;
            doJump = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Check if the player is grounded
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player leaves the ground
        isGrounded = false;
    }
}
