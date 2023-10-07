using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public Scrollbar rowBar;
    public Scrollbar columnBar;

    public TextMeshProUGUI rowNumberText;
    public TextMeshProUGUI columnNumberText;

    public Button GenerateButton;

    public Canvas gameCanvas;

    public void ScrollValueChange(Scrollbar scrollBar)
    {
        int step = Mathf.FloorToInt(scrollBar.numberOfSteps * scrollBar.value);
        step = (step == scrollBar.numberOfSteps) ? step : step + 1;
        scrollBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = step.ToString();
    }

   int DimensionInt(TextMeshProUGUI text)
    {
        return Convert.ToInt32(text.text);
    }

    public void GenerateMatrice()
    {
        BoardGenerator bg = BoardGenerator.instance;
        bg.rows = DimensionInt(rowNumberText);
        bg.columns = DimensionInt(columnNumberText);
        bg.InitializeGame();

        Camera.main.GetComponent<AdjustCamera>().SetAgain();

        DisableElements();
    }

    public void DisableElements()
    {
        gameCanvas.gameObject.SetActive(false);
    }
}
