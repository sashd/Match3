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
    public bool freestanding = false;

    private bool highlighted = false;
    public static event Action<int, int> OnElementClick;


    public void Init(TileElementData data, int x, int y)
    {
        spriteRenderer.sprite = data.sprite;
        Type = data.type;
        indices = new Vector2Int(x, y);
    }

    public void SetEmpty()
    {
        Type = TileType.empty;
        spriteRenderer.color = Color.clear;

        // return to pool
        //Destroy(gameObject);
    }

    public void Move(Vector3 position, int x, int y)
    {
        StartCoroutine(MoveTo(position, moveSpeed));

        // Change indices info
        indices.x = x;
        indices.y = y;
    }

    public IEnumerator MoveTo(Vector3 destination, float speed)
    {
        while (transform.localPosition != destination)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, speed * Time.deltaTime);
            yield return null;
        }
        transform.localPosition = destination;
    }

    public void Highlight()
    {
        if (!highlighted)
        {
            spriteRenderer.color = Color.blue;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
        highlighted = !highlighted;
    }


    private void OnMouseDown()
    {
        Debug.Log(indices.x + "," + indices.y + " type: " + Type);

        if (Type == TileType.empty)
            return;

        OnElementClick?.Invoke(indices.x, indices.y);
        //Highlight();
    }
}
