using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public GameManager gameManager;
    private int clickCount = 0;
    private Vector2Int first;
    private Vector2Int second;

    private void Awake()
    {
        TileElement.OnElementClick += TileElement_OnElementClick;
    }

    private void OnDestroy()
    {
        TileElement.OnElementClick -= TileElement_OnElementClick;
    }

    private void TileElement_OnElementClick(int x, int y)
    {
        if (clickCount == 0)
        {
            first = new Vector2Int(x, y);
            clickCount = 1;
        }
        else
        {
            second = new Vector2Int(x, y);
            clickCount = 0;

            // if the tiles are adjacent
            Vector2Int dif = first - second;
            if (dif.magnitude == 1)
            {
                StartCoroutine(gameManager.MakeMove(first.x, first.y, second.x, second.y));
            }
        }


    }
}
