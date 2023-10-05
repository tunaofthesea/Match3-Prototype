using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public GameObject[] sprites;
    public GameObject selectedSpriteObject;
    public int ColorIndex;
    public Vector3 targetPosition;
    // the name is drop already, no need to make as dropX 
    public int x, y;

    private void Start()
    {
        int r = Random.Range(0, 4);
        sprites[r].SetActive(true);
        selectedSpriteObject = sprites[r];

        switch (r)  // Dont forget to turn this logic into board generation 
        {
            case 0:
                this.tag = "Red";
                break;
            case 1:
                this.tag = "Yellow";
                break;
            case 2:
                this.tag = "Blue";
                break;
            case 3:
                this.tag = "Green";
                break;
            // handle the cases where it is not in the cases
            default:
                this.tag = "Green";
                break;

        }
    }
    public void OnDisable()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].SetActive(false);
        }

    }

    public void ChooseSprite(int index)
    {
        if (index < 0 || index >= sprites.Length) return;
        ColorIndex = index;
        sprites[i].SetActive(true);
    }

    public void MoveTowardsTarget(Vector2 target)
    {
        StartCoroutine(MoveTowardsTarget_cor(target));
    }

    IEnumerator MoveTowardsTarget_cor(Vector2 targetPosition)
    {
        while (true)
        {

            yield return null;
        }
    }







}
