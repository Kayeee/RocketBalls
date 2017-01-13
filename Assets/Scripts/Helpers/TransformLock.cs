using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLock : MonoBehaviour {

    public bool lockPosition = false;
    public bool lockRotation = false;
    public bool lockScale = false;

    public Vector3 positionLock;
    public Vector3 rotationLock;
    public Vector3 scaleLock;

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (lockPosition)
        {
            transform.position = positionLock;
        }

        if (lockRotation)
        {
            transform.rotation = Quaternion.Euler(rotationLock);
        }

        if (lockScale)
        {
            transform.localScale = scaleLock;
        }
    }
}
