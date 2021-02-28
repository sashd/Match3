using UnityEngine;

public enum TileType
{
    Empty,
    Apple,
    Milk,
    GreenGuy,
    Orange,
    Bread
}

[CreateAssetMenu(fileName = "New Tile Element", menuName = "Tile Element")]
public class TileElementData : ScriptableObject
{
    public Sprite sprite;
    public TileType type;
}
