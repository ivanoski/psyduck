using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public Transform ChargeMeterPivot;
    public ChargeMeter chargeMeter;

    public RotationFrogUI rotationFrogUI;

    public float chargingJumpSpeed = 0.2f;
    public float MaxJumpCharge = 10f;
    private float totalJumpCharge = 0f;

    private float jumpRotation = 0f;


    private bool isGrounded;

    private bool charging = true;
    private bool doJump = false;

    private Vector2 mousePosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 VectorToMousePos = mousePosition - new Vector2(transform.position.x, transform.position.y);
        //float moveInput = Input.GetAxis("Horizontal");
        //rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        /*if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }*/

        if (isGrounded)
        {
            //rb.velocity = new Vector2(0f, 0f);
            //rb.angularVelocity = 0f;
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                charging = true;
                totalJumpCharge += chargingJumpSpeed;
                if (totalJumpCharge > MaxJumpCharge) totalJumpCharge = MaxJumpCharge;
                chargeMeter.percentageFull = totalJumpCharge / MaxJumpCharge;
            }
            else if(charging == true)
            {
                doJump = true;
                charging = false;
                chargeMeter.percentageFull = 0f;
            }
            
            
            float angle = Mathf.Atan2(VectorToMousePos.y, VectorToMousePos.x) * Mathf.Rad2Deg;
            ChargeMeterPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }



        if (doJump && isGrounded)
        {
            
            Vector2 directionToClick = mousePosition - new Vector2(transform.position.x, transform.position.y);
            directionToClick.Normalize();
            Debug.Log(directionToClick);
            Debug.Log(totalJumpCharge);
            rb.AddForce(directionToClick * totalJumpCharge, ForceMode2D.Impulse);
            rb.angularVelocity = rotationFrogUI.angularVelocity;
            totalJumpCharge = 0f;
            doJump = false;
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
