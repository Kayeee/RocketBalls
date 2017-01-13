using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworking : MonoBehaviour {

    public NetworkManager manager;

    // Use this for initialization
    void Start ()
    {
        manager = GetComponent<NetworkManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
        {
            //Debug.Log("Starting Server!");
            manager.StartHost();
        }
        else
        {
            manager.StopMatchMaker();
        }
    }
}
