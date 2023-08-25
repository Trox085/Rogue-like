using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;



public static class BreadthFirstSearch
{
    public static void FindPath(Vector2Int start, Vector2Int end, bool ignoreObstacles, List<Vector2Int> pathCoordinates)
    {
        MapManager mapManager = MapManager.Instance;
        Tilemap obstacleMap = mapManager.ObstacleMap;

        pathCoordinates.Clear();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> parents = new Dictionary<Vector2Int, Vector2Int>();
        parents[start] = Vector2Int.zero;

        bool pathFound = false;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == end)
            {
                pathFound = true;
                break;
            }

            List<Vector2Int> neighbors = GetNeighbors(current);
            foreach (Vector2Int neighbor in neighbors)
            {
                if (parents.ContainsKey(neighbor) == true)
                {
                    continue;
                }

                if (ignoreObstacles == false && IsObstructed(obstacleMap, neighbor) == true)
                {
                    continue;
                }

                queue.Enqueue(neighbor);
                parents[neighbor] = current;
            }
        }

        if(pathFound == false)
        {
            Debug.Log($"No path found between {start} and {end}.");
            return;
        }

        Vector2Int traceBack = end;
        while (traceBack != start)
        {
            pathCoordinates.Insert(0, traceBack);
            traceBack = parents[traceBack];
        }
        pathCoordinates.Insert(0, start);
    }



    private static bool IsObstructed(Tilemap obstacleMap, Vector2Int position)
    {
        Vector3Int gridPosition = new Vector3Int(position.x, position.y, 0);
        return obstacleMap.HasTile(gridPosition);
    }



    private static void LoadNeighbors(Vector2Int position, List<Vector2Int> neighbors)
    {
        neighbors.Clear();
        neighbors.Add(new Vector2Int(position.x + 1, position.y));
        neighbors.Add(new Vector2Int(position.x - 1, position.y));
        neighbors.Add(new Vector2Int(position.x, position.y + 1));
        neighbors.Add(new Vector2Int(position.x, position.y - 1));
    }



    private static List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        return new List<Vector2Int>
        {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1)
        };
    }
}