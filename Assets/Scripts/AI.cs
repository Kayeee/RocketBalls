using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewMonoBehaviour : MonoBehaviour
{
    List<GameBall> balls;
    List<PlayerMovement> players;
    List<Goal> goals;

    GameBall activeBall;
    Goal activeGoal;

    AIState activeState = AIState.ScoreBall;

    enum AIState { ScoreBall, DefendGoal }

    // Use this for initialization
    void Start()
    {
        balls = new List<GameBall>(FindObjectsOfType<GameBall>());
        players = new List<PlayerMovement>(FindObjectsOfType<PlayerMovement>());
        goals = new List<Goal>(FindObjectsOfType<Goal>());

        activeBall = balls[Random.Range(0, balls.Count)];
        activeGoal = goals[Random.Range(0, goals.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        if (activeState == AIState.ScoreBall)
        {
            //Find goal position (line ball up with goal

            //if goal position is far away 
                //Move towards goal position

            //if goal position is within fire range
                //shoot at ball
        }
    }
}
