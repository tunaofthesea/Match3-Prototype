using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
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

    public int coroutineNumber;

    private int clickCount;

    public bool dropping;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
                clickCount++;
                if(clickCount == 1)
                {
                    DebugDropMatrice();
                    Time.timeScale = 0;
                }
                else
                {
                    Time.timeScale = 1;
                    clickCount = 0;
                }
        }
    }
#endif
    public void InitializeGame()
    {
        DropMatrice = new GameObject[columns, rows];
        StartCoroutine(GenerateBoard_cor());
        GenerateDrops();
    }

    /*
    private void Start()
    {
        DropMatrice = new GameObject[columns, rows];
        StartCoroutine(GenerateBoard_cor());
        GenerateDrops();
        //GenerateBoard();
    }
    */
    void CheckMatriceAtStart()
    {

    }
    IEnumerator GenerateBoard_cor()
    {
        boardParent = new GameObject("Board Parent");
        float tileWidth = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        float startX = -((columns * tileWidth) / 2) + (tileWidth / 2) - 0.8f;
        float startY = -((rows * tileWidth) / 2) + (tileWidth / 2) - 0.8f;

        for (int i = 1; i < rows + 1; i++)
        {
            for (int j = 1; j < columns + 1; j++)
            {
                Vector2 tilePos = new Vector2(startX + (j * tileWidth), startY + (i * tileWidth));
                GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity);

                tile.name = "Tile (" + j + ", " + i + ")";
                tile.GetComponent<Tile>().tileX = j;
                tile.GetComponent<Tile>().tileY = i;
                if (i == rows)
                {
                    tile.GetComponent<Tile>().spawner = true;
                    tile.GetComponent<Tile>().activator.SetActive(true);
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

        if (dropNumber < rows * columns)
        {
            Debug.LogError("Warning, you have entered a number smaller than the size of the board matrice");
            dropNumber = rows * columns + (rows + columns);
        }

        Vector3 tempPos = OutsideTopPosition();

        for (int i = 0; i < dropNumber; i++)
        {
            GameObject drop = Instantiate(dropPrefab, tempPos, Quaternion.identity);
            drop.transform.parent = dropPool.transform;
            drop.name = "drop" + i;
        }

    }

    public Vector3 OutsideTopPosition()
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

        GameObject go = dropPool.transform.GetChild(0).gameObject;
        if (tile != null)
        {
            go.transform.parent = tile.transform;
            //go.transform.localPosition = Vector2.zero + new Vector2(0, tileWidth);
            go.transform.position = tile.transform.position;
            //go.GetComponent<Collider2D>().enabled = true;
        }

        else
        {
            go.transform.parent = null;  // It will be used placeDropsOnTop script, if I dont remove it from the pool parent, the same object will be used when creating drops over the tiles
        }
        return go;

    }

    public void AssignBoardDimensions()
    {
        for (int i = 0; i < DropsOnBoard.Count; i++)
        {
            //Debug.Log(i);
            DropMatrice[DropsOnBoard[i].GetComponent<Drop>().dropX, DropsOnBoard[i].GetComponent<Drop>().dropY] = DropsOnBoard[i];
        }

        MatchChecker.instance.CheckMatches(DropMatrice);
    }

    public void RelocateChangedDrops(GameObject drop, int x, int y)
    {
        DropMatrice[x, y] = drop;
    }

    public bool CheckMatches() 
    {
        List<GameObject> toDestroy = new List<GameObject>();

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

            foreach (GameObject drop in toDestroy)   // Returns the used drops back into the object pool
            {
                drop.transform.parent = dropPool.transform;
                drop.GetComponent<Drop>().ScaleDownAnimationTrigger();

                int x = drop.GetComponent<Drop>().dropX;
                int y = drop.GetComponent<Drop>().dropY;

                DropMatrice[x, y] = null;

            }
            FillAndSpawnDrops();
            return true;
        }
        else
        {
            //FillAndSpawnDrops();
            return false;
        }

    }

    public void FillAndSpawnDrops()
    {
        //SpawnDropsForEmptyTopTiles();

        FillEmptyTiles();
        CheckAndPlace();  // Added extra, didn't check If that produces a bottle neck, but fixed an issue where the top tiles won't spawn new drops.

        //SpawnDropsForEmptyTopTiles();
    }

    public void FillEmptyTiles()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows - 1; y++)
            {
                if (DropMatrice[x, y] == null)  // If we found an empty spot
                {
                    for (int upY = y + 1; upY < rows; upY++)
                    {
                        if (DropMatrice[x, upY] != null)
                        {
                            GameObject dropToMove = DropMatrice[x, upY];
                            Vector2 targetPosition = Tiles[x + (columns * y)].transform.position;
                            DropMatrice[x, y] = dropToMove;
                            DropMatrice[x, upY] = null;

                            StartCoroutine(MoveDropToPosition(dropToMove, targetPosition));

                            dropToMove.GetComponent<Drop>().dropX = x;
                            dropToMove.GetComponent<Drop>().dropY = y;
                            break;
                        }
                    }
                }
            }
        }
    }

    IEnumerator MoveDropToPosition(GameObject drop, Vector2 targetPosition)
    {
        dropping = true;
        coroutineNumber++;
        float speed = dropSpeed;
        float gravity = -9.8f;
        float verticalVelocity = 0.0f;

        while ((Vector2)drop.transform.position != targetPosition)
        {
            verticalVelocity += gravity * Time.deltaTime;
            float newYPosition = drop.transform.position.y + verticalVelocity * Time.deltaTime;

            if (newYPosition <= targetPosition.y)
            {
                newYPosition = targetPosition.y;
                drop.transform.position = new Vector2(drop.transform.position.x, newYPosition);
                break;
            }
            drop.transform.position = new Vector2(drop.transform.position.x, newYPosition);
            yield return null;
        }

        drop.transform.position = targetPosition;

        coroutineNumber--;

        
        if (coroutineNumber == 0)
        {
            if(!CheckMatches())
            {
                CheckAndPlace();
            }
            dropping = false;
        }
    }

    public void CheckAndPlace()
    {
        for (int i = 0; i < columns; i++)
        {
            //Debug.Log("Checking Column: " + i);
            int countOfNull = 0;

            for (int j = 0; j < rows; j++)
            {
                if (DropMatrice[i, j] == null)
                {
                    //Debug.Log("Found Null at: [" + i + ", " + j + "]");
                    countOfNull++;

                    if (j == rows - 1 || DropMatrice[i, j + 1] != null) // Check if I reached the top or the next isn't null
                    {
                        PlaceDropsOnTop(i, j, countOfNull);

                        countOfNull = 0;
                    }
                }
                else if (countOfNull > 0)
                {
                    //Debug.Log("Found non-null after a sequence at: [" + i + ", " + j + "]");
                    countOfNull = 0;
                }
            }
        }

        DebugDropMatrice();
    }

    void PlaceDropsOnTop(int column,int row, int numberOfDrops)
    {
        float spriteWidth = dropPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        int topTileIndex = column + columns * (rows - 1);         // Determine the base Y position for the drop placement, from the top tile of the column
        Vector2 topTilePos = Tiles[topTileIndex].transform.position;

        if(!Tiles[topTileIndex].GetComponent<Tile>().spawner) // Checks if tile can spawn drops on top. If not returns. (Case requirement number 4)
        {
            return;  
        }

        for (int i = 0; i < numberOfDrops; i++)
        {
            //Debug.Log("Creating Drop " + (i + 1) + " for column: " + column);
            Vector2 newPosition = topTilePos + new Vector2(0, spriteWidth * (i + 1));
            Vector2 targetPos = topTilePos + new Vector2(0, spriteWidth * (i + 1 - numberOfDrops));  // I am calculating the target tile location, because getting the Tiles[index] is costy at this point

            GameObject spawnedDrop = PlaceDropOnTile(null);    // Get a drop from the pool or instantiate a new one
            spawnedDrop.GetComponent<Drop>().AssignRandomShape();  // Reassigns a random shape 
            DropMatrice[column, row - (numberOfDrops - i - 1)] = spawnedDrop;

            spawnedDrop.transform.position = newPosition;

            StartCoroutine(MoveDropToPosition(spawnedDrop, targetPos));
        }

    }

    void DebugDropMatrice()
    {
        string output = "DropMatrice content:\n";

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (DropMatrice[i, j] != null)
                    output += DropMatrice[i, j].name + " ";
                else
                    output += "null ";
            }
            output += "\n";
        }

        Debug.Log(output);
    }

    // Matchler den sonra, elenen ve pool'a geri d�nen droplar bir order olu?turdu?u i�in, eksik olan yerlere bu pooldan s?ra ile obje �ekersem zaten matchlenmi? olan s?ralar? tekrardan �a??rm?? oluyorum.



}

