using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class BresenhamsLineAlgorithm : IFieldOfViewCalculator
{
    private readonly Tilemap _obstaclesTilemap;



    public BresenhamsLineAlgorithm(Tilemap obstaclesTilemap)
    {
        if (obstaclesTilemap == null)
        {
            throw new System.ArgumentNullException(nameof(obstaclesTilemap));
        }
        _obstaclesTilemap = obstaclesTilemap;
    }



    public void CalculateFieldOfView(Vector3Int origin, int rangeLimit, List<Vector3Int> fieldOfView)
    {
        fieldOfView.Add(origin);

        for (int x = origin.x - rangeLimit; x <= origin.x + rangeLimit; x += 1)
        {
            for (int y = origin.y - rangeLimit; y <= origin.y + rangeLimit; y += 1)
            {
                if (Mathf.Abs(x - origin.x) + Mathf.Abs(y - origin.y) <= rangeLimit)
                {
                    if (BresenhamLineOfSight(origin.x, origin.y, x, y))
                    {
                        fieldOfView.Add(new Vector3Int(x, y, 0));
                    }
                }
            }
        }
    }



    private bool BresenhamLineOfSight(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;

        while (x0 != x1 || y0 != y1)
        {
            if(_obstaclesTilemap.HasTile(new Vector3Int(x0, y0)) == true)
            {
                // Impassable cell found
                return false;
            }

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
        return true; // Line of sight is clear
    }
}