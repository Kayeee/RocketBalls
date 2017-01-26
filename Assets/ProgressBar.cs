using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

    [SerializeField]
    RectTransform background;
    [SerializeField]
    RectTransform foreground;
    [SerializeField]
    float ratio = 1;

    public float Ratio
    {
        get
        {
            return ratio;
        }

        set
        {
            ratio = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SetProgress(Ratio);
    }
    
    public void SetProgress(float percentage)
    {
        Vector3 newScale = foreground.localScale;
        newScale.x = percentage;
        foreground.localScale = newScale;
    }
}
