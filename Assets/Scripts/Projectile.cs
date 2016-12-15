﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField]
    AudioClip bounceSound;
    bool collisionLock = false;

    [SerializeField]
    float gravity = 100f;
    float gravityForce;

    int gravityUpdatesPerSecond = 1;
    float lastGravityUpdateTime = 0;


    [SerializeField]
    public PlayerMovement.Team team;

	// Use this for initialization
	void Start () {
        lastGravityUpdateTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        //if (Time.time - lastGravityUpdateTime > gravityUpdatesPerSecond)
        if (Random.Range(0,60) == 0)
        {
            lastGravityUpdateTime = Time.time;

            gravityForce = GetComponent<Rigidbody>().mass * gravity;

            foreach (Projectile projectile in FindObjectsOfType<Projectile>())
            {
                if (projectile.team == team)
                {
                    Vector3 direction = (transform.position - projectile.transform.position).normalized;
                    float distance = Vector3.Distance(transform.position, projectile.transform.position);
                    float power = 1 / distance * gravityForce;
                    Vector3 force = direction * power;
                    if (!float.IsNaN(force.x) && !float.IsNaN(force.y) && !float.IsNaN(force.z))
                    {
                        projectile.GetComponent<Rigidbody>().AddForce(force);
                    }
                }
            }
        }
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

        Projectile otherProjectile = col.gameObject.GetComponent<Projectile>();
        if (otherProjectile != null && otherProjectile.team == team)
        {
            if (otherProjectile.transform.localScale == transform.localScale)
            { 
                if (otherProjectile.collisionLock)
                {
                    otherProjectile.collisionLock = false;
                }
                else
                {
                    collisionLock = true;

                    CombineProjectiles(otherProjectile);
                }
            }
            else if (otherProjectile.transform.localScale.x < transform.localScale.x)
            {
                CombineProjectiles(otherProjectile);
            }
        }
    }

    void CombineProjectiles(Projectile otherProjectile)
    {
        float radius1 = transform.localScale.x / 2;
        float area1 = 4 * Mathf.PI * radius1 * radius1; //a = 4 * pi * r^2

        float radius2 = otherProjectile.transform.localScale.x / 2;
        float area2 = 4 * Mathf.PI * radius2 * radius2;

        float newArea = area1 + area2;
        float newRadius = Mathf.Sqrt(newArea / (4 * Mathf.PI)); //r = sqrt(a / (4 * pi))

        transform.localScale = Vector3.one * newRadius * 2;

        GetComponent<Rigidbody>().mass = transform.localScale.x;

        Destroy(otherProjectile.gameObject);
    }
}
