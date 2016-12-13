using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    Transform[] spawnLocations;

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
    public float maxMoveSpeed = 100;

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
        Cursor.lockState = CursorLockMode.Locked;
        body = GetComponent<Rigidbody>();
        int randSpawnIndex = UnityEngine.Random.Range(0, spawnLocations.Length);

        Transform spawnLocation = spawnLocations[randSpawnIndex];
        transform.position = spawnLocation.position;
        transform.rotation = spawnLocation.rotation;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    // Update is called once per frame
    void Update ()
    {
        Debug.Log("isLocalPlayer: " + isLocalPlayer);

        if (!isLocalPlayer)
        {   
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            return;
        }
        else
        {
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 normalizeAxis = new Vector3(horizontal, 0, vertical).normalized;


        if (normalizeAxis.magnitude > 0)
        {
            GetComponent<CapsuleCollider>().material.dynamicFriction = 0;
            GetComponent<CapsuleCollider>().material.staticFriction = 0;
        }
        else
        {
            GetComponent<CapsuleCollider>().material.dynamicFriction = 40;
            GetComponent<CapsuleCollider>().material.staticFriction = 40;
        }

        float adjustedMoveForce = moveForce;

        if (!grounded)
        {
            adjustedMoveForce = moveForce / 10;
        }

        body.AddForce(playerCamera.transform.rotation * new Vector3(normalizeAxis.x, 0, 0) * adjustedMoveForce);

        Quaternion verticalLook = Quaternion.AngleAxis(playerCamera.transform.rotation.eulerAngles.y, Vector3.up);
        body.AddForce(verticalLook * new Vector3(0, 0, normalizeAxis.z) * adjustedMoveForce);

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
            Cursor.lockState = CursorLockMode.Locked;

            //Rigidbody tempProjectile = Instantiate<Rigidbody>(projectilePrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
            //tempProjectile.AddForce(playerCamera.transform.rotation * Vector3.forward * projectileForce);
            //tempProjectile.AddForce(body.velocity);
            //NetworkServer.Spawn(tempProjectile.gameObject);
            Cmd_Shoot();
        }

        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;

            //Rigidbody tempProjectile = Instantiate<Rigidbody>(projectilePrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
            //tempProjectile.AddForce(playerCamera.transform.rotation * Vector3.forward * projectileForce);
            //tempProjectile.AddForce(body.velocity);
            //NetworkServer.Spawn(tempProjectile.gameObject);
            //Cmd_ShootSmall();
        }

        //currentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        deltaMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
        playerCamera.transform.Rotate(new Vector3(0, deltaMousePosition.x * lookSensitivity, 0), Space.World);
        playerCamera.transform.Rotate(new Vector3(-deltaMousePosition.y * lookSensitivity, 0, 0), Space.Self);


        prevoiousMousePosition = currentMousePosition;
    }

    [Command]
    public void Cmd_Shoot()
    {
        Rigidbody tempProjectile = Instantiate<Rigidbody>(projectilePrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
        tempProjectile.AddForce(playerCamera.transform.rotation * Vector3.forward * projectileForce);
        tempProjectile.AddForce(body.velocity);
        NetworkServer.Spawn(tempProjectile.gameObject);
    }

    [Command]
    public void Cmd_ShootSmall()
    {
        Rigidbody tempProjectile = Instantiate<Rigidbody>(projectilePrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
        tempProjectile.transform.localScale = Vector3.one / 2;
        tempProjectile.GetComponent<Rigidbody>().mass = .5f;
        tempProjectile.AddForce(playerCamera.transform.rotation * Vector3.forward * projectileForce);
        tempProjectile.AddForce(body.velocity);
        NetworkServer.Spawn(tempProjectile.gameObject);
    }
}
