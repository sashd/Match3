using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clusters;

public class Level: MonoBehaviour
{
    public Vector2Int fieldSize = new Vector2Int(5, 5);
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private TileElementData[] tileElements;

    [Range(0.7f, 1.2f)]
    [SerializeField] public float spacing = 1;

    public void Generate(TileElement[,] tiles)
    {
        for (int i = 0; i < fieldSize.x; i++)
        {
            for (int j = 0; j < fieldSize.y; j++)
            {
                tiles[i, j] = PlaceRandomTile(i, j);
            }
        }
    }

    public TileElement PlaceRandomTile(int x, int y)
    {
        Vector2 tilePosition = new Vector2(x, y);
        GameObject element = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
        element.transform.localPosition = new Vector2(x * spacing, y * spacing);
        int randomElement = Random.Range(0, tileElements.Length);
        TileElement tile = element.GetComponent<TileElement>();
        tile.Init(tileElements[randomElement], x, y);
        return tile;
    }

    public void DestroyAll()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void MoveGridToCenter()
    {
        Vector2 gridSize = new Vector2(fieldSize.x * spacing, fieldSize.y * spacing);
        transform.position = new Vector2(-gridSize.x / 2 + spacing / 2, -(gridSize.y / 2 - spacing / 2));
    }

    public void RemoveClusters(TileElement[,] tiles, List<Cluster> clusters)
    {
        foreach(Cluster cluster in clusters)
        {
            // Re-Init all tiles in the cluster
            for (int i = 0; i < cluster.length; i++)
            {
                int randomElement = Random.Range(0, tileElements.Length);
                if (cluster.horizontal)
                {
                    tiles[cluster.column + i, cluster.row].Init(tileElements[randomElement], cluster.column + i, cluster.row);
                }
                else
                {
                    tiles[cluster.column, cluster.row + i].Init(tileElements[randomElement], cluster.column, cluster.row + i);
                }
            }
        }
    }
}
