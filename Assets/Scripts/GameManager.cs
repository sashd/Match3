using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clusters;
using Moves;

public enum GameState
{
    moving,
    idle
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Level level;

    [SerializeField] private float hintTime = 5f;

    public static GameState gameState;

    private const int maxSize = 20;
    private TileElement[,] tiles = new TileElement[maxSize, maxSize];

    private List<Cluster> clusters = new List<Cluster>();
    private List<Move> moves = new List<Move>();

    private void Start()
    {
        bool levelGenerated = false;
        while (!levelGenerated)
        {
            level.Generate(tiles);
            FindClusters();
            while (clusters.Count > 0)
            {
                level.RemoveClusters(tiles, clusters);
                FindClusters();
            }

            FindMoves();
            if (moves.Count > 0)
            {
                levelGenerated = true;
                level.MoveGridToCenter();
                Debug.Log("Level has no clusters and " + moves.Count + " moves");
            }
            else
            {
                level.DestroyAll();
            }

            moves.Clear();
            clusters.Clear();
        }

        // ------------ TEST ----------------

    }

    private void FindClusters()
    {
        clusters.Clear();

        // Find Horizontal clusters
        for (int j = 0; j < level.fieldSize.y; j++)
        {
            int matchLength = 1;
            for (int i = 0; i < level.fieldSize.x; i++)
            {
                bool checkCluster = false;

                if (i == level.fieldSize.x - 1)
                {
                    checkCluster = true;
                }
                else
                {
                    if (tiles[i, j].Type == tiles[i + 1, j].Type && tiles[i, j].Type != TileType.empty)
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
                        // Cluster found
                        Cluster cluster = new Cluster(i + 1 - matchLength, j, matchLength, true);
                        clusters.Add(cluster);
                        //Debug.Log("horizontal cluster found: (" + (i + 1 - matchLength) + "," + j + ") length: " + matchLength);

                    }
                    matchLength = 1;
                }

            }
        }

        // Find Vertical clusters
        for (int i = 0; i < level.fieldSize.x; i++)
        {
            int matchLength = 1;

            for (int j = 0; j < level.fieldSize.y; j++)
            {
                bool checkCluster = false;

                if (j == level.fieldSize.y - 1)
                {
                    checkCluster = true;
                }
                else
                {
                    if (tiles[i, j].Type == tiles[i, j + 1].Type && tiles[i, j].Type != TileType.empty)
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
                        // Cluster found
                        Cluster cluster = new Cluster(i, j + 1 - matchLength, matchLength, false);
                        clusters.Add(cluster);
                        //Debug.Log("vertical cluster found: (" + i + "," + (j + 1 - matchLength) + ") length: " + matchLength);

                    }
                    matchLength = 1;
                }

            }
        }
    }

    private void Swap(int x1, int y1, int x2, int y2)
    {
        TileElement temp = tiles[x1, y1];
        tiles[x1, y1] = tiles[x2, y2];
        tiles[x2, y2] = temp;
    }

    private void FindMoves()
    {
        Debug.Log("FindMoves");


        // Horizontal swaps
        for (int j = 0; j < level.fieldSize.y; j++)
        {
            for (int i = 0; i < level.fieldSize.x - 1; i++)
            {
                Swap(i, j, i + 1, j);
                FindClusters();
                Swap(i, j, i + 1, j); // Swap back

                // If there was a cluster so move is available 
                if (clusters.Count > 0)
                {
                    Move move = new Move(i, j, i + 1, j);
                    moves.Add(move);
                    //Debug.Log("move: (" + i + "," + j + ") -> (" + (i + 1) + "," + j + ") Horizontal swap");
                }
            }
        }

        // Vertical swaps
        for (int i = 0; i < level.fieldSize.x; i++)
        {
            for (int j = 0; j < level.fieldSize.y - 1; j++)
            {
                Swap(i, j, i, j + 1);
                FindClusters();
                Swap(i, j, i, j + 1); // Swap back

                // If there was a cluster so move is available 
                if (clusters.Count > 0)
                {
                    Move move = new Move(i, j, i, j + 1);
                    moves.Add(move);
                    //Debug.Log("move: (" + i + "," + j + ") -> (" + i + "," + (j + 1) + ") Vertical swap");
                }
            }
        }
    }

    public IEnumerator MakeMove(int x1, int y1, int x2, int y2)
    {
        Debug.Log("MakeMove");

        tiles[x1, y1].Move(tiles[x2, y2].transform.localPosition, x2, y2);
        tiles[x2, y2].Move(tiles[x1, y1].transform.localPosition, x1, y1);
        Swap(x1, y1, x2, y2);

        FindClusters();
        yield return new WaitUntil(() => tiles[x1, y1].move == false);

        if (clusters.Count > 0)
        {

            // If after the move cluster was formed
            BreakClusters();
        }
        else
        {

            // Return tiles to their previous position
            tiles[x1, y1].Move(tiles[x2, y2].transform.localPosition, x2, y2);
            tiles[x2, y2].Move(tiles[x1, y1].transform.localPosition, x1, y1);
            Swap(x1, y1, x2, y2);
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
            }
        }
        clusters.Clear();
        StartCoroutine(ShiftTiles());
    }

    private IEnumerator ShiftTiles()
    {
        Debug.Log("Shift tiles");
        int emptyCount = 0;

        for (int i = 0; i < level.fieldSize.x; i++)
        {
            for (int j = 0; j < level.fieldSize.y; j++)
            {
                if (tiles[i,j].Type == TileType.empty)
                {
                    emptyCount++;
                }
                else if (emptyCount > 0)
                {
                    tiles[i, j].Move(tiles[i, j - emptyCount].transform.localPosition, i, j - emptyCount);
                    tiles[i, j - emptyCount].Move(tiles[i, j].transform.localPosition, i, j);
                    Swap(i, j, i, j - emptyCount);
                }
                yield return new WaitUntil(() => tiles[i, j].move == false);

            }

            emptyCount = 0;
        }
        FindClusters();
        if (clusters.Count > 0)
            BreakClusters();
    }
}
