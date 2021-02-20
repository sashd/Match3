using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    /*
     --------------------------------------------------------------------------
    ------------------ Õ≈ »—œŒÀ‹«”≈“—ﬂ ----------------------------------------
    ---------------------------------------------------------------------------
     */

    private void Start()
    {
        GenerateLevel();

        //ResolveClusters()

        /*
        if (clusters.Count == 0)
        {
            Debug.Log("There is no cluster, finding moves...");
            FindMoves();
        }
        else
        {
            RemoveClusters();
        }
        */

    }

    private void GenerateLevel()
    {
    }

    private void ResolveClusters()
    {
    }

    /*
    private void Swap(int x1, int y1, int x2, int y2)
    {
        var typeSwap = tiles[x1, y1].Type;
        //if (tiles[x2, y2].Type)

        tiles[x1, y1].Type = tiles[x2, y2].Type;
        tiles[x2, y2].Type = typeSwap;
    }

    private void FindMoves()
    {
        // Horizontal swaps
        for (int j = 0; j < fieldSize.y - 1; j++)
        {
            for (int i = 0; i < fieldSize.x - 1; i++)
            {
                Swap(i, j, i + 1, j);
                FindClusters();
                Swap(i, j, i + 1, j); // Swap back

                // If there was a cluster so move is available 
                if (clusters.Count > 0)
                {
                    Move move = new Move(i, j, i + 1, j);
                    moves.Add(move);
                    Debug.Log("move: (" + i + "," + j + ") -> (" + (i + 1) + "," + j + ") Horizontal swap");
                }
            }
        }
        
        // Vertical swaps
        for (int i = 0; i < fieldSize.x - 1; i++)
        {
            for (int j = 0; j < fieldSize.y - 1; j++)
            {
                Swap(i, j, i, j + 1);
                FindClusters();
                Swap(i, j, i, j + 1); // Swap back

                // If there was a cluster so move is available 
                if (clusters.Count > 0)
                {
                    Move move = new Move(i, j, i, j + 1);
                    moves.Add(move);
                    Debug.Log("move: (" + i + "," + j + ") -> (" + i + "," + (j + 1) + ") Vertical swap");
                }
            }
        }
        
        clusters.Clear();
    }

    private void ShiftTiles()
    {
        Debug.Log("Shift Tiles not implemented");
    }

    private void RemoveClusters()
    {
        
        for (int i = 0; i < clusters.Count; i++)
        {
            var cluster = clusters[i];
            Vector2 offset = new Vector2(0, 0);

            for (var j = 0; j < cluster.length; j++)
            {
                // ************** GOVNO CODE ********************

                // change the type
                tiles[i, j].Break();
                tiles[i, j].Type = (TileType)(-1);
                // ************** GOVNO CODE ********************


                if (cluster.horizontal)
                {
                    offset.x += 1;
                }
                else
                {
                    offset.y += 1;
                }
            }
        }

        for (int i = 0; i < fieldSize.x; i++)
        {
            int shift = 0;
            for(int j = fieldSize.y - 1; j >= 0; j--)
            {
                // ************** GOVNO CODE ********************
                if (tiles[i,j].Type == (TileType)(-1))
                {
                    shift += 1;
                    //tiles[i,j].
                }
            }
        }


    }


    */

}
