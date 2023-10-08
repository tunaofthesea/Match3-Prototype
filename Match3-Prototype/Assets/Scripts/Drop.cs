using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public GameObject[] sprites;
    public GameObject selectedSpriteObject;
    private Animator anim;
    public int ColorIndex;
    public Vector3 targetPosition;
    public Vector3 outsidePosition;
    public int dropX, dropY;

    private void Start()
    {
        anim = GetComponent<Animator>();
        AssignRandomShape();
    }

    public void AssignRandomShape()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].SetActive(false);
        }

        int r = Random.Range(0, 4);

        switch (r)  // !!Dont forget to turn this logic into board generation!! 
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

        sprites[r].SetActive(true);

        selectedSpriteObject = sprites[r];
    }

    public void RematchShape(string tag)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].SetActive(false);
        }
        switch (tag)
        {
            case "Red":
                sprites[0].SetActive(true);
                selectedSpriteObject = sprites[0];
                break;
            case "Yellow":
                sprites[1].SetActive(true);
                selectedSpriteObject = sprites[1];
                break;
            case "Blue":
                sprites[2].SetActive(true);
                selectedSpriteObject = sprites[2];
                break;
            case "Green":
                sprites[3].SetActive(true);
                selectedSpriteObject = sprites[3];
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

    public void ScaleDownAnimationTrigger()
    {
        anim.enabled = true;
        anim.Play("Drop_Scale_Down");

    }

    public void ScaleDownAnimationEndEvent()
    {
        anim.enabled = false;
        transform.position = BoardGenerator.instance.OutsideTopPosition();
    }

    public void FallDownAnimationEndEvent()
    {
        anim.enabled = false;
    }

}
