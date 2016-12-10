using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBall : MonoBehaviour {

    [SerializeField]
    GameObject celebrationPrefab;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Goal()
    {
        GameObject celebrationTemp = Instantiate<GameObject>(celebrationPrefab, transform.position, Quaternion.identity);
        celebrationTemp.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }
}
