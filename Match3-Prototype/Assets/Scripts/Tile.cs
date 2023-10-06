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
            /*
            GameObject go = BoardGenerator.instance.PlaceDropOnTile(GetComponent<Tile>());
            go.transform.position = transform.position;
            BoardGenerator.instance.DropMatrice[tileX - 1, tileY - 1] = go;
            StartCoroutine(MoveDrop_cor(go));*/

        }
        else
        {

            int x = tileX - 1;
            int y = tileY - 1;
            /*
            for (int i = 1; i <= BoardGenerator.instance.rows - tileY; i++)
            {
                GameObject go = BoardGenerator.instance.DropMatrice[x, y + i];
                if (go != null)
                { 
                    
                    //Debug.Log("[" + Time.frameCount + "]" + ".........dropX " + collision.GetComponent<Drop>().dropX + " dropY: " + collision.GetComponent<Drop>().dropY);
                    StartCoroutine(MoveDrop_cor(go));
                    BoardGenerator.instance.DropMatrice[x, y] = go;
                    BoardGenerator.instance.DropMatrice[x, y + i] = null;
                    
                    break;
                }
            }
            */
            
        }
        
        IEnumerator MoveDrop_cor(GameObject drop)
        {
            
            while (drop.transform.position.y > this.transform.position.y)
            {
                drop.transform.position = Vector2.MoveTowards(drop.transform.position, transform.position, Time.deltaTime * dropSpeed);
                yield return null;
            }
            drop.transform.position = transform.position;
        }

    }

    

}
