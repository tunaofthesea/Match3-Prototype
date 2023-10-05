using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public int rows;
    public int columns;
    public int dropNumber;
    public int minMatchingLength = 3;

    public GameObject tilePrefab;
    public GameObject dropPrefab;

    public List<GameObject> GenerativeTiles = new List<GameObject>();

    public List<GameObject> DropsOnBoard = new List<GameObject>();
    public GameObject[,] DropMatrice;
    public int selectedX, selectedY;

    public static BoardGenerator instance;
    public GameObject platformCollider;

    private List<GameObject> Tiles = new List<GameObject>();
    private GameObject boardParent;
    private GameObject dropPool;
    private float waitFor = 0.01f;

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

    private void Start()
    {
        DropMatrice = new GameObject[columns, rows];
        StartCoroutine(GenerateBoard_cor());
        GenerateDrops();
    }

    public void GenerateBoard()
    {
        boardParent = new GameObject("Board Parent");
        float tileWidth = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        float startX = -((columns * tileWidth) / 2) + (tileWidth / 2);
        float startY = -((rows * tileWidth) / 2) + (tileWidth / 2);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // flattening should have its own method
                Vector2 tilePos = new Vector2(startX + (j * tileWidth), startY + (i * tileWidth));
                GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity);
                tile.transform.parent = boardParent.transform;
            }
        }
    }

    IEnumerator GenerateBoard_cor()
    {
        boardParent = new GameObject("Board Parent");
        float tileWidth = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        float startX = -((rows * tileWidth) / 2) + (tileWidth / 2);
        float startY = -((columns * tileWidth) / 2) + (tileWidth / 2);

        for (int i = 1; i < rows + 1; i++)
        {
            for (int j = 1; j < columns + 1; j++)
            {
                Vector2 tilePos = new Vector2(startX + (j * tileWidth), startY + (i * tileWidth));
                GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity);

                tile.name = "Tile (" + j + ", " + i + ")";
                tile.GetComponent<Tile>().x = j;
                tile.GetComponent<Tile>().y = i;
                // this can be moved out of this loop and have its own loop
                if (i == rows)
                {
                    tile.GetComponent<Tile>().spawner = true;
                    GenerativeTiles.Add(tile);
                }
                Tiles.Add(tile);
                tile.transform.parent = boardParent.transform;
                // these configuration values should have their own variables
                yield return new WaitForSeconds(this.waitFor);
            }
            yield return new WaitForSeconds(this.waitFor);
        }

        platformCollider.transform.parent = Tiles[columns / 2].transform;
        platformCollider.transform.localPosition = Vector2.zero;

        StartCoroutine(PlaceDrops_cor());
    }

    public void GenerateDrops()
    {
        dropPool = new GameObject("Drop Pool");
        // worst case scenario where both swipes land a matching 'trident' 
        // e.g. when LR is swiped to RL -> there will minMatchingLength*3 - 2 matching tiles (-2 because we counted more) 
        // another way to think about it -> 3*(minMatchingLength-1)+1
        dropNumber = minMatchingLength * 6 - 4;

        Vector3 tempPos = OutsideTopPosition();

        for (int i = 0; i < dropNumber; i++)
        {
            GameObject drop = Instantiate(dropPrefab, tempPos, Quaternion.identity);
            drop.transform.parent = dropPool.transform;
        }

    }

    Vector3 OutsideTopPosition()
    {
        // Object pool should not be spawned on the screen boundaries. 
        Vector3 screenPos = new Vector3(Screen.width / 2, Screen.height, -Camera.main.transform.position.z);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector3 tempPos = worldPos + new Vector3(0, 10, 0);

        return tempPos;
    }

    IEnumerator PlaceDrops_cor()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            GameObject go = dropPool.transform.GetChild(dropPool.transform.childCount - 1).gameObject;
            //go.name = Tiles[i].name;
            DropsOnBoard.Add(go);
            go.transform.parent = Tiles[i].transform;
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<Collider2D>().enabled = true;
            yield return new WaitForSeconds(0.01f);
        }
        AssignBoardDimensions();
    }

    public GameObject PlaceDropOnTile(Tile tile)
    {
        float tileWidth = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        GameObject go = dropPool.transform.GetChild(dropPool.transform.childCount - 1).gameObject;
        go.transform.parent = tile.transform;
        go.transform.localPosition = Vector2.zero + new Vector2(0, tileWidth);
        go.GetComponent<Collider2D>().enabled = true;

        return go;

    }

    public void CheckGenTiles()
    {
        for (int i = 0; i < GenerativeTiles.Count; i++)
        {
            if (GenerativeTiles[i].GetComponent<Tile>().isEmpty)
            {
                PlaceDropOnTile(GenerativeTiles[i].GetComponent<Tile>());
            }
        }
    }

    public void AssignBoardDimensions()
    {
        Drop d;
        for (int i = 0; i < DropsOnBoard.Count; i++)
        {
            Debug.Log(i);
            d = DropsOnBoard[i].GetComponent<Drop>();
            DropMatrice[d.x, d.y] = DropsOnBoard[i];
        }
    }

    public void RelocateChangedDrops(GameObject drop, int x, int y)
    {
        DropMatrice[x, y] = drop;
    }

    public list<GameObject> CheckMatchesAt(int row, int col)
    {
        List<GameObject> toDestroy = new List<GameObject>();
        // Check vertical 
        // go left
        int l, r = col, col;
        // move left as long as the left tile is the same as that of original position 
        while (l > 0 && DropMatrice[row, l - 1] != null && DropMatrice[row, col].tag == DropMatrice[row, l - 1].tag)
        {
            l--;
            toDestroy.Add(DropMatrice[row, l]);
        }
        // move left as long as the right tile is the same as that of original position 
        while (r < columns - 1 && DropMatrice[row, r + 1] != null && DropMatrice[row, col].tag == DropMatrice[row, r + 1].tag)
        {
            r++;
            toDestroy.Add(DropMatrice[row, r]);
        }
        int u, d = row, row;
        // move up as long as the upper tile is the same as that of original position 
        while (u > rows - 1 && DropMatrice[u - 1, col] != null && DropMatrice[row, col].tag == DropMatrice[u + 1, col].tag)
        {
            u++;
            toDestroy.Add(DropMatrice[u, col]);
        }
        // move down as long as the below tile is the same as that of original position 
        while (d > 0 && DropMatrice[d - 1, col] != null && DropMatrice[row, col].tag == DropMatrice[d - 1, col].tag)
        {
            d++;
            toDestroy.Add(DropMatrice[d, col]);
        }
        return toDestroy;
    }

    public void ReturnDropsBackToPool(List<GameObject> toDestroy)
    {
        // Returns the used drops back into the object pool
        foreach (GameObject drop in toDestroy)
        {
            drop.transform.parent = dropPool.transform;
            drop.transform.position = OutsideTopPosition();
            Drop d = drop.GetComponent<Drop>();
            // Coment this changes the pop wont work
            int x = d.x;
            int y = d.y;

            DropMatrice[x, y] = null;
            /*
            if (y + 1 < rows && DropMatrice[x, y + 1] != null)
            {
                DropTile(DropMatrice[x, y + 1].GetComponent<Drop>());
            }*/
        }
    }


    public void TileCheck(int x, int y)
    {
        for (int i = 1; i < rows - (y + 1); i++)
        {
            GameObject go = DropMatrice[x, y + i];
            if (go != null)
            {
                StartCoroutine(MoveDrop_cor(go));
                DropMatrice[x, y + i] = null;
                DropMatrice[x, y] = go;
                break;
            }
        }
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

