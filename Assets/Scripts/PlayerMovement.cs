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
    float gravityMultiplier;
    [SerializeField]
    ProgressBar shotProgressBar;
    [SerializeField]
    ProgressBar runProgressBar;
    float maxRunMultiplier = 1.5f;
    float maxTimeRunning = 2;
    float timeRunning = 0;

    Rigidbody playerRigidbody;

    [SerializeField]
    float standardProjectileForce = 3000;
    float minProjectileForce = 500;
    float maxProjectileForce = 8000;

    [SerializeField]
    Camera playerCamera;

    [SerializeField]
    float jumpForce = 1;
    [SerializeField]
    float airJumps = 1;
    float airJumpCount = 0;

    bool shoot = false;
    float lastShootTime;
    float minTimeBetweenShots = .2f;
    bool powerShotStarted;
    float powerShotStartTime;
    float powerShotTimeToCharge = 1;

    [SerializeField]
    float moveForce = 1;

    [SerializeField]
    public float maxMoveSpeed = 100;

    [SerializeField]
    float lookSensitivity = .5f;
    
    bool grounded = false;
    float lastJumpTime;

    float keyboard_special;
    float keyboard_special_previous;
    float keyboard_horizontal_move;
    float keyboard_vertical_move;
    bool keyboard_shoot_previous;
    bool keyboard_shoot;
    bool keyboard_shoot_alt;
    float jump;
    float crouch;
    
    float jumpInput;
    float previousJumpInput;

    Level level;
    
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
    public enum LocalPlayer { None, P1, P2, P3, P4 };

    // Use this for initialization
    void Start () {
        level = FindObjectOfType<Level>();

        playerRigidbody = GetComponent<Rigidbody>();

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
            float randOffset = UnityEngine.Random.Range(-5, 5);
            transform.localPosition = transform.localPosition + new Vector3(randOffset, 0, -randOffset);
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
            float randOffset = UnityEngine.Random.Range(-5, 5);
            transform.localPosition = transform.localPosition + new Vector3(randOffset, 0, -randOffset);
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
        if (level.gameStarted)
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

            //if (localPlayer == LocalPlayer.P1)
            {
                ControlsKeyboard();
            }
            //ControllsController();

            playerRigidbody.AddForce(Physics.gravity * gravityMultiplier * playerRigidbody.mass);
        }
    }

    void ControlsKeyboard()
    {
        Pause pause = FindObjectOfType<Pause>();
        if (pause == null || !pause.IsPaused)
        {

            keyboard_horizontal_move = Input.GetAxis("Horizontal");
            keyboard_vertical_move = Input.GetAxis("Vertical");
            float keyboard_horizontal_look = Input.GetAxis("Mouse X");
            float keyboard_vertical_look = Input.GetAxis("Mouse Y");
            keyboard_shoot_previous = keyboard_shoot;
            keyboard_shoot = Input.GetMouseButton(0);
            keyboard_shoot_alt = Input.GetMouseButtonDown(1);
            previousJumpInput = jumpInput;
            jumpInput = Input.GetAxis("Jump");
            keyboard_special_previous = keyboard_special;
            keyboard_special = Input.GetAxis("Special");

            //Debug.Log(
            //" horz mv: " + keyboard_horizontal_move +
            //", vert mv: " + keyboard_vertical_move +
            //", horz lk: " + keyboard_horizontal_look +
            //", very lk: " + keyboard_vertical_look +
            //", shoot: " + keyboard_shoot +
            //", jump: " + jumpInput
            //    );

            Vector3 normalizeAxis = new Vector3(keyboard_horizontal_move, 0, keyboard_vertical_move).normalized;


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

            float runMultiplier = 1;

            Debug.Log("keyboard_special: " + keyboard_special);

            if (keyboard_special > 0 && timeRunning < maxTimeRunning)
            {
                runMultiplier = maxRunMultiplier;

                if (keyboard_special_previous > 0)
                {
                    timeRunning += Time.deltaTime;
                }
            }
            else
            {
                timeRunning -= Time.deltaTime;

                if (timeRunning < 0)
                    timeRunning = 0;
            }

            float runTimeRatio = 1 - timeRunning / maxTimeRunning;
            runProgressBar.Ratio = runTimeRatio;

            float adjustedMoveForce = moveForce * runMultiplier;

            if (!grounded)
            {
                adjustedMoveForce = moveForce / 10;
            }

            body.AddForce(playerCamera.transform.rotation * new Vector3(normalizeAxis.x, 0, 0) * adjustedMoveForce);

            Quaternion verticalLook = Quaternion.AngleAxis(playerCamera.transform.rotation.eulerAngles.y, Vector3.up);
            body.AddForce(verticalLook * new Vector3(0, 0, normalizeAxis.z) * adjustedMoveForce);

            CapsuleCollider collider = GetComponent<CapsuleCollider>();

            grounded = Physics.Raycast(new Ray(transform.position, Vector3.down), collider.bounds.extents.y + .1f);

            //Ground Jump
            if (grounded && jumpInput != 0 && previousJumpInput == 0) //Time.time - lastJumpTime > .1
            {
                jump = jumpInput;
            }
            else
            {
                jump = 0;
            }

            //Air Jump
            if (!grounded && airJumpCount < airJumps && jumpInput != 0 && previousJumpInput == 0)
            {
                jump = jumpInput;
                airJumpCount++;
            }

            if (grounded)
            {
                airJumpCount = 0;
            }

            if (jump > 0)
            {
                lastJumpTime = Time.time;
            }

            body.AddForce(Vector3.up * jump * jumpForce * -Physics.gravity.y);

            previousJumpInput = Input.GetAxis("Jump");

            if (body.velocity.magnitude > maxMoveSpeed * runMultiplier)
            {
                body.velocity = body.velocity.normalized * maxMoveSpeed * runMultiplier;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                crouch = 1;
            }
            else
            {
                crouch = 0;
            }

            if (keyboard_shoot || keyboard_shoot_previous)
            {
                LockMouse();

                Shoot();
            }

            if (keyboard_shoot_alt)
            {
                LockMouse();

                ShootAlt();
            }

            //Shoot Cheat
            if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.Q))
            {
                ShootAlt();
            }

            deltaMousePosition = new Vector2(keyboard_horizontal_look, keyboard_vertical_look);

            playerCamera.transform.Rotate(new Vector3(0, deltaMousePosition.x * lookSensitivity, 0), Space.World);
            playerCamera.transform.Rotate(new Vector3(-deltaMousePosition.y * lookSensitivity, 0, 0), Space.Self);

            Vector3 look_rotation = playerCamera.transform.localEulerAngles;
            float vertical_look_rotation = look_rotation.x;
            //if (vertical_look_rotation > 180)
            //{
            //    vertical_look_rotation -= 360;
            //}
            //if (vertical_look_rotation < -180)
            //{
            //    vertical_look_rotation += 360;
            //}
            //Debug.Log("look_rotation: " + look_rotation);
            //if (vertical_look_rotation > 90 || vertical_look_rotation < -90)
            //{
            //    float polarity = Mathf.Abs(vertical_look_rotation) / vertical_look_rotation;
            //    Vector3 bounded_look_rotation = new Vector3(90 * polarity, look_rotation.y, look_rotation.z);
            //    //playerCamera.transform.rotation = Quaternion.Euler(bounded_look_rotation);
            //    playerCamera.transform.localEulerAngles = bounded_look_rotation;
            //}
        }
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnLockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void ControllsController()
    {
        String playerString = localPlayer.ToString();

        float controller_horizontal_move = Input.GetAxis(playerString + "_Horizontal_Move");
        float controller_vertical_move = Input.GetAxis(playerString + "_Vertical_Move");
        float controller_horizontal_look = Input.GetAxis(playerString + "_Horizontal_Look");
        float controller_vertical_look = Input.GetAxis(playerString + "_Vertical_Look");
        float controller_jump = Input.GetAxis(playerString + "_Jump");
        float controller_shoot = Input.GetAxis(playerString + "_Shoot");

        Vector3 normalizeAxis = new Vector3(controller_horizontal_move, 0, controller_vertical_move).normalized;
        
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
            jump = controller_jump;
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

        if (controller_shoot > 0)
        {
            if (!previousFireButtonState)
            {
                previousFireButtonState = true;

                Cmd_ShootAlt();
            }
        }
        else
        {
            previousFireButtonState = false;
        }

        deltaMousePosition = new Vector2(controller_horizontal_look, controller_vertical_look);
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
    public void Cmd_ShootAlt()
    {
        ShootAlt();
    }

    public void Shoot()
    {
        float chargeTime = 0;
        float powerRatio = 0;

        if (powerShotStarted)
        {
            chargeTime = Time.time - powerShotStartTime;
            powerRatio = chargeTime / powerShotTimeToCharge;

            if (powerRatio > 1)
            {
                //powerRatio = .9f;
                powerRatio = UnityEngine.Random.Range(.8f, 1f);
            }
            else if (powerRatio > .95f)
            {
                powerRatio = 1f;
            }
        }

        shotProgressBar.Ratio = powerRatio;

        if (!keyboard_shoot_previous && keyboard_shoot) //Shot started
        {
            powerShotStarted = true;
            powerShotStartTime = Time.time;
        }

        if (keyboard_shoot && keyboard_shoot_previous) //Shot Charging
        {

        }

        if (keyboard_shoot_previous && !keyboard_shoot) //Shot ended
        {
            powerShotStarted = false;

            if (Time.time - lastShootTime > minTimeBetweenShots)
            {
                float shotPower = minProjectileForce + (maxProjectileForce - minProjectileForce) * powerRatio;

                Rigidbody tempProjectile = Instantiate<Rigidbody>(projectilePrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
                tempProjectile.AddForce(playerCamera.transform.rotation * Vector3.forward * shotPower);
                tempProjectile.AddForce(body.velocity);
                NetworkServer.Spawn(tempProjectile.gameObject);

                lastShootTime = Time.time;
            }
        }
    }

    public void ShootAlt()
    {
        if (Time.time - lastShootTime > minTimeBetweenShots)
        {
            Rigidbody tempProjectile = Instantiate<Rigidbody>(projectilePrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
            tempProjectile.AddForce(playerCamera.transform.rotation * Vector3.forward * standardProjectileForce);
            tempProjectile.AddForce(body.velocity);
            NetworkServer.Spawn(tempProjectile.gameObject);

            lastShootTime = Time.time;
        }
    }
}
