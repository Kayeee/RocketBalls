using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField]
    AudioClip bounceSound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnCollisionEnter(Collision col)
    {
        GameBall gameBall = col.gameObject.GetComponent<GameBall>();
        if (gameBall != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = bounceSound;
            audioSource.volume = GetComponent<Rigidbody>().velocity.magnitude / 100;
            audioSource.Play();
        }
    }
}
