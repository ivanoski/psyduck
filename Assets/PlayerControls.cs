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

    private float DontLandYetTimer = 0f;
    public float DontLandYetTime = 0.2f;

    public float landedDistance = 0.2f;
    public float landingAngleDegrees = 30f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private bool AreVectorsParallel(Vector2 vectorA, Vector2 vectorB)
    {
        float dotProduct = Vector2.Dot(vectorA.normalized, vectorB.normalized);
        float angle = Mathf.Acos(dotProduct);
        float angleDegrees = angle * Mathf.Rad2Deg;

        return Mathf.Abs(angleDegrees) <= landingAngleDegrees;
    }

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 up = new Vector2(transform.up.x, transform.up.y);
        Vector2 VectorToMousePos = mousePosition - new Vector2(transform.position.x, transform.position.y);
        //float moveInput = Input.GetAxis("Horizontal");
        //rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, -up, landedDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, -up, Color.red, 2f);
        //if (hit) Debug.Log(hit + "hit");
        //if (DontLandYetTimer <= 0f) Debug.Log("DontLandYetTimer");
        if (!isGrounded) Debug.Log("!isGrounded");


        if (hit && DontLandYetTimer <= 0f && !isGrounded)
        {
            Debug.Log(hit.normal + " " + up);
            if(AreVectorsParallel(hit.normal, up))
            {
                Debug.Log("got here!");
                isGrounded = true;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.gravityScale = 0f;
            }
        }

        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer) && (DontLandYetTimer <= 0f);

        if (DontLandYetTimer > 0f) DontLandYetTimer -= Time.deltaTime;

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
            rb.AddForce(directionToClick * totalJumpCharge, ForceMode2D.Impulse);
            rb.angularVelocity = rotationFrogUI.angularVelocity;
            totalJumpCharge = 0f;
            doJump = false;
            DontLandYetTimer = DontLandYetTime;
            rb.gravityScale = 5f;
            isGrounded = false;
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
