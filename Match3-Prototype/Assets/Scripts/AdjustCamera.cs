using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AdjustCamera : MonoBehaviour
{
    public Camera targetCamera; 
    public int initialMatrixRows = 8; 
    public float initialOrthoSize = 6; 


    private void Start()
    {
        targetCamera = Camera.main;
        int rows = BoardGenerator.instance.rows;
        AdjustCameraSize(rows);
    }

    public void SetAgain()
    {
        targetCamera = Camera.main;
        int rows = BoardGenerator.instance.rows;
        int columns = BoardGenerator.instance.columns;

        int maxSize = (rows > columns) ? rows : columns;

        AdjustCameraSize(maxSize);
    }
    // Call this function whenever you want to adjust the camera size based on a new matrix dimension.
    public void AdjustCameraSize(int newMatrixRows)
    {
        targetCamera.orthographicSize = (initialOrthoSize * newMatrixRows) / initialMatrixRows;
    }

}
    
