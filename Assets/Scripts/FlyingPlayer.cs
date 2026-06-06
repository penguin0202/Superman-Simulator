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

    [Header("Laser")]
    public LineRenderer rightLaser;
    public LineRenderer leftLaser;
    public float maxDistanceOfLaser = 100f;
    public Vector3 laserOffset; // play around with this


    float flyForwardFloat = 0f;

    Rigidbody rb;

    float playerHeight;

    public Camera cameraObj;

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
    }

    void Update()
    {
        RotateCamera();
        MovePlayer();

        





        //Laser
        if (Input.GetMouseButton(0)) // Hold left click
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

        transform.Rotate(0, 0, rotate * mouseSensitivity / 2 * Time.deltaTime);
    }

    void MovePlayer()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (flyForwardFloat == 0f) flyForwardFloat = 1f;
            else flyForwardFloat = 0f;
        }

        Vector3 move = transform.forward * flyForwardFloat;
        rb.linearVelocity = move.normalized * flySpeed;
    }

    void FireLaser()
    {
        Ray ray = cameraObj.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 endPoint;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceOfLaser))
        {
            endPoint = hit.point;
            Debug.Log("Hit: " + hit.collider.name);
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