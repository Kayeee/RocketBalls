using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {

    bool gameStarted = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (FindObjectsOfType<GameBall>().Length == 0)
        {
            if (gameStarted)
            {
                ReloadScene();
            }
        }
        else 
        {
            gameStarted = true;
        }
        
        if (Input.GetKey(KeyCode.F5))
        {
            ReloadScene();
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
