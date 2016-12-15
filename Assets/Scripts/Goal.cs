using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {

    public int score = 0;
    public Text scoreText;
    public Transform goalOrigin;
    public PlayerMovement.Team team;
    public bool dynamicScale = true;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (dynamicScale)
        {
            UpdateGoalSize();
        }
    }

    void UpdateGoalSize()
    {
        float largestProjectileScale = 1;

        foreach (Projectile projectile in FindObjectsOfType<Projectile>())
        {
            if (projectile.team != team && projectile.transform.localScale.x > largestProjectileScale)
            {
                largestProjectileScale = projectile.transform.localScale.x;
            }
        }

        if (largestProjectileScale < 3)
        {
            largestProjectileScale = 3;
        }

        goalOrigin.transform.localScale = Vector3.one * (largestProjectileScale + 1f);
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
