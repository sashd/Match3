using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileElement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveSpeed;

    public TileType Type { get; set; }
    [HideInInspector]
    public Vector2Int indices;

    public void Init(TileElementData data, int x, int y)
    {
        spriteRenderer.sprite = data.sprite;
        Type = data.type;
        indices = new Vector2Int(x, y);
    }

    public void Break()
    {
        Type = TileType.empty;

        // return to pool
        Destroy(gameObject);
    }

    public void Move(Vector3 position, int x, int y)
    {
        StartCoroutine(MoveFromTo(transform.position, position, moveSpeed));

        // Change indices info
        indices.x = x;
        indices.y = y;
    }

    private IEnumerator MoveFromTo(Vector3 from, Vector3 to, float speed)
    {
        float step = (speed / (from - to).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step;
            transform.position = Vector3.Lerp(from, to, t); 
            yield return new WaitForFixedUpdate();         
        }
        transform.position = to;
    }

    private void OnMouseDown()
    {
        Debug.Log("(" + indices.x + "," + indices.y + ")");
    }
}
