using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TouchContoller : MonoBehaviour
{
    private Vector2 initialTouchPos;
    private Vector2 finalTouchPos;
    public GameObject selectedObject;

    public LayerMask clickableLayer;

    public bool interactionActivated;

    public float SwapSpeed;

    public static TouchContoller instance;


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

                if (hit.collider != null)  // Touch object detected and initial touch position is saved.
                {
                    Debug.Log("Clicked on " + hit.collider.gameObject.name);
                    selectedObject = hit.collider.gameObject;
                    initialTouchPos = hit.point;
                }
                else
                {
                    Debug.Log("No objects detected.");
                }
            }

            else if (Input.GetMouseButtonUp(0))
            {
                if(selectedObject == null)
                {
                    return;
                }
                finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 swipeDirection = finalTouchPos - initialTouchPos;

                swipeDirection.Normalize();  // I would like to see the distance as a unit vector

                if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))  // Compares the magnitudes of x and y and chooses the best swipe direction
                {

                    if (swipeDirection.x > 0)
                    {
                        Debug.Log("Swipe Right");
                        SwapNeighbor('r');
                    }

                    else
                    {
                        Debug.Log("Swipe Left");
                        SwapNeighbor('l');
                    }
                }
                else
                {

                    if (swipeDirection.y > 0)
                    {
                        Debug.Log("Swipe Up");
                        SwapNeighbor('u');
                    }
                    else
                    {
                        Debug.Log("Swipe Down");
                        SwapNeighbor('d');
                    }
                }
            }
        }
    }

    public void SwapNeighbor(char c)
    {
        int x, y;
        BoardGenerator board = BoardGenerator.instance;
        GameObject neighbor;

        switch (c)
        {
            case 'r':
                x = selectedObject.GetComponent<Drop>().dropX + 1;
                y = selectedObject.GetComponent<Drop>().dropY;

                try
                {
                    GameObject value = board.DropMatrice[x, y];
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log("Caught exception: " + e.Message);
                    break;
                }
                if (board.DropMatrice[x, y] == null)
                {
                    break;
                }
                neighbor = board.DropMatrice[x, y];

                TrySwap(board, selectedObject, neighbor, x, y, x - 1, y);

                break;

            case 'l':
                x = selectedObject.GetComponent<Drop>().dropX - 1;
                y = selectedObject.GetComponent<Drop>().dropY;
                try
                {
                    GameObject value = board.DropMatrice[x, y];
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log("Caught exception: " + e.Message);
                    break;
                }
                if (board.DropMatrice[x, y] == null)
                {
                    break;
                }
                neighbor = board.DropMatrice[x, y];

                TrySwap(board, selectedObject, neighbor, x, y, x + 1, y);


                break;

            case 'u':
                x = selectedObject.GetComponent<Drop>().dropX;
                y = selectedObject.GetComponent<Drop>().dropY + 1;

                try
                {
                    GameObject value = board.DropMatrice[x, y];
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log("Caught exception: " + e.Message);
                    break;
                }
                if (board.DropMatrice[x, y] == null)
                {
                    break;
                }
                neighbor = board.DropMatrice[x, y];

                TrySwap(board, selectedObject, neighbor, x, y, x, y - 1);

                break;

            case 'd':
                x = selectedObject.GetComponent<Drop>().dropX;
                y = selectedObject.GetComponent<Drop>().dropY - 1;

                try
                {
                    GameObject value = board.DropMatrice[x, y];
                }

                catch (IndexOutOfRangeException e)
                {
                    Debug.Log("Caught exception: " + e.Message);
                    break;
                }
                if (board.DropMatrice[x, y] == null)
                {
                    break;
                }
                neighbor = board.DropMatrice[x, y];

                TrySwap(board, selectedObject, neighbor, x, y, x, y + 1);

                break;
        }
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

        SwapSpritesOrder(selected, 3, neighbor, 2);
        yield return SwapPositions(selected, neighbor, initialPos, neighborPos);

        BoardGenerator.instance.RelocateChangedDrops(selectedObject, x, y);  // Chanes the indees of the swapped drop objects in the DropMatrice 
        BoardGenerator.instance.RelocateChangedDrops(neighbor, relocateX, relocateY);

        if (!BoardGenerator.instance.CheckMatches())  // No matches
        {
            BoardGenerator.instance.RelocateChangedDrops(neighbor, x, y);  // Sets back the initial indexes of the swapped objects 
            BoardGenerator.instance.RelocateChangedDrops(selectedObject, relocateX, relocateY);
            SwapSpritesOrder(selected, 2, neighbor, 3);
            yield return SwapPositions(selected, neighbor, neighborPos, initialPos);  // If no match found, initializes new Swap Position coroutine (reversed version) ad returns
        }

        SwapSpritesOrder(selected, 1, neighbor, 1);
        interactionActivated = false;

        BoardGenerator.instance.CheckAndPlace();

        selectedObject = null;

    }

    IEnumerator SwapPositions(GameObject selected, GameObject neighbor, Vector2 pos1, Vector2 pos2)  // Base function to move two adjecent drops 
    {
        while (true)
        {
            neighbor.transform.position = Vector2.MoveTowards(neighbor.transform.position, pos1, Time.deltaTime * SwapSpeed);
            selected.transform.position = Vector2.MoveTowards(selected.transform.position, pos2, Time.deltaTime * SwapSpeed);

            if (Vector2.Distance(neighbor.transform.position, pos1) < 0.05f)
            {
                neighbor.transform.position = pos1;
                selected.transform.position = pos2;
                yield break;
            }
            yield return null;
        }
    }

    void SwapSpritesOrder(GameObject obj1, int order1, GameObject obj2, int order2)   // Takes the rendering order of the clicked drop front, to make an effectshown in the sample video 2
    {
        obj1.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = order1;
        obj2.GetComponent<Drop>().selectedSpriteObject.GetComponent<SpriteRenderer>().sortingOrder = order2;
    }

}
