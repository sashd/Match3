using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileElement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private TileType type;

    public TileType Type
    {
        get 
        { 
            return type; 
        }

        set
        {
            type = value;
        }

    }

    public void Init(TileElementData data)
    {
        spriteRenderer.sprite = data.sprite;
        type = data.type;
        
    }

    public void Break()
    {
        Debug.Log("i am break");
        type = TileType.empty;

        Color color = spriteRenderer.color;
        color.a = 0.3f;
        spriteRenderer.color = color;
        //Destroy(gameObject);
    }

}
