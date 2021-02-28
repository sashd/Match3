using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Level: MonoBehaviour
{
    private const int maxSize = 10;

    [Header("Level Size")]
    [Range(3, maxSize)] [SerializeField] private int width = 5;
    [Range(3, maxSize)] [SerializeField] private int height = 5;

    [Range(0.7f, 1.2f)] [SerializeField] public float spacing = 0.9f;

    [Header("Tiles on level")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private TileElementData[] tileElements;

    [Header("Timings")]
    [Min(0.1f)] [SerializeField] private float timeBeforeBreak = 0.2f;
    [Min(0.1f)] [SerializeField] private float timeBeforeGenerate = 0.2f;

    public event Action OnReadyToMakeMove;
    public event Action<int> OnMatchBreak;

    private TileElement[,] tiles = new TileElement[maxSize, maxSize];
    private List<Match> matches = new List<Match>();
    private List<Move> moves = new List<Move>();

    public void Generate()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tilePosition = new Vector2(i, j);
                GameObject element = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                element.transform.localPosition = new Vector2(i * spacing, j * spacing);
                TileElement tile = element.GetComponent<TileElement>();
                tiles[i, j] = tile;
            }
        }
        MoveGridToCenter();
        InitAll();
    }

    public IEnumerator MakeMove(Move move)
    {
        MoveTiles(move);

        yield return new WaitUntil(() => tiles[move.from.x, move.from.y].Moving == false);

        FindMatches();
        if (matches.Count > 0)
        {
            yield return new WaitForSeconds(timeBeforeBreak);
            BreakMatchTiles();
        }
        else
        {
            // Return back if there is no matches
            MoveTiles(move);
            OnReadyToMakeMove?.Invoke();
        }
    }

    public void ShowHint()
    {
        FindMoves();
        if (moves.Count > 0)
        {
            Move availableMove = moves[0];
            tiles[availableMove.from.x, availableMove.from.y].Hint();
            tiles[availableMove.to.x, availableMove.to.y].Hint();
        }
    }

    private void InitAll()
    {
        bool levelGenerated = false;
        while (!levelGenerated)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    RandomInit(tiles[i, j], i, j);
                }
            }
            FindMatches();
            while (matches.Count > 0)
            {
                ReInitMatchTiles();
                FindMatches();
            }

            FindMoves();
            if (moves.Count > 0)
            {
                levelGenerated = true;
            }
            else
            {
                continue;
            }
        }
        OnReadyToMakeMove?.Invoke();
    }

    private void MoveTiles(Move move)
    {
        tiles[move.from.x, move.from.y].Move(tiles[move.to.x, move.to.y].transform.localPosition, move.to.x, move.to.y);
        tiles[move.to.x, move.to.y].Move(tiles[move.from.x, move.from.y].transform.localPosition, move.from.x, move.from.y);
        SwapTiles(move.from.x, move.from.y, move.to.x, move.to.y);
    }

    private void SwapTiles(int x1, int y1, int x2, int y2)
    {
        TileElement temp = tiles[x1, y1];
        tiles[x1, y1] = tiles[x2, y2];
        tiles[x2, y2] = temp;
    }

    private void FindMatches()
    {
        matches.Clear();

        for (int j = 0; j < height; j++)
        {
            int matchLength = 1;
            for (int i = 0; i < width; i++)
            {
                bool matchChecked = false;
                if (i == width - 1)
                {
                    matchChecked = true;
                }
                else
                {
                    if (tiles[i, j].Type == tiles[i + 1, j].Type && tiles[i, j].Type != TileType.Empty)
                    {
                        matchLength += 1;
                    }
                    else
                    {
                        matchChecked = true;
                    }
                }

                if (matchChecked)
                {
                    if (matchLength >= 3)
                    {
                        Match match = new Match(i + 1 - matchLength, j, matchLength, true);
                        matches.Add(match);
                    }
                    matchLength = 1;
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            int matchLength = 1;
            for (int j = 0; j < height; j++)
            {
                bool matchChecked = false;
                if (j == height - 1)
                {
                    matchChecked = true;
                }
                else
                {
                    if (tiles[i, j].Type == tiles[i, j + 1].Type && tiles[i, j].Type != TileType.Empty)
                    {
                        matchLength += 1;
                    }
                    else
                    {
                        matchChecked = true;
                    }
                }

                if (matchChecked)
                {
                    if (matchLength >= 3)
                    {
                        Match match = new Match(i, j + 1 - matchLength, matchLength, false);
                        matches.Add(match);
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

    private void ReInitMatchTiles()
    {
        foreach (Match match in matches)
        {
            for (int i = 0; i < match.length; i++)
            {
                if (match.horizontal)
                {
                    RandomInit(tiles[match.column + i, match.row], match.column + i, match.row);
                }
                else
                {
                    RandomInit(tiles[match.column, match.row + i], match.column, match.row + i);
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
                FindMatches();
                SwapTiles(i, j, i + 1, j); // Swap back

                // If there was a match so move is available 
                if (matches.Count > 0)
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
                FindMatches();
                SwapTiles(i, j, i, j + 1);

                if (matches.Count > 0)
                {
                    Move move = new Move(i, j, i, j + 1);
                    moves.Add(move);
                }
            }
        }
    }

    private void BreakMatchTiles()
    {
        foreach (Match match in matches)
        {
            // Set all tiles in the match as empty
            for (int i = 0; i < match.length; i++)
            {
                if (match.horizontal)
                {
                    tiles[match.column + i, match.row].SetEmpty();
                }
                else
                {
                    tiles[match.column, match.row + i].SetEmpty();
                }
            }
            OnMatchBreak?.Invoke(match.length);
        }
        StartCoroutine(ShiftTiles());
    }

    private IEnumerator ShiftTiles()
    {
        int emptyCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j].Type == TileType.Empty)
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

        FindMatches();
        if (matches.Count > 0)
        {
            yield return new WaitForSeconds(timeBeforeBreak);
            BreakMatchTiles();
        }
        else
        {
            yield return new WaitForSeconds(timeBeforeGenerate);
            StartCoroutine(FillEmptyTiles());
        }
    }

    private IEnumerator FillEmptyTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j].Type == TileType.Empty)
                {
                    RandomInit(tiles[i, j], i, j);
                }
            }
        }

        FindMatches();
        if (matches.Count > 0)
        {
            yield return new WaitForSeconds(timeBeforeBreak);
            BreakMatchTiles();
            yield break;
        }

        FindMoves();
        if (moves.Count > 0)
        {
            OnReadyToMakeMove?.Invoke();
        }
        else
        {
            Debug.Log("No moves! Generate again");
            yield return new WaitForSeconds(timeBeforeGenerate);
            InitAll();
        }
    }
}
