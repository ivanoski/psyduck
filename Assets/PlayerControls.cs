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

    public GameObject frogProjectionPrefab;
    public float numberOfProjections = 5;
    public float timeBetweenProjections = 0.3f;
    private List<GameObject> frogProjectionPrefabs = new List<GameObject>();

    public RotationFrogUI rotationFrogUI;

    public float chargingJumpSpeed = 0.2f;
    public float MaxJumpCharge = 10f;
    public float MinJumpCharge = 3f;
    private float totalJumpCharge = 3f;

    private float jumpRotation = 0f;


    private bool isGrounded;

    private bool charging = true;
    private bool reverseCharging = false;
    private bool doJump = false;

    private Vector2 mousePosition;

    private float DontLandYetTimer = 0f;
    public float DontLandYetTime = 0.2f;

    public float landedDistance = 0.2f;
    public float landingAngleDegrees = 30f;

    private bool gravity = true;
    public float gravitySize = 0.2f;

    private void Awake()
    {
        for(int x = 0; x < numberOfProjections; x++)
        {
            GameObject newPrefab = Instantiate(frogProjectionPrefab, new Vector3(-1000, -1000, -1000), Quaternion.identity);
            frogProjectionPrefabs.Add(newPrefab);
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private bool AreVectorsParallel(Vector2 vectorA, Vector2 vectorB)
    {
        float dotProduct = Vector2.Dot(vectorA.normalized, vectorB.normalized);
        float angle = Mathf.Acos(dotProduct);
        float angleDegrees = angle * Mathf.Rad2Deg;

        return Mathf.Abs(angleDegrees) <= landingAngleDegrees;
    }

    private Vector2 simulatePhysicsPos(Vector2 velocity, Vector2 position, float time)
    {
        Vector2 acceleration = new Vector2(0f, 0f);
        acceleration += Vector2.down * gravitySize * Time.deltaTime;
        Vector2 endPos = position + (velocity * time) + (0.5f * acceleration * time);
        return endPos;
    }
    private float simulatePhysicsRot(float angularVelocity, float time)
    {
        return angularVelocity * time;
    }

    public static void PredictPositionAndRotation(Rigidbody2D rb, Vector2 initialVelocity, float initialAngularVelocity, float timeInSeconds, out Vector2 predictedPosition, out float predictedRotation)
    {
        Vector2 initialPosition = rb.position;
        //Vector2 initialVelocity = rb.velocity;
        float gravity = Physics2D.gravity.y; // Get the y component of gravity

        // Calculate the predicted position after the given time
        predictedPosition = initialPosition + initialVelocity * timeInSeconds + 0.5f * Vector2.up * gravity * 5f * timeInSeconds * timeInSeconds;

        // Calculate the predicted rotation after the given time
        predictedRotation = rb.rotation + initialAngularVelocity * timeInSeconds;
    }

    private void projection(Vector2 ivelocity)
    {
        float angVelocity = rotationFrogUI.angularVelocity;

        int i = 0;
        foreach (GameObject m in frogProjectionPrefabs)
        {
            Vector2 predictedPosition;
            float predictedRotation;
            m.SetActive(true);
            PredictPositionAndRotation(rb, ivelocity, angVelocity, (i + 1) * timeBetweenProjections, out predictedPosition, out predictedRotation);
            m.transform.position = predictedPosition;
            m.transform.rotation = Quaternion.Euler(0f,0f,predictedRotation);
            i++;
        }
    }
    private void endProjection()
    {
        foreach(GameObject m in frogProjectionPrefabs)
        {
            m.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        //rb.position = simulatePhysicsPos(rb.velocity, rb.position, Time.deltaTime);
        //rb.rotation = simulatePhysicsRot(rb.angularVelocity, Time.deltaTime);
    }

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 up = new Vector2(transform.up.x, transform.up.y);
        Vector2 VectorToMousePos = mousePosition - new Vector2(transform.position.x, transform.position.y);
        //float moveInput = Input.GetAxis("Horizontal");
        //rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        /*if(gravity)
        {
            rb.velocity += Vector2.down * gravitySize * Time.deltaTime;
        }*/
        

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
            if (Input.GetMouseButton(1)) { charging = false; totalJumpCharge = MinJumpCharge; endProjection(); return; }
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                charging = true;

                if(reverseCharging)
                    totalJumpCharge -= chargingJumpSpeed;
                else
                    totalJumpCharge += chargingJumpSpeed;

                if (totalJumpCharge > MaxJumpCharge) reverseCharging = true;
                if (totalJumpCharge < MinJumpCharge) reverseCharging = false;
                chargeMeter.percentageFull = totalJumpCharge / MaxJumpCharge;

                Vector2 directionToClick = mousePosition - new Vector2(transform.position.x, transform.position.y);
                directionToClick.Normalize();
                projection(directionToClick * totalJumpCharge);

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
            //rb.AddForce(directionToClick * totalJumpCharge, ForceMode2D.Impulse);
            rb.velocity = directionToClick * totalJumpCharge;
            rb.angularVelocity = rotationFrogUI.angularVelocity;
            totalJumpCharge = MinJumpCharge;
            doJump = false;
            DontLandYetTimer = DontLandYetTime;
            rb.gravityScale = 5f;
            isGrounded = false;
            endProjection();
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

