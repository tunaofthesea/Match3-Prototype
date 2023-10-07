using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AdjustCamera : MonoBehaviour
{
    public Camera targetCamera; // Assign your camera here in the inspector
    public int initialMatrixRows = 8; // Your initial matrix rows
    public float initialOrthoSize = 5; // Your initial orthographic size


    private void Start()
    {
        targetCamera = Camera.main;
        int rows = BoardGenerator.instance.rows;
        AdjustCameraSize(rows);
    }
    // Call this function whenever you want to adjust the camera size based on a new matrix dimension.
    public void AdjustCameraSize(int newMatrixRows)
    {
        targetCamera.orthographicSize = (initialOrthoSize * newMatrixRows) / initialMatrixRows;
    }

}
    
