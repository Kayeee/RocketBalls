using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCamera : NetworkBehaviour
{

	// Use this for initialization
	void Start () {
        if (isLocalPlayer)
        {
            Debug.Log("Not local player");
            GetComponent<Camera>().enabled = true;
        }
        else
        {
            
            GetComponent<Camera>().enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
