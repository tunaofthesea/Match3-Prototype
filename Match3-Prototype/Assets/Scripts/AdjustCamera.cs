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

        int maxSize = (rows > columns) ? rows : columns;  // TErnary opreator checks rows and columns number and sets the camera distance according to the bigger value. Still needs some work, I can also check the base/new screen boundaries and resize it with another function. 

        AdjustCameraSize(maxSize);
    }
    public void AdjustCameraSize(int newMatrixRows)
    {
        targetCamera.orthographicSize = (initialOrthoSize * newMatrixRows) / initialMatrixRows;
        if(targetCamera.orthographicSize < initialOrthoSize)
        {
            targetCamera.orthographicSize = initialOrthoSize - 1;
        }
    }

}
    
