using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    [SerializeField]
    PlayerMovement playerMovement;

    Animator gunBobble;

	// Use this for initialization
	void Start () {
        gunBobble = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
	
	// Update is called once per frame
	void Update () {
        gunBobble.SetFloat("MoveSpeed", playerMovement.GetComponent<Rigidbody>().velocity.magnitude);
        Debug.Log("Velocity: " + playerMovement.GetComponent<Rigidbody>().velocity.magnitude);
	}
}
