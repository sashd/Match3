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
    private Animator animator;

    public static event Action<int, int> OnElementClick;



    public void Init(TileElementData data, int x, int y)
    {
        animator = GetComponent<Animator>();

        spriteRenderer.sprite = data.sprite;
        spriteRenderer.color = Color.white;
        animator.SetTrigger("Appear");

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
        // Set Sprite alpha to 0
        spriteRenderer.color = Color.clear;
    }

    public void Move(Vector3 target, int x, int y)
    {
        Moving = true;
        this.target = target;

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
        if (Type == TileType.empty)
            return;

        OnElementClick?.Invoke(indices.x, indices.y);
        //Highlight(true);
    }
}
