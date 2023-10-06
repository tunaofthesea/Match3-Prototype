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

    public GameObject tilePrefab;
    public GameObject dropPrefab;

    private List<GameObject> Tiles = new List<GameObject>();
    public List<GameObject> GenerativeTiles = new List<GameObject>();

    public List<GameObject> DropsOnBoard = new List<GameObject>();

    private GameObject boardParent;
    private GameObject dropPool;

    public GameObject[,] DropMatrice;
    public int selectedX, selectedY;

    public static BoardGenerator instance;
    public GameObject platformCollider;

    public float dropSpeed;

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
                tile.GetComponent<Tile>().tileX = j;
                tile.GetComponent<Tile>().tileY = i;
                if(i == rows)
                {
                    tile.GetComponent<Tile>().spawner = true;
                    GenerativeTiles.Add(tile);
                }
                Tiles.Add(tile);
                tile.transform.parent = boardParent.transform;
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.01f);
        }
        platformCollider.transform.parent = Tiles[columns / 2].transform;
        platformCollider.transform.localPosition = Vector2.zero;

        StartCoroutine(PlaceDrops_cor());
    }
    
    
    public void GenerateDrops()
    {
        dropPool = new GameObject("Drop Pool");

        if(dropNumber < rows * columns)
        {
            Debug.LogError("Warning, you have entered a number smaller than the size of the board matrice");
            dropNumber = rows * columns + (rows + columns);
        }

        Vector3 tempPos = OutsideTopPosition();

        for (int i = 0; i < dropNumber; i++)
        {
            GameObject drop = Instantiate(dropPrefab, tempPos, Quaternion.identity);
            drop.transform.parent = dropPool.transform;
        }

    }

    Vector3 OutsideTopPosition()
    {
        Vector3 screenPos = new Vector3(Screen.width / 2, Screen.height, -Camera.main.transform.position.z);  // Object pool should not be spawned on the screen boundaries. 
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
        for (int i = 0; i < DropsOnBoard.Count; i++)
        {
            //Debug.Log(i);
            DropMatrice[DropsOnBoard[i].GetComponent<Drop>().dropX, DropsOnBoard[i].GetComponent<Drop>().dropY] = DropsOnBoard[i];
        }
    }

    public void RelocateChangedDrops(GameObject drop, int x, int y)
    {
        DropMatrice[x, y] = drop;
    }

    public bool CheckMatches()
    {
        List<GameObject> toDestroy = new List<GameObject>();
        // Checks rows
        for (int row = 0; row < columns; row++)
        {
            int count = 1;
            for (int col = 1; col < rows; col++)
            {
                if ((DropMatrice[row, col] != null && DropMatrice[row, col - 1] != null) && DropMatrice[row, col].tag == DropMatrice[row, col - 1].tag)
                {
                    count++;
                    if (count >= 3)
                    {
                        for (int j = col; j > col - count; j--)
                        {
                            toDestroy.Add(DropMatrice[row, j]);
                        }
                    }
                }
                else
                {
                    count = 1;
                }
            }
        }

        // Checks columns
        for (int col = 0; col < rows; col++)
        {
            int count = 1;
            for (int row = 1; row < columns; row++)
            {
                if ((DropMatrice[row, col] != null && DropMatrice[row - 1, col] != null) && DropMatrice[row, col].tag == DropMatrice[row - 1, col].tag)
                {
                    count++;
                    if (count >= 3)
                    {
                        for (int i = row; i > row - count; i--)
                        {
                            toDestroy.Add(DropMatrice[i, col]);
                        }
                    }
                }
                else
                {
                    count = 1;
                }
            }
        }

        if (toDestroy.Count > 0)
        {
            // Returns the used drops back into the object pool
            foreach (GameObject drop in toDestroy)
            {
                drop.transform.parent = dropPool.transform;
                drop.transform.position = OutsideTopPosition();

                // Coment this changes the pop wont work
                int x = drop.GetComponent<Drop>().dropX;
                int y = drop.GetComponent<Drop>().dropY;

                DropMatrice[x, y] = null;

            }
            BoardGenerator.instance.FillEmptyTiles();
            return true;
        }
        else
        {
            return false;
        }

    }
   
    public void TileCheck(int x, int y)
    {
        for (int i = 1; i < rows - (y + 1); i++)
        {
            GameObject go = DropMatrice[x, y + i];
            if (go != null)
            {
                DropMatrice[x, y + i] = null;
                DropMatrice[x, y] = go;
                break;
            }
        }
    }

    public void FillEmptyTiles()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows - 1; y++)  // Start from the bottom and go up, hence no need to check the topmost row
            {
                if (DropMatrice[x, y] == null)  // If we found an empty spot
                {
                    for (int aboveY = y + 1; aboveY < rows; aboveY++)  // Search the tiles above
                    {
                        if (DropMatrice[x, aboveY] != null)  // If we found a tile above that's not null
                        {
                            GameObject dropToMove = DropMatrice[x, aboveY];  // Store a reference to the drop we're moving
                            Vector2 targetPosition = Tiles[x + (columns * y)].transform.position;
                            DropMatrice[x, y] = dropToMove;  // Assign the moving drop to the current empty position in the matrix
                            DropMatrice[x, aboveY] = null;  // Set the original position to null

                            StartCoroutine(MoveDropToPosition(dropToMove, targetPosition));  // Start the movement coroutine

                            // Update the logical position stored in the Drop component (if any)
                            dropToMove.GetComponent<Drop>().dropX = x;
                            dropToMove.GetComponent<Drop>().dropY = y;

                            break;  // Move to the next tile
                        }
                    }
                }
            }
        }
    }

    IEnumerator MoveDropToPosition(GameObject drop, Vector2 targetPosition)
    {
        while ((Vector2)drop.transform.position != targetPosition)
        {
            drop.transform.position = Vector2.MoveTowards(drop.transform.position, targetPosition, Time.deltaTime * dropSpeed);
            yield return null;
        }
    }


}

