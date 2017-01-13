using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplitScreen : MonoBehaviour {

    GridLayoutGroup gridLayout;

    // Use this for initialization
    void Start ()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        int children = transform.childCount;
        int rows = children < 3 ? 1 : 2;
        int columns = (int)Mathf.Ceil(children / (float)rows);

        Debug.Log("Children: " + children + ", Rows: " + rows + ", Columns: " + columns);

        Vector2 newCellSize = new Vector2(Screen.width / columns, Screen.height / rows);

        gridLayout.cellSize = newCellSize;

        //float ratio = newCellSize.x / newCellSize.y;

        //foreach (RawImage rawImage in GetComponentsInChildren<RawImage>())
        //{
        //    if (ratio > 1)
        //    {
        //        rawImage.texture.width = (int)(1000 * ratio);
        //        rawImage.texture.height = 1000;
        //        //Debug.Log("width greter than height - Width: "+ rawImage.texture.width + ", height: " + rawImage.texture.height);
        //    }
        //    else
        //    {
        //        rawImage.texture.width = (int)(1000 * ratio);
        //        rawImage.texture.height = 1000;
        //        //Debug.Log("width greter than height - Width: " + rawImage.texture.width + ", height: " + rawImage.texture.height);
        //    }

        //    //rawImage.texture.width = (int)gridLayout.cellSize.x;  
        //    //rawImage.texture.height = (int)gridLayout.cellSize.y;

        //    //rawImage.texture = new RenderTexture((int)gridLayout.cellSize.x, (int)gridLayout.cellSize.y, 24);
        //}
    }
}
