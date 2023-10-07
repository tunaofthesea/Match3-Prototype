using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSInitializer : MonoBehaviour
{
    public int targetFrameRate;
    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
