using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public static class IntersectingCoordinates
{
    public static void Compute(Vector2Int start, Vector2Int end, bool ignoreObstactles, List<Vector2Int> lineCoordinates)
    {
        int deltaX = end.x - start.x;
        int deltaY = end.y - start.y;

        int stepX = Mathf.Clamp(Mathf.RoundToInt(Mathf.Sign(deltaX)), -1, 1);
        int stepY = Mathf.Clamp(Mathf.RoundToInt(Mathf.Sign(deltaY)), -1, 1);

        Vector2Int currentGridPosition = start;

        MapManager mapManager = MapManager.Instance;
        Tilemap obstacleMap = mapManager.ObstacleMap;

        Vector3Int obstaclePosition = new Vector3Int(currentGridPosition.x, currentGridPosition.y, 0);

        while (currentGridPosition != end)
        {
            obstaclePosition.x = currentGridPosition.x;
            obstaclePosition.y = currentGridPosition.y;
            if (ignoreObstactles == true || obstacleMap.HasTile(obstaclePosition) == false)
            {
                lineCoordinates.Add(currentGridPosition);
            }

            if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            {
                currentGridPosition.x += stepX;
                deltaX = end.x - currentGridPosition.x;
            }   
            else
            {
                currentGridPosition.y += stepY;
                deltaY = end.y - currentGridPosition.y;
            }
        }

        obstaclePosition.x = end.x;
        obstaclePosition.y = end.y;
        if (ignoreObstactles == true || obstacleMap.HasTile(obstaclePosition) == false)
        {
            lineCoordinates.Add(end);
        }
    }
}