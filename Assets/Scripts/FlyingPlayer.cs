using System.IO.MemoryMappedFiles;
using UnityEngine;

public class FlyingPlayer : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity;
    public float smoothSpeed;

    [Header("Flying")]
    public float flySpeed;
    public float bounceForce; // once on ground when flying, must bounce back a little to land
    public float speedMultiplier = 1.25f;

    [Header("Laser")]
    public LineRenderer rightLaser;
    public LineRenderer leftLaser;
    public float maxDistanceOfLaser = 100f;
    public Vector3 laserOffset; // play around with this
    public bool canShoot;


    float flyForwardFloat = 0f;

    Rigidbody rb;

    float playerHeight;

    public Camera cameraObj;

    public int perspective; // 1, 2

    public Vector3 thirdPersonFront;
    public Vector3 thirdPersonFrontRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        SphereCollider sphere = GetComponent<SphereCollider>();
        playerHeight = sphere.radius * transform.localScale.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rightLaser.positionCount = 2;
        leftLaser.positionCount = 2;

        perspective = 1;

        canShoot = true;
    }

    void Update()
    {
        RotateCamera();
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (perspective == 1) perspective = 2;
            else if (perspective == 2) perspective = 1;
            else { Debug.Log("What... how in the hel-"); }

            if (perspective == 1)
            {
                cameraObj.transform.localPosition = Vector3.zero;
                cameraObj.transform.localEulerAngles = Vector3.zero;
                canShoot = true;
            }
            else if (perspective == 2)
            {
                cameraObj.transform.localPosition = thirdPersonFront;
                cameraObj.transform.localEulerAngles = thirdPersonFrontRotation;
                canShoot = false;
            }
        }

        //Laser
        if (Input.GetMouseButton(0) && canShoot) // Hold left click
        {
            FireLaser();
        }
        else
        {
            rightLaser.enabled = false;
            leftLaser.enabled = false;
        }

        if (OnGround()) rb.AddForce(Vector3.up * flySpeed);
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(-mouseY * mouseSensitivity * Time.deltaTime, mouseX * mouseSensitivity * Time.deltaTime, 0);

        float rotate = 0f;

        if (Input.GetKey(KeyCode.A)) rotate = 1f;
        if (Input.GetKey(KeyCode.D)) rotate = -1f;

        transform.Rotate(0, 0, rotate * mouseSensitivity / 1.5f * Time.deltaTime);
    }

    void MovePlayer()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (flyForwardFloat == 0f) flyForwardFloat = 1f;
            else
            {
                if (flyForwardFloat <= 16f)
                {
                    flyForwardFloat *= speedMultiplier;
                }
            }
            Debug.Log(flyForwardFloat);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (flyForwardFloat <= 1f)
            {
                flyForwardFloat = 0f;
            }
            else flyForwardFloat /= speedMultiplier;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            flyForwardFloat = 0f;
        }

        rb.linearVelocity = transform.forward * flyForwardFloat * flySpeed;
    }

    void FireLaser()
    {
        Ray ray = cameraObj.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 endPoint;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceOfLaser))
        {
            endPoint = hit.point;
            //Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.transform.CompareTag("Enemy"))
                hit.collider.transform.parent.GetComponent<TheEnemy>().Die();
        }
        else
            endPoint = ray.origin + ray.direction * maxDistanceOfLaser;
        
        // hit.collider to get collider, then can use GetComponent<>() to, well, get components

        Vector3 rightStartPoint = cameraObj.transform.TransformPoint(laserOffset);
        Vector3 leftStartPoint = cameraObj.transform.TransformPoint(new Vector3(-laserOffset.x, laserOffset.y, laserOffset.z));

        rightLaser.SetPosition(0, rightStartPoint);
        leftLaser.SetPosition(0, leftStartPoint);
        rightLaser.SetPosition(1, endPoint);
        leftLaser.SetPosition(1, endPoint);

        rightLaser.enabled = true;
        leftLaser.enabled = true;
    }

    bool OnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f);
    }
}