using UnityEngine;
using System;

public class TileElement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveSpeed;
    public TileType Type { get; set; }

    [HideInInspector] public Vector2Int indices;
    public bool Moving { get; private set; }

    private Vector3 target;
    private Animator animator;

    public static event Action<int, int> OnElementClick;

    private static readonly int appearTrigger = Animator.StringToHash("Appear");
    private static readonly int hintTrigger = Animator.StringToHash("Hint");

    public void Init(TileElementData data, int x, int y)
    {
        animator = GetComponent<Animator>();
        spriteRenderer.sprite = data.sprite;
        spriteRenderer.color = Color.white;
        Type = data.type;
        indices = new Vector2Int(x, y);

        animator.SetTrigger(appearTrigger);
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
        Type = TileType.Empty;
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

    public void Hint()
    {
        animator.SetTrigger(hintTrigger);
    }

    private void OnMouseDown()
    {
        if (Type == TileType.Empty)
            return;

        OnElementClick?.Invoke(indices.x, indices.y);
    }
}
