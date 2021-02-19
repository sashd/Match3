using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    [SerializeField] private Vector2Int fieldSize = new Vector2Int(5, 5);

    [SerializeField] private TileElementData[] tileElement;

    [SerializeField] private GameObject tilePrefab;

    [Range(0.7f, 1.2f)]
    [SerializeField] public float spacing = 1;

    private const int maxSize = 50;
    private TileElement[,] tiles = new TileElement[maxSize, maxSize];

    //---------------временные штуки---------------------
    
    struct Cluster
    {
        public Cluster(int column, int row, int length, bool horizontal)
        {
            this.column = column;
            this.row = row;
            this.length = length;
            this.horizontal = horizontal;
        }

        int column;
        int row;
        int length;
        bool horizontal;
    }

    struct Move
    {
        public Move(int column_1, int row_1, int column_2, int row_2)
        {
            this.column_1 = column_1;
            this.row_1 = row_1;
            this.column_2 = column_2;
            this.row_2 = row_2;
        }

        int column_1;
        int row_1;
        int column_2;
        int row_2;
    }

    List<Cluster> clusters = new List<Cluster>();
    List<Move> moves = new List<Move>();

    //-----------------------------------------------------

    private void Start()
    {
        GenerateLevel();
        FindClusters();
        FindMoves();
    }

    private void GenerateLevel()
    {
        bool done = false;
        while (!done)
        {
            for (int i = 0; i < fieldSize.x; i++)
            {
                for (int j = 0; j < fieldSize.y; j++)
                {
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject element = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                    element.transform.position = new Vector2(i * spacing, j * spacing);
                    int randomElement = Random.Range(0, tileElement.Length);
                    tiles[i, j] = element.GetComponent<TileElement>();
                    tiles[i, j].Init(tileElement[randomElement]);
                }
            }


            /*
            ResolveClusters();
            
            FindMoves();

            if (moves.Count > 0)
            {
                done = true;
            }
            */
            // убрать потом
            done = true;
        }

        // Move the grid to the center
        Vector2 gridSize = new Vector2(fieldSize.x * spacing, fieldSize.y * spacing); 
        transform.position = new Vector2(-gridSize.x / 2 + spacing / 2, -(gridSize.y / 2 - spacing / 2));
    }
    private void ResolveClusters()
    {
        FindClusters();

        while (clusters.Count > 0)
        {
            RemoveClusters();
            ShiftTiles();
            FindClusters();
        }
    }

    private void FindClusters()
    {
        clusters.Clear();

        // Find Horizontal clusters
        for (int j = 0; j < fieldSize.y; j++)
        {
            int matchLength = 1;

            for (int i = 0; i < fieldSize.x; i++)
            {
                bool checkCluster = false;

                if (i == fieldSize.x - 1)
                {
                    checkCluster = true;
                }
                else
                {
                    if (tiles[i,j].Type == tiles[i + 1, j].Type)
                    {
                        matchLength += 1;
                    }
                    else
                    {
                        checkCluster = true;
                    }
                }

                if (checkCluster)
                {
                    if (matchLength >= 3)
                    {
                        // cluster found
                        Cluster cluster = new Cluster(i + 1 - matchLength, j, matchLength, true);
                        clusters.Add(cluster);
                        //Debug.Log("horizontal cluster found: column " + (i + 1 - matchLength) + " row: " + j + " length: " + matchLength);

                    }
                    matchLength = 1;
                }

            }
        }

        // Find Vertical clusters
        for (int i = 0; i < fieldSize.x; i++)
        {
            int matchLength = 1;

            for (int j = 0; j < fieldSize.y; j++)
            {
                bool checkCluster = false;

                if (j == fieldSize.y - 1)
                {
                    checkCluster = true;
                }
                else
                {
                    if (tiles[i, j].Type == tiles[i, j + 1].Type)
                    {
                        matchLength += 1;
                    }
                    else
                    {
                        checkCluster = true;
                    }
                }

                if (checkCluster)
                {
                    if (matchLength >= 3)
                    {
                        // cluster found
                        Cluster cluster = new Cluster(i, j + 1 - matchLength, matchLength, true);
                        clusters.Add(cluster);
                        //Debug.Log("vertical cluster found: column " + i + " row: " + (j + 1 - matchLength) + " length: " + matchLength);

                    }
                    matchLength = 1;
                }

            }
        }
    }

    private void Swap(int x1, int y1, int x2, int y2)
    {
        var typeSwap = tiles[x1, y1].Type;
        tiles[x1, y1].Type = tiles[x2, y2].Type;
        tiles[x2, y2].Type = typeSwap;
    }

    private void FindMoves()
    {
        // Horizontal swaps
        for (int j = 0; j < fieldSize.y - 1; j++)
        {
            for (int i = 0; i < fieldSize.y - 1; i++)
            {
                Swap(i, j, i + 1, j);
                FindClusters();
                Swap(i, j, i + 1, j); // Swap back

                // If there was a cluster so move is available 
                if (clusters.Count > 0)
                {
                    Move move = new Move(i, j, i + 1, j);
                    moves.Add(move);
                    Debug.Log("move: (" + i + "," + j + ") -> (" + (i + 1) + "," + j + ") Horizontal swap");
                }
            }
        }
        
        // Vertical swaps
        for (int i = 0; i < fieldSize.y - 1; i++)
        {
            for (int j = 0; j < fieldSize.y - 1; j++)
            {
                Swap(i, j, i, j + 1);
                FindClusters();
                Swap(i, j, i, j + 1); // Swap back

                // If there was a cluster so move is available 
                if (clusters.Count > 0)
                {
                    Move move = new Move(i, j, i, j + 1);
                    moves.Add(move);
                    Debug.Log("move: (" + i + "," + j + ") -> (" + i + "," + (j + 1) + ") Vertical swap");
                }
            }
        }
        
        clusters.Clear();
    }

    private void ShiftTiles()
    {
        Debug.Log("Shift Tiles not implemented");
    }

    private void RemoveClusters()
    {
        Debug.Log("Remove Clusters not implemented");
    }




}
