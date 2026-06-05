using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity;
    public float smoothSpeed;

    [Header("Walking")]
    public Vector3 walkingCameraOffset; // third person
    public float walkSpeed;
    public float maxWalkingSpeed;
    public float maxSlopeAngle;
    RaycastHit groundHit;

    [Header("Flying")]
    public float initialUpForce; // when turn from walking to flying, up force
    public Vector3 flyingCameraOffset; // first person? (0, 0, 0)
    public float flySpeed;
    public float bounceForce; // once on ground when flying, must bounce back a little to land

    [Header("Laser")]
    public LineRenderer laser;
    public float maxDistanceOfLaser = 100f;
    public Vector3 laserOffset; // play around with this
    
    Rigidbody rb;

    public bool isFlying = false;

    float playerHeight;

    public Transform cameraT;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        playerHeight = capsule.height * transform.localScale.y;
    }

    float pitch;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(KeyCode.Space) && !isFlying)
        {
            isFlying = true;
            rb.AddForce(Vector3.up * initialUpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0, 0, 0),
                smoothSpeed * Time.deltaTime
            );

            isFlying = false;
        }

        OnSlope();

        //Movement
        if (!isFlying)
        {
            transform.Rotate(0, mouseX * mouseSensitivity * Time.deltaTime, 0);
            cameraT.position = transform.position + walkingCameraOffset;

            // camera itself do the y
            pitch -= mouseY * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -90f, 90f);
            cameraT.rotation = Quaternion.Euler(
                pitch,
                transform.eulerAngles.y,
                0
            );

            rb.useGravity = true;

            // limit walking speed (horizontal only though)
            Vector3 velocity = rb.linearVelocity;
            Vector3 horizontal = new Vector3(velocity.x, 0, velocity.z);
            horizontal = Vector3.ClampMagnitude(horizontal, maxWalkingSpeed);
            rb.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);

            if (Input.GetKey(KeyCode.W))
                rb.AddForce(GetSlopeMoveDirection(transform.forward) * walkSpeed);
            if (Input.GetKey(KeyCode.A))
                rb.AddForce(GetSlopeMoveDirection(transform.right) * -walkSpeed);
            if (Input.GetKey(KeyCode.S))
                rb.AddForce(GetSlopeMoveDirection(transform.forward) * -walkSpeed);
            if (Input.GetKey(KeyCode.D))
                rb.AddForce(GetSlopeMoveDirection(transform.right) * walkSpeed);
        }
        else // isFlying
        {
            if (OnGround()) // reset back to walking once touch ground when flying
            {
                isFlying = false;
                rb.AddForce(Vector3.up * bounceForce);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.Euler(0, 0, 0),
                    smoothSpeed * Time.deltaTime
                );

                rb.useGravity = true;
            }
            else // gonna fly lesgo!!
            {
                cameraT.position = transform.position + flyingCameraOffset; // but lerp this later

                transform.Rotate(
                    -mouseY * mouseSensitivity * Time.deltaTime, 
                    mouseX * mouseSensitivity * Time.deltaTime, 
                    0
                );

                cameraT.rotation = transform.rotation;

                rb.useGravity = false;

                float input = 0f;

                if (Input.GetKey(KeyCode.Space))
                    input = 1f;

                Vector3 move = transform.forward * input * flySpeed;

                rb.linearVelocity = new Vector3(
                    move.x,
                    rb.linearVelocity.y,
                    move.z
                );

            }
        }

        //Laser
        if (Input.GetMouseButton(0)) // Hold left click
        {
            FireLaser();
        }
        else
        {
            laser.enabled = false;
        }
    }

    void FireLaser()
    {
        laser.enabled = true;

        Vector3 startPos = transform.position + laserOffset;
        Vector3 direction = transform.forward;

        laser.SetPosition(0, startPos);

        RaycastHit hit;

        if (Physics.Raycast(startPos, direction, out hit, maxDistanceOfLaser))
        {
            laser.SetPosition(1, hit.point);
            //Debug.Log("Hit: " + hit.collider.name);
            // hit.collider to get collider, then can use GetComponent<>() to, well, get components
        }
        else
        {
            laser.SetPosition(1, startPos + direction * maxDistanceOfLaser);
        }
    }

    bool OnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, out groundHit, playerHeight * 0.5f + 0.3f);
    }

    // Im always going to be on a slope dumbass
    bool OnSlope()
    {
        if (OnGround()) {
            //groundHit already contains slopeHit data, as you jsut called OnGround, which modifies it
            float angle = Vector3.Angle(Vector3.up, groundHit.normal);
            return angle < maxSlopeAngle && angle != 0.0f;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
    {
        return Vector3.ProjectOnPlane(moveDirection, groundHit.normal).normalized;
    }
}