using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileX, tileY;
    public GameObject valve;

    public bool spawner;
    public bool isEmpty;

    public float dropSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Drop>().dropY = tileY - 1;  // drops X & Y vallues will be used in array manipulation, so we are translating matrice coordinates into array indexes.
        collision.GetComponent<Drop>().dropX = tileX - 1;
        isEmpty = false;
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        isEmpty = true;
        if(TouchContoller.instance.interactionActivated)
        {
            return;
        }

        if (spawner)
        {
            
            GameObject go = BoardGenerator.instance.PlaceDropOnTile(GetComponent<Tile>());
            StartCoroutine(MoveDrop_cor(go));
            //BoardGenerator.instance.DropMatrice[tileX - 1, tileY - 1] = go;
        }
        else
        {
            //valve.SetActive(false);
            /*
             * x = tileX - 1;
             * y = tileY - 1;
             * for(int i = 1; i <= boardGenerator.instance.columns - tileY; i++)
             * {
             *  if(DropMatrice[x, y + i] != null)
             *  {
             *      DropMatrice[x, y + i].
             *  }
             * }
            */
            int x = tileX - 1;
            int y = tileY - 1;
            
            for (int i = 1; i <= BoardGenerator.instance.rows - tileY; i++)
            {
                GameObject go = BoardGenerator.instance.DropMatrice[x, y + i];
                if (go != null)
                {
                    StartCoroutine(MoveDrop_cor(go));
                    BoardGenerator.instance.DropMatrice[x, y + i] = null;
                    BoardGenerator.instance.DropMatrice[x, y] = go;
                    break;
                }
            }
            
            //BoardGenerator.instance.TileCheck(x, y);
        }
        
        IEnumerator MoveDrop_cor(GameObject drop)
        {
            while(drop.transform.position.y > this.transform.position.y)
            {
                drop.transform.position = Vector2.MoveTowards(drop.transform.position, transform.position, Time.deltaTime * dropSpeed);
                yield return null;
            }
            drop.transform.position = transform.position;

        }

    }

    

}
