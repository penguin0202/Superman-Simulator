using UnityEngine;

public class FlyingPlayer : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity;
    public float smoothSpeed;

    [Header("Flying")]
    public float flySpeed;
    public float bounceForce; // once on ground when flying, must bounce back a little to land

    [Header("Laser")]
    public LineRenderer laser;
    public float maxDistanceOfLaser = 100f;
    public Vector3 laserOffset; // play around with this


    float flyForwardFloat = 0f;

    Rigidbody rb;

    float playerHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        SphereCollider sphere = GetComponent<SphereCollider>();
        playerHeight = sphere.radius * transform.localScale.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(-mouseY * mouseSensitivity * Time.deltaTime, mouseX * mouseSensitivity * Time.deltaTime, 0);

        /*if (Input.GetKey(KeyCode.W))
            rb.AddForce(transform.forward * flySpeed);
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(transform.right * flySpeed);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(transform.forward * -flySpeed);
        if (Input.GetKey(KeyCode.D))
            rb.AddForce(transform.right * -flySpeed);*/
        
        float right = 0f;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (flyForwardFloat == 0f) flyForwardFloat = 1f;
            else flyForwardFloat = 0f;
        }
            
        if (Input.GetKey(KeyCode.A))
            right = -1f;
        if (Input.GetKey(KeyCode.D))
            right = 1f;

        Vector3 move = transform.forward * flyForwardFloat + transform.right * right;
        rb.linearVelocity = move.normalized * flySpeed;
    

        //Laser
        if (Input.GetMouseButton(0)) // Hold left click
        {
            FireLaser();
        }
        else
        {
            laser.enabled = false;
        }

        if (OnGround()) rb.AddForce(Vector3.up * flySpeed);
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
            Debug.Log("Hit: " + hit.collider.name);
            // hit.collider to get collider, then can use GetComponent<>() to, well, get components
        }
        else
            laser.SetPosition(1, startPos + direction * maxDistanceOfLaser);
    }

    bool OnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f);
    }
}