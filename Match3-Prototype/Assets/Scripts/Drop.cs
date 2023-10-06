using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public GameObject[] sprites;
    public GameObject selectedSpriteObject;
    public int ColorIndex;
    public Vector3 targetPosition;
    public int dropX, dropY;

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
        for (int i = 0; i < sprites.Length; i++)
        {
            if(i == index)
            {
                ColorIndex = index;
                sprites[i].SetActive(true);
                break;
            }
        }
    }

    public void MoveTowardsTarget(Vector2 target)
    {
        StartCoroutine(MoveTowardsTarget_cor(target));
    }

    IEnumerator MoveTowardsTarget_cor(Vector2 targetPosition)
    {
        while(true)
        {
            
            yield return null;
        }
    }

    public void ScaleDownAnimationEndEvent()
    {

    }


        
        
        
        

}
