using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swag : MonoBehaviour {

    [SerializeField]
    private float dopeSwagger = 5;
    public float swagger = 1000000;

	// Use this for initialization
	void Start () {

        //dopeSwagger = GetComponent<Transform>().position.x;

        Vector3 newPosition = GetComponent<Transform>().position;
        newPosition.x = dopeSwagger;

        transform.position = newPosition;
	}
	
	// Update is called once per frame
	void Update ()
    {

    }
}
