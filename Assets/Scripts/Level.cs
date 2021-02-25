using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clusters;
using Moves;
using System;

public class Level: MonoBehaviour
{
    private const int maxSize = 10;

    [Range(3, maxSize)]
    [SerializeField] private float width = 5;
    [Range(3, maxSize)]
    [SerializeField] private float height = 5;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private TileElementData[] tileElements;
    [SerializeField] private float tileGenerationDelay = 0.3f; 

    [Range(0.7f, 1.2f)]
    [SerializeField] public float spacing = 1;

    public event Action OnNoMovesLeft;
    public event Action OnReadyToMakeMove;
    public event Action<int> OnClusterBreak;

    private TileElement[,] tiles = new TileElement[maxSize, maxSize];
    private List<Cluster> clusters = new List<Cluster>();
    private List<Move> moves = new List<Move>();

    public void Generate()
    {
        bool levelGenerated = false;
        while (!levelGenerated)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = PlaceRandomTile(i, j);
                }
            }
            FindClusters();
            while (clusters.Count > 0)
            {
                RemoveClusters();
                FindClusters();
            }

            FindMoves();
            if (moves.Count > 0)
            {
                levelGenerated = true;
                MoveGridToCenter();
                Debug.Log("Level generated with " + clusters.Count + " clusters and " + moves.Count + " available moves");
                OnReadyToMakeMove?.Invoke();
            }
            else
            {
                // Re-init all tiles
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        RandomInit(tiles[i,j],i,j);
            }
        }
    }

    private void MoveTiles(Move move)
    {
        tiles[move.from.x, move.from.y].Move(tiles[move.to.x, move.to.y].transform.localPosition, move.to.x, move.to.y);
        tiles[move.to.x, move.to.y].Move(tiles[move.from.x, move.from.y].transform.localPosition, move.from.x, move.from.y);
        SwapTiles(move.from.x, move.from.y, move.to.x, move.to.y);
    }

    public IEnumerator MakeMove(Move move)
    {
        MoveTiles(move);

        yield return new WaitUntil(() => tiles[move.from.x, move.from.y].Moving == false);

        FindClusters();
        if (clusters.Count > 0)
        {
            BreakClusters();
        }
        else
        {
            // Return back if there is no cluster
            MoveTiles(move);
        }
    }

    private void SwapTiles(int x1, int y1, int x2, int y2)
    {
        TileElement temp = tiles[x1, y1];
        tiles[x1, y1] = tiles[x2, y2];
        tiles[x2, y2] = temp;
    }

    private TileElement PlaceRandomTile(int x, int y)
    {
        Vector2 tilePosition = new Vector2(x, y);
        GameObject element = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
        element.transform.localPosition = new Vector2(x * spacing, y * spacing);
        TileElement tile = element.GetComponent<TileElement>();
        RandomInit(tile, x, y);
        return tile;
    }

    private void FindClusters()
    {
        clusters.Clear();

        // Find Horizontal clusters
        for (int j = 0; j < height; j++)
        {
            int matchLength = 1;
            for (int i = 0; i < width; i++)
            {
                bool clusterChecked = false;
                if (i == width - 1)
                {
                    clusterChecked = true;
                }
                else
                {
                    if (tiles[i, j].Type == tiles[i + 1, j].Type && tiles[i, j].Type != TileType.empty)
                    {
                        matchLength += 1;
                    }
                    else
                    {
                        clusterChecked = true;
                    }
                }

                if (clusterChecked)
                {
                    if (matchLength >= 3)
                    {
                        // Cluster found
                        Cluster cluster = new Cluster(i + 1 - matchLength, j, matchLength, true);
                        clusters.Add(cluster);
                    }
                    matchLength = 1;
                }
            }
        }

        // Find Vertical clusters
        for (int i = 0; i < width; i++)
        {
            int matchLength = 1;
            for (int j = 0; j < height; j++)
            {
                bool clusterChecked = false;
                if (j == height - 1)
                {
                    clusterChecked = true;
                }
                else
                {
                    if (tiles[i, j].Type == tiles[i, j + 1].Type && tiles[i, j].Type != TileType.empty)
                    {
                        matchLength += 1;
                    }
                    else
                    {
                        clusterChecked = true;
                    }
                }

                if (clusterChecked)
                {
                    if (matchLength >= 3)
                    {
                        // Cluster found
                        Cluster cluster = new Cluster(i, j + 1 - matchLength, matchLength, false);
                        clusters.Add(cluster);
                    }
                    matchLength = 1;
                }
            }
        }
    }

    private void MoveGridToCenter()
    {
        Vector2 gridSize = new Vector2(width * spacing, height * spacing);
        transform.position = new Vector2(-gridSize.x / 2 + spacing / 2, -(gridSize.y / 2 - spacing / 2));
    }

    private void RemoveClusters()
    {
        // Re-Init all tiles in the cluster
        foreach (Cluster cluster in clusters)
        {
            for (int i = 0; i < cluster.length; i++)
            {
                if (cluster.horizontal)
                {
                    RandomInit(tiles[cluster.column + i, cluster.row], cluster.column + i, cluster.row);
                }
                else
                {
                    RandomInit(tiles[cluster.column, cluster.row + i], cluster.column, cluster.row + i);
                }
            }
        }
    }

    private void RandomInit(TileElement tile, int x, int y)
    {
        
        int randomElement = UnityEngine.Random.Range(0, tileElements.Length);
        tile.Init(tileElements[randomElement], x, y);
    }

    private void FindMoves()
    {
        moves.Clear();

        // Horizontal swaps
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width - 1; i++)
            {
                SwapTiles(i, j, i + 1, j);
                FindClusters();
                SwapTiles(i, j, i + 1, j); // Swap back

                // If there was a cluster so move is available 
                if (clusters.Count > 0)
                {
                    Move move = new Move(i, j, i + 1, j);
                    moves.Add(move);
                }
            }
        }

        // Vertical swaps
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height - 1; j++)
            {
                SwapTiles(i, j, i, j + 1);
                FindClusters();
                SwapTiles(i, j, i, j + 1); // Swap back

                // If there was a cluster so move is available 
                if (clusters.Count > 0)
                {
                    Move move = new Move(i, j, i, j + 1);
                    moves.Add(move);
                }
            }
        }
    }

    private void BreakClusters()
    {
        Debug.Log("BreakClusters");

        foreach (Cluster cluster in clusters)
        {
            // set all tiles in the cluster as empty
            for (int i = 0; i < cluster.length; i++)
            {
                if (cluster.horizontal)
                {
                    tiles[cluster.column + i, cluster.row].SetEmpty();

                }
                else
                {
                    tiles[cluster.column, cluster.row + i].SetEmpty();
                }
                OnClusterBreak?.Invoke(cluster.length);
            }
        }
        clusters.Clear();
        StartCoroutine(ShiftTiles());
    }

    private IEnumerator ShiftTiles()
    {
        int emptyCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j].Type == TileType.empty)
                {
                    emptyCount++;
                }
                else if (emptyCount > 0)
                {
                    Move move = new Move(i, j, i, j - emptyCount);
                    MoveTiles(move);
                    yield return new WaitUntil(() => tiles[i, j].Moving == false);
                }
            }
            emptyCount = 0;
        }

        FindClusters();
        if (clusters.Count > 0)
        {
            BreakClusters();
        }
        else
        {
            yield return new WaitForSeconds(tileGenerationDelay);
            GenerateNewTiles();
        }
    }

    private void GenerateNewTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i,j].Type == TileType.empty)
                {
                    RandomInit(tiles[i, j], i, j);
                }
            }
        }

        FindClusters();
        while (clusters.Count > 0)
        {
            BreakClusters();
            FindClusters();
        }

        FindMoves();
        if (moves.Count == 0)
        {
            OnNoMovesLeft?.Invoke();
        }
        else
        {
            OnReadyToMakeMove?.Invoke();
        }
        

    }
}
