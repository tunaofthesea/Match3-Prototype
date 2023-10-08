using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    private string[] dropTags = { "Red", "Yellow", "Green", "Blue" };
    public static MatchChecker instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void CheckMatches(GameObject[,] matrix) // Sadly, the improved matching method I've been working on didn't make it in time, but I want to update it as soon as I finish!
    {
        {
            bool foundMatch = false;

            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    GameObject currentGO = matrix[row, col];

                    int matchCount = CheckDirection(matrix, row, col, 0, 1);
                    if (matchCount >= 3)
                    {
                        foundMatch = true;
                        for (int i = 0; i < matchCount; i++)
                        {
                            ChangeTag(matrix[row, col + i]);
                        }
                    }

                    matchCount = CheckDirection(matrix, row, col, 1, 0);
                    if (matchCount >= 3)
                    {
                        foundMatch = true;
                        for (int i = 0; i < matchCount; i++)
                        {
                            ChangeTag(matrix[row + i, col]);
                        }
                    }
                }
            }

            if (foundMatch)
            {
                CheckMatches(matrix);
            }
        }
    }

    private int CheckDirection(GameObject[,] matrix, int startRow, int startCol, int rowStep, int colStep)
    {
        int matched = 1;
        int row = startRow + rowStep;
        int col = startCol + colStep;
        string initialTag = matrix[startRow, startCol].tag;

        while (row >= 0 && row < matrix.GetLength(0) && col >= 0 && col < matrix.GetLength(1) && matrix[row, col].tag == initialTag)
        {
            matched++;
            row += rowStep;
            col += colStep;
        }

        return matched;
    }

    private void ChangeTag(GameObject go)
    {
        string currentTag = go.tag;
        string newTag = GetNewTag(currentTag);
        go.tag = newTag;
        go.GetComponent<Drop>().RematchShape(go.tag);
    }

    private string GetNewTag(string currentTag)
    {
        List<string> otherTags = new List<string>(dropTags);
        otherTags.Remove(currentTag);
        int randomIndex = Random.Range(0, otherTags.Count);
        return otherTags[randomIndex];
    }
}
