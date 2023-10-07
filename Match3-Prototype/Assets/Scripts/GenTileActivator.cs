using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenTileActivator : MonoBehaviour
{
    int clickCount;
    public Sprite[] sprites;
    public void OnClick()
    {
        clickCount++;

        if(clickCount % 2 == 0)
        {
            clickCount = 0;
        }

        if(clickCount == 1)
        {
            transform.parent.GetComponent<Tile>().spawner = false;
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        }

        else
        {
            transform.parent.GetComponent<Tile>().spawner = true;
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
    }
}
