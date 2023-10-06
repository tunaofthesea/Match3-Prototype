using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TouchContoller : MonoBehaviour
{
    public GameObject selectedObject;

    public LayerMask clickableLayer;

    public bool interactionActivated;

    public float SwapSpeed;

    public static TouchContoller instance;
    private Vector2 initialTouchPos;
    private Vector2 finalTouchPos;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!interactionActivated)
        {
            if (Input.GetMouseButtonDown(0)) // Detects touch input.
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, clickableLayer);

                if (hit.collider == null)  // Touch object detected and initial touch position is saved.
                {
                    Debug.Log("No objects detected.");
                    return;
                }
                Debug.Log("Clicked on " + hit.collider.gameObject.name);
                selectedObject = hit.collider.gameObject;
                initialTouchPos = hit.point;
            }

            else if (Input.GetMouseButtonUp(0))
            {
                finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 swipeDirection = finalTouchPos - initialTouchPos;

                swipeDirection.Normalize();  // I would like to see the distance as a unit vector

                // assume it is vertical at first
                char dir = swipeDirection.y > 0 ? 'u' : 'd';

                // Compares the magnitudes of x and y and chooses the best swipe direction
                if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                    dir = swipeDirection.x > 0 ? 'r' : 'l'; //change if horizontal instead
            }
        }
    }

    public void SwapNeighbor(char c)
    {
        BoardGenerator board = BoardGenerator.instance;
        GameObject neighbor;

        int horizontal = Convert.ToInt16((c == 'r')) - Convert.ToInt16((c == 'l'));
        int vertical = Convert.ToInt16((c == 'u')) - Convert.ToInt16((c == 'd'));

        Drop d = selectedObject.GetComponent<Drop>();
        int x, y = d.x + horizontal, d.y + vertical

        try
        {
            GameObject value = board.DropMatrice[x, y];
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log("Caught exception: " + e.Message);
            return;
        }
        if (board.DropMatrice[x, y] == null) return;

        // check this
        neighbor = board.DropMatrice[x, y];

        TrySwap(board, selectedObject, neighbor, x, y, x - horizontal, y - vertical);

        //board.RelocateChangedDrops(selectedObject, x, y);
        //board.RelocateChangedDrops(neighbor, x - 1, y);

    }


    public void TrySwap(BoardGenerator boardGenerator, GameObject selected, GameObject neighbor, int x, int y, int relocateX, int relocateY)
    {
        Vector2 neighborPos;
        Vector2 initialPos;

        neighborPos = boardGenerator.DropMatrice[x, y].transform.position;
        initialPos = selectedObject.transform.position;

        StartCoroutine(TrySwap_cor(selected, neighbor, initialPos, neighborPos, x, y, relocateX, relocateY));
    }

    IEnumerator TrySwap_cor(GameObject selected, GameObject neighbor, Vector2 initialPos, Vector2 neighborPos, int x, int y, int relocateX, int relocateY)
    {
        interactionActivated = true;

        selectedObject.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
        neighbor.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = 2;

        //selectedObject.GetComponent<Collider2D>().enabled = false;
        //neighbor.GetComponent<Collider2D>().enabled = false;

        while (true)
        {
            neighbor.transform.position = Vector2.MoveTowards(neighbor.transform.position, initialPos, Time.deltaTime * SwapSpeed);
            selected.transform.position = Vector2.MoveTowards(selected.transform.position, neighborPos, Time.deltaTime * SwapSpeed);

            // 0.05f can be another variable 
            if (Vector2.Distance(neighbor.transform.position, initialPos) < 0.05f)
            {
                neighbor.transform.position = initialPos;
                selected.transform.position = neighborPos;

                //interactionActivated = false;
                break;
            }
            yield return null;
        }
        selectedObject.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        neighbor.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

        BoardGenerator.instance.RelocateChangedDrops(selectedObject, x, y);
        BoardGenerator.instance.RelocateChangedDrops(neighbor, relocateX, relocateY);

        // If checkMatch bool function returns false, it means there are no matches so, we revert back the change we did in the DropMatrice
        if (!BoardGenerator.instance.CheckMatches())
        {
            BoardGenerator.instance.RelocateChangedDrops(neighbor, x, y);
            BoardGenerator.instance.RelocateChangedDrops(selectedObject, relocateX, relocateY);

            selectedObject.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
            neighbor.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

            while (true)
            {
                neighbor.transform.position = Vector2.MoveTowards(neighbor.transform.position, neighborPos, Time.deltaTime * SwapSpeed);
                selected.transform.position = Vector2.MoveTowards(selected.transform.position, initialPos, Time.deltaTime * SwapSpeed);

                // 0.05f can be another variable 
                if (Vector2.Distance(neighbor.transform.position, neighborPos) < 0.05f)
                {
                    neighbor.transform.position = neighborPos;
                    selected.transform.position = initialPos;

                    //interactionActivated = false;
                    break;
                }
                yield return null;
            }
            selectedObject.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            neighbor.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        interactionActivated = false;

    }
}
