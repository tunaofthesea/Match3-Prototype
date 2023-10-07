using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Tile : MonoBehaviour
{
    public int tileX, tileY;
    public GameObject valve;

    public bool spawner;
    public bool isEmpty;

    public float dropSpeed;

    public GameObject activator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Drop>().dropY = tileY - 1;  // drops X & Y vallues will be used in array manipulation, so we are translating matrice coordinates into array indexes.
        collision.GetComponent<Drop>().dropX = tileX - 1;
        isEmpty = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isEmpty = true;
    }

}
