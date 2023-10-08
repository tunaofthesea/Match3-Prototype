using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Scrollbar rowBar;
    public Scrollbar columnBar;

    public TextMeshProUGUI rowNumberText;
    public TextMeshProUGUI columnNumberText;

    public Button GenerateButton;
    public GameObject ReloadButton;

    public GameObject[] objectsToDisable;
    public GameObject[] objectsToEnable;

    public GameObject Agave;

    public void ScrollValueChange(Scrollbar scrollBar)
    {
        int step = Mathf.FloorToInt(scrollBar.numberOfSteps * scrollBar.value);
        step = (step == scrollBar.numberOfSteps) ? step : step + 1;
        scrollBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = step.ToString();

        ResizeAgave(Convert.ToInt32(rowNumberText.text), Convert.ToInt32(columnNumberText.text));
    }

    public void ResizeAgave(float x, float y)
    {
        Agave.GetComponent<RectTransform>().localScale = (x * y / 32f) * Vector3.one;
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
        for (int i = 0; i < objectsToDisable.Length; i++)
        {
            objectsToDisable[i].SetActive(false);
        }

        for (int i = 0; i < objectsToEnable.Length; i++)
        {
            objectsToEnable[i].SetActive(true);
        }
    }

    public void ReloadTheScene()
    {
        ReloadButton.GetComponent<Button>().enabled = false;
        StartCoroutine(LateReload_cor());
    }

    IEnumerator LateReload_cor()
    {
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
