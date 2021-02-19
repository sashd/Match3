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

}
