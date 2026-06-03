using UnityEngine;

public class Player : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float maxDistance = 100f;

    Rigidbody rb;

    public bool isFlying = false;

    public float moveSpeed;

    public float maxSlopeAngle;
    RaycastHit slopeHit;

    float playerHeight;

    public float maxSpeed;

    public GameObject camera;

    public Vector3 offsetLaser;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        playerHeight = capsule.height * transform.localScale.y;
    }

    void Update()
    {
        OnSlope();

        // 1. Get mouse inputs
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 2. Rotate the player body left/right (Updates transform.forward correctly)
        transform.Rotate(Vector3.up * mouseX);

        // 3. Rotate only the camera up/down (Does not mess up player movement)
        camera.transform.Rotate(Vector3.left * mouseY);

        //Movement
        if (!isFlying)
        {
            rb.useGravity = true;

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }



            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(GetSlopeMoveDirection(transform.forward) * moveSpeed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(GetSlopeMoveDirection(transform.right) * -moveSpeed);
            }
            if (Input.GetKey(KeyCode.S)) 
            {
                rb.AddForce(GetSlopeMoveDirection(transform.forward) * -moveSpeed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(GetSlopeMoveDirection(transform.right) * moveSpeed);
            }
        }
        else
        {
            rb.useGravity = false;
        }
        



        //Laser
        if (Input.GetMouseButton(0)) // Hold left click
        {
            FireLaser();
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void FireLaser()
    {
        lineRenderer.enabled = true;

        Vector3 startPos = transform.position + offsetLaser;
        Vector3 direction = transform.forward;

        lineRenderer.SetPosition(0, startPos);

        RaycastHit hit;

        if (Physics.Raycast(startPos, direction, out hit, maxDistance))
        {
            lineRenderer.SetPosition(1, hit.point);

            Debug.Log("Hit: " + hit.collider.name);

            // Example damage:
            // hit.collider.GetComponent<Enemy>()?.TakeDamage(10);
        }
        else
        {
            lineRenderer.SetPosition(1, startPos + direction * maxDistance);
        }
    }

    // Im always going to be on a slope dumbass
    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle; // && angle != 0.0f;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}