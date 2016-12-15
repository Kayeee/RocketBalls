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
    Rigidbody projectilePrefabRed;
    [SerializeField]
    Rigidbody projectilePrefabBlue;
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

    private bool previousFireButtonState = true;

    [SerializeField]
    Team team = Team.None;
    public enum Team { None, Red, Blue };

    [SerializeField]
    Transform spawnRed;
    [SerializeField]
    Transform spawnBlue;

    [SerializeField]
    bool forceLocalPlayer = false;
    [SerializeField]
    LocalPlayer localPlayer = LocalPlayer.None;
    public enum LocalPlayer { None, P1, P2 };

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        body = GetComponent<Rigidbody>();
        
        if (team == Team.None)
        {
            int randTeamIndex = UnityEngine.Random.Range(0, 1);

            if (randTeamIndex == 0)
            {
                team = Team.Red;
            }
            else if (randTeamIndex == 1)
            {
                team = Team.Blue;
            }
        }

        if (team == Team.Red)
        {
            transform.position = spawnRed.position;
            transform.rotation = spawnRed.rotation;
            GetComponent<MeshRenderer>().material.color = Color.red;
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material.color = Color.red;
            }
            projectilePrefab = projectilePrefabRed;
        }
        else if (team == Team.Blue)
        {
            transform.position = spawnBlue.position;
            transform.rotation = spawnBlue.rotation;
            GetComponent<MeshRenderer>().material.color = Color.blue;
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material.color = Color.blue;
            }
            projectilePrefab = projectilePrefabBlue;
        }
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
        if (!forceLocalPlayer)
        {
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
        }

        if (localPlayer == LocalPlayer.P1)
        {
            ControlsP1();
        }
        else if (localPlayer == LocalPlayer.P2)
        {
            ControlsP2();
        }
    }

    void ControlsP1()
    {
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
            Cmd_Shoot();
        }

        if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.Q))
        {
            Cursor.lockState = CursorLockMode.Locked;

            Cmd_Shoot();
        }

        //currentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        deltaMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        playerCamera.transform.Rotate(new Vector3(0, deltaMousePosition.x * lookSensitivity, 0), Space.World);
        playerCamera.transform.Rotate(new Vector3(-deltaMousePosition.y * lookSensitivity, 0, 0), Space.Self);
    }

    void ControlsP2()
    {
        float p2_horizontal_move = Input.GetAxis("P2_Horizontal_Move");
        float p2_vertical_move = Input.GetAxis("P2_Vertical_Move");
        float p2_horizontal_look = Input.GetAxis("P2_Horizontal_Look");
        float p2_vertical_look = Input.GetAxis("P2_Vertical_Look");
        float p2_jump = Input.GetAxis("P2_Jump");
        float p2_shoot = Input.GetAxis("P2_Shoot");

        Vector3 normalizeAxis = new Vector3(p2_horizontal_move, 0, p2_vertical_move).normalized;
        
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
            jump = Input.GetAxis("P2_Jump");
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

        if (p2_shoot > 0)
        {
            if (!previousFireButtonState)
            {
                previousFireButtonState = true;

                Cmd_Shoot();
            }
        }
        else
        {
            previousFireButtonState = false;
        }


        //if (Input.GetMouseButton(1))
        //{
        //Cursor.lockState = CursorLockMode.Locked;

        //Rigidbody tempProjectile = Instantiate<Rigidbody>(projectilePrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
        //tempProjectile.AddForce(playerCamera.transform.rotation * Vector3.forward * projectileForce);
        //tempProjectile.AddForce(body.velocity);
        //NetworkServer.Spawn(tempProjectile.gameObject);
        //Cmd_ShootSmall();
        //}

        //currentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        //deltaMousePosition = new Vector2(Input.GetAxis("P2_Horizontal_Look"), Input.GetAxis("P2_Vertical_Look"));
        //playerCamera.transform.Rotate(new Vector3(0, deltaMousePosition.x * lookSensitivity, 0), Space.World);
        //playerCamera.transform.Rotate(new Vector3(-deltaMousePosition.y * lookSensitivity, 0, 0), Space.Self);

        deltaMousePosition = new Vector2(Input.GetAxis("P2_Horizontal_Look"), Input.GetAxis("P2_Vertical_Look"));
        playerCamera.transform.Rotate(new Vector3(0, deltaMousePosition.x * lookSensitivity, 0), Space.World);
        playerCamera.transform.Rotate(new Vector3(-deltaMousePosition.y * lookSensitivity, 0, 0), Space.Self);
    }

    //public void Shoot()
    //{
    //    Rigidbody tempProjectile = Instantiate<Rigidbody>(projectilePrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
    //    tempProjectile.AddForce(playerCamera.transform.rotation * Vector3.forward * projectileForce);
    //    tempProjectile.AddForce(body.velocity);
    //}

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
