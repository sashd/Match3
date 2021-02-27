using System;
using UnityEngine;
using Moves;

public class GameInput : MonoBehaviour
{
    public event Action<Move> OnMoveMade;

    private int clickCount = 0;
    private Move move;

    private void Awake()
    {
        TileElement.OnElementClick += OnTileClick;
    }

    private void OnTileClick(int x, int y)
    {
        if (PauseMenu.isPaused)
            return;

        if (clickCount == 0)
        {
            move.from = new Vector2Int(x, y);
            clickCount = 1;
        }
        else
        {
            move.to = new Vector2Int(x, y);
            clickCount = 0;

            // Check if the tiles are not adjacent
            Vector2Int dif = move.to - move.from;
            if (dif.magnitude == 1)
            {
                OnMoveMade?.Invoke(move);
            }
        }
    }

    private void OnDestroy()
    {
        TileElement.OnElementClick -= OnTileClick;
    }
}
