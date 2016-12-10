using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {

    public int redScore = 0;
    public Text redScoreText;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<GameBall>() != null)
        {
            Destroy(col.gameObject);
            redScore += (int)col.gameObject.transform.localScale.x;

            redScoreText.text = "Score: " + redScore.ToString();

            Debug.Log("Score: " + redScore);
        }
    }
}
