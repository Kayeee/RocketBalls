using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Level : MonoBehaviour {

    public bool gameStarted = false;
    public GameObject blueWinText;
    public GameObject redWinText;
    public GameObject tieText;
    public int blueScore;
    public int redScore;
    public float timeScale = 1.0f;
    
    // Use this for initialization
    void Start () {
        blueWinText.SetActive(false);
        redWinText.SetActive(false);
        tieText.SetActive(false);
    }

    IEnumerator StartNewGame()
    {
        yield return new WaitForSeconds(3);

        FindObjectsOfType<NetworkManager>()[0].StopHost();

        if (gameStarted)
        {
            ReloadScene();
            //gameStarted = false;
        }
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("timeScale: " + timeScale);

        if (timeScale > 0)
        {
            Time.timeScale = timeScale;
        }

        if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.Equals))
        {
            Time.timeScale += .1f;
        }
        else if (Input.GetKeyUp(KeyCode.Minus))
        {
            Time.timeScale -= .1f;
        }

        if (FindObjectsOfType<GameBall>().Length == 0)
        {
            if (blueScore > redScore)
            {
                blueWinText.SetActive(true);
            }
            else if (redScore > blueScore)
            {
                redWinText.SetActive(true);
            }
            else 
            {
                tieText.SetActive(true);
            }

            StartCoroutine(StartNewGame());

            //FindObjectsOfType<NetworkManager>()[0].StopHost();

            //if (gameStarted)
            //{
            //    ReloadScene();
            //    gameStarted = false;
            //}
        }
        else 
        {
            gameStarted = true;
        }
        
        if (Input.GetKey(KeyCode.F5))
        {
            StartCoroutine(StartNewGame());
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
