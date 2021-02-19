using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    apple,
    milk,
    greenGuy,
    orange,
    bread
}

[CreateAssetMenu(fileName = "New Tile Element", menuName = "Tile Element")]
public class TileElementData : ScriptableObject
{
    public Sprite sprite;
    public TileType type;
}
