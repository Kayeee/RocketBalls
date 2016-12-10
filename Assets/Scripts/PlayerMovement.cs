using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField]
    GameObject projectileOrigin;

    [SerializeField]
    Rigidbody projectilePrefab;

    [SerializeField]
    float projectileForce = 1;

    [SerializeField]
    Camera playerCamera;

    [SerializeField]
    float jumpForce = 1;

    [SerializeField]
    float moveForce = 1;

    [SerializeField]
    float maxMoveSpeed = 100;

    [SerializeField]
    float lookSensitivity = .5f;


    bool grounded = false;
    float lastJumpTime;

    float horizontal;
    float vertical;
    float jump;
    float crouch;



    Vector2 prevoiousMousePosition;
    Vector2 currentMousePosition;
    Vector2 deltaMousePosition;

    Rigidbody body;

    // Use this for initialization
    void Start () {
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        body = GetComponent<Rigidbody>();

    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        Debug.Log("The color is blue");
    }

    // Update is called once per frame
    void Update ()
    {
        Debug.Log(isLocalPlayer);
        if (!isLocalPlayer)
        {
            
            GetComponentInChildren<Camera>().enabled = false;
            return;
        }
        else
        {
            GetComponentInChildren<Camera>().enabled = true;
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 normalizeAxis = new Vector3(horizontal, 0, vertical).normalized;

        //transform.Translate(playerCamera.transform.rotation * new Vector3(horizontal, 0, vertical), Space.Self);
        body.AddForce(playerCamera.transform.rotation * new Vector3(normalizeAxis.x, 0, 0) * moveForce);

        Quaternion verticalLook = Quaternion.AngleAxis(playerCamera.transform.rotation.eulerAngles.y, Vector3.up);
        body.AddForce(verticalLook * new Vector3(0, 0, normalizeAxis.z) * moveForce);

        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        grounded = Physics.Raycast(new Ray(transform.position, Vector3.down), collider.height / 2);
        
        if (grounded && Time.time - lastJumpTime > .5)
        {
            jump = Input.GetAxis("Jump");
        }
        else
        {
            jump = 0;
        }

        if (jump > 0)
        {
            lastJumpTime = Time.time;
        }
        
        body.AddForce(Vector3.up * jump * jumpForce * -Physics.gravity.y);

        if (body.velocity.magnitude > maxMoveSpeed)
        {
            body.velocity = body.velocity.normalized * maxMoveSpeed;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouch = 1;
        }
        else
        {
            crouch = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Rigidbody tempProjectile = Instantiate<Rigidbody>(projectilePrefab, projectileOrigin.transform.position, Quaternion.identity);
            tempProjectile.AddForce(playerCamera.transform.rotation * Vector3.forward * projectileForce);
            tempProjectile.AddForce(body.velocity);
        }

        //currentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        deltaMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
        playerCamera.transform.Rotate(new Vector3(0, deltaMousePosition.x * lookSensitivity, 0), Space.World);
        playerCamera.transform.Rotate(new Vector3(-deltaMousePosition.y * lookSensitivity, 0, 0), Space.Self);


        prevoiousMousePosition = currentMousePosition;
    }
}
