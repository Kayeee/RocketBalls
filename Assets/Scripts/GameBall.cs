using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBall : MonoBehaviour {

    [SerializeField]
    GameObject celebrationPrefab;

    [SerializeField]
    GameObject bounds;

    Goal[] goals;

    AudioSource ballSounds;

	// Use this for initialization
	void Start ()
    {
        ballSounds = GetComponent<AudioSource>();
        goals = FindObjectsOfType<Goal>();
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
                Bounds ballBounds = bounds.GetComponent<MeshRenderer>().bounds;

                bool ballInGoal = goalBounds.Contains(ballBounds.min) && goalBounds.Contains(ballBounds.max);

                if (ballInGoal)
                {
                    Goal();

                    goal.score += (int)transform.localScale.x;

                    goal.scoreText.text = goal.score.ToString();

                    Debug.Log("Score: " + goal.score);

                    if (goal.team == PlayerMovement.Team.Blue)
                    {
                        FindObjectOfType<Level>().blueScore = goal.score;
                    }
                    else if (goal.team == PlayerMovement.Team.Blue)
                    {
                        FindObjectOfType<Level>().redScore = goal.score;
                    }
                }
                else
                {
                    float amountIn = 0;

                    float distance_min = Mathf.Sqrt(goalBounds.SqrDistance(ballBounds.min));
                    float distance_max = Mathf.Sqrt(goalBounds.SqrDistance(ballBounds.max));

                    if (distance_min == 0 || distance_max == 0) //Ball is intersecting goal
                    {
                        amountIn = (distance_min + distance_max) / ballBounds.size.x;

                        if (!ballSounds.isPlaying)
                        {
                            ballSounds.Play();
                        }

                        Debug.Log("amountIn: " + amountIn);

                        ballSounds.pitch = (1 - amountIn) * 2 + 1;
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
