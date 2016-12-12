using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBall : MonoBehaviour {

    [SerializeField]
    GameObject celebrationPrefab;
    [SerializeField]
    Goal[] goals;

	// Use this for initialization
	void Start ()
    {
	}

    private Type TypeOf(Goal goal)
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update ()
    {
		foreach (Goal goal in goals)
        {            
            if (Mathf.Abs(transform.position.x - goal.transform.position.x) < 25 &&
                Mathf.Abs(transform.position.z - goal.transform.position.z) < 25 &&
                Mathf.Abs(transform.position.y - goal.transform.position.y) < 25)
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
