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
                Vector2 tilePosition = new Vector2(i, j);
                // get from pool 
                GameObject element = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                element.transform.position = new Vector2(i * spacing, j * spacing);
                int randomElement = Random.Range(0, tileElements.Length);
                tiles[i, j] = element.GetComponent<TileElement>();
                tiles[i, j].Init(tileElements[randomElement]);
            }
        }
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
                    tiles[cluster.column + i, cluster.row].Init(tileElements[randomElement]);
                }
                else
                {
                    tiles[cluster.column, cluster.row + i].Init(tileElements[randomElement]);
                }
            }

        }

    }



}
