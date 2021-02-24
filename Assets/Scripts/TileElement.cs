using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileElement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveSpeed;

    public TileType Type { get; set; }

    [HideInInspector]
    public Vector2Int indices;
    public bool Moving { get; private set; }

    private Vector3 target;

    public static event Action<int, int> OnElementClick;

    public void Init(TileElementData data, int x, int y)
    {
        Debug.Log("init");
        spriteRenderer.sprite = data.sprite;
        spriteRenderer.color = new Color(1,1,1,1);

        Type = data.type;
        indices = new Vector2Int(x, y);
    }

    private void Update()
    {
        if (!Moving)
            return;

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, moveSpeed * Time.deltaTime);
        if (transform.localPosition == target)
        {
            Moving = false;
        }
    }

    public void SetEmpty()
    {
        Type = TileType.empty;
        spriteRenderer.color = Color.clear;
    }



    public void Move(Vector3 position, int x, int y)
    {
        Moving = true;
        target = position;

        // Change indices info
        indices.x = x;
        indices.y = y;
    }

    public void Highlight(bool on)
    {
        if (on)
        {
            spriteRenderer.color = Color.blue;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }


    private void OnMouseDown()
    {
        Debug.Log(indices.x + "," + indices.y + " type: " + Type);

        if (Type == TileType.empty)
            return;

        OnElementClick?.Invoke(indices.x, indices.y);
        //Highlight(true);
    }
}
