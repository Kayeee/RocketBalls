using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBall : MonoBehaviour {

    [SerializeField]
    GameObject celebrationPrefab;
    [SerializeField]
    Goal[] goals;

    AudioSource ballSounds;

	// Use this for initialization
	void Start ()
    {
        ballSounds = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {
		foreach (Goal goal in goals)
        {            
            if (Mathf.Abs(transform.position.x - goal.transform.position.x) < 25 * 2 &&
                Mathf.Abs(transform.position.z - goal.transform.position.z) < 25 * 2 &&
                Mathf.Abs(transform.position.y - goal.transform.position.y) < 25 * 2)
            {
                Bounds goalBounds = goal.GetComponent<MeshRenderer>().bounds;
                Bounds ballBounds = GetComponent<MeshRenderer>().bounds;

                bool ballInGoal = goalBounds.Contains(ballBounds.min) && goalBounds.Contains(ballBounds.max);

                if (ballInGoal)
                {
                    Goal();

                    goal.score += (int)transform.localScale.x;

                    goal.scoreText.text = "Score: " + goal.score.ToString();

                    Debug.Log("Score: " + goal.score);
                }
                else
                {
                    float amountIn = 0;

                    float distance_min = goalBounds.SqrDistance(ballBounds.min);
                    float distance_max = goalBounds.SqrDistance(ballBounds.max);

                    if (distance_min == 0 || distance_max == 0) //Ball is intersecting goal
                    {
                        amountIn = (distance_min + distance_max) / goal.transform.localScale.x;

                        if (!ballSounds.isPlaying)
                        {
                            ballSounds.Play();
                        }

                        ballSounds.pitch = (1 - amountIn) * 2 + 1 ;
                    }
                    else
                    {
                        ballSounds.Stop();
                    }
                }
            }
        }
	}

    public void Goal()
    {
        GameObject celebrationTemp = Instantiate<GameObject>(celebrationPrefab, transform.position, Quaternion.identity);
        celebrationTemp.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }
}
