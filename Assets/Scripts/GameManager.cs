using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clusters;
using Moves;
using UnityEngine.SceneManagement;

public enum GameState
{

}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Level level;

    [SerializeField] private float hintTime = 5f;

    private const int maxSize = 20;
    private TileElement[,] tiles = new TileElement[maxSize, maxSize];

    private List<Cluster> clusters = new List<Cluster>();
    private List<Cluster> emptyClusters = new List<Cluster>(); 
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
                level.ReInitElements(tiles, clusters);
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
                bool clusterChecked = false;
                if (i == level.fieldSize.x - 1)
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
        for (int i = 0; i < level.fieldSize.x; i++)
        {
            int matchLength = 1;
            for (int j = 0; j < level.fieldSize.y; j++)
            {
                bool clusterChecked = false;
                if (j == level.fieldSize.y - 1)
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

    private void FindEmptyClusters()
    {
        Debug.Log("FindEmptyClusters");
        emptyClusters.Clear();

        for (int i = 0; i < level.fieldSize.x; i++)
        {
            int length = 0;
            for (int j = 0; j < level.fieldSize.y; j++)
            {
                bool clusterChecked = false;
                if (j == level.fieldSize.y - 1)
                {
                    clusterChecked = true;
                    if (tiles[i,j].Type == TileType.empty)
                        length += 1;
                }
                else
                {
                    if (tiles[i, j].Type == tiles[i, j + 1].Type && tiles[i, j].Type == TileType.empty)
                    {
                        length += 1;
                    }
                    else
                    {
                        clusterChecked = true;
                    }
                }

                if (clusterChecked)
                {
                    if (length > 0)
                    {
                        // Cluster found
                        Cluster cluster = new Cluster(i, (j + 1 - length), length, false);
                        emptyClusters.Add(cluster);
                        Debug.Log("empty vertical cluster: " + i + "," + (j + 1 - length) + " len: " + length);
                    }
                    length = 0;
                }
            }
        }

    }

    private void SwapTiles(int x1, int y1, int x2, int y2)
    {
        TileElement temp = tiles[x1, y1];
        tiles[x1, y1] = tiles[x2, y2];
        tiles[x2, y2] = temp;
    }

    private void FindMoves()
    {
        Debug.Log("FindMoves");
        moves.Clear();

        // Horizontal swaps
        for (int j = 0; j < level.fieldSize.y; j++)
        {
            for (int i = 0; i < level.fieldSize.x - 1; i++)
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
        for (int i = 0; i < level.fieldSize.x; i++)
        {
            for (int j = 0; j < level.fieldSize.y - 1; j++)
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

    private void MoveTiles(int x1, int y1, int x2, int y2)
    {
        tiles[x1, y1].Move(tiles[x2, y2].transform.localPosition, x2, y2);
        tiles[x2, y2].Move(tiles[x1, y1].transform.localPosition, x1, y1);
        SwapTiles(x1, y1, x2, y2);
    }

    public IEnumerator MakeMove(int x1, int y1, int x2, int y2)
    {
        MoveTiles(x1, y1, x2, y2);
        FindClusters();
        yield return new WaitUntil(() => tiles[x1, y1].Moving == false);

        if (clusters.Count > 0)
        {
            // If after the move cluster was formed
            BreakClusters();
        }
        else
        {
            // Return tiles to their previous position
            MoveTiles(x1, y1, x2, y2);
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
                    MoveTiles(i, j, i, j - emptyCount);
                }
                yield return new WaitUntil(() => tiles[i, j].Moving == false);

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
            StartCoroutine(GenerateNewTiles());
        }
    }
    private IEnumerator GenerateNewTiles()
    {
        Debug.Log("GenerateNewTiles");

        FindEmptyClusters();
        if (emptyClusters.Count > 0)
        {
            yield return new WaitForSeconds(0.3f);
            level.ReInitElements(tiles, emptyClusters);

            FindClusters();
            if (clusters.Count > 0)
            {
                BreakClusters();
            }
            FindMoves();
            if (moves.Count == 0)
            {
                Debug.Log("No moves! Restart");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.Log("(generatenewtiles) avalable move: " + moves[0].Info);
            }
        }

    }
}
