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

    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        } 
    }

    /*void OnCollisionStay2D(Collision2D collision)
    {
        // Check if the player is grounded
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player leaves the ground
        isGrounded = false;
    }*/
}
