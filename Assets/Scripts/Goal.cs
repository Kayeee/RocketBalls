using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {

    public int score = 0;
    public Text scoreText;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    //void OnCollisionEnter(Collision col)
    //{
    //    GameBall gameBall = col.gameObject.GetComponent<GameBall>();
    //    if (gameBall != null)
    //    {
    //        gameBall.Goal();

    //        score += (int)col.gameObject.transform.localScale.x;

    //        scoreText.text = "Score: " + score.ToString();

    //        Debug.Log("Score: " + score);
    //    }
    //}
}
