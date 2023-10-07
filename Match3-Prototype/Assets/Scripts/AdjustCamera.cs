using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AdjustCamera : MonoBehaviour
{
    public int matrixRows;
    public int matrixColumns;

    private float initialRatio;
    private Camera mainCamera;

    void Start()
    {
        matrixRows = BoardGenerator.instance.rows;
        matrixColumns = BoardGenerator.instance.columns;

        mainCamera = Camera.main;
        initialRatio = Screen.width / Screen.height;
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float newRatio = Screen.width / Screen.height;

        float baseSize = (1.25f * matrixRows) / 2;  // My initial calculation was: Camera size 5 for a 4 x 8 dimension Matrice -> 2 * 5 / 8 = 1.25. It was really late, I couldnt derive the necessary formula.

        mainCamera.orthographicSize = baseSize * (initialRatio / newRatio);
    }
}
