using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public static class AStar
{
    public static void FindPath(Vector2Int start, Vector2Int end, bool ignoreObstacles, List<Vector2Int> lineCoordinates)
    {
        MapManager mapManager = MapManager.Instance;
        Tilemap floorMap = mapManager.FloorMap;
        Tilemap obstacleMap = mapManager.ObstacleMap;

        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        PriorityQueue<PathNode> openSet = new PriorityQueue<PathNode>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();

        openSet.Enqueue(new PathNode(start, 0, Heuristic(start, end)));

        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            PathNode current = openSet.Dequeue();

            if (current.position == end)
            {
                ReconstructPath(cameFrom, current.position, lineCoordinates);
                return;
            }

            closedSet.Add(current.position);

            foreach (Vector2Int neighbor in GetNeighbors(current.position))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGScore = gScore[current.position] + GetMoveCost(current.position, neighbor, floorMap, obstacleMap, ignoreObstacles);

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current.position;
                    gScore[neighbor] = tentativeGScore;

                    float fScore = tentativeGScore + Heuristic(neighbor, end);
                    openSet.Enqueue(new PathNode(neighbor, tentativeGScore, fScore));
                }
            }
        }
    }



    private static float GetMoveCost(Vector2Int from, Vector2Int to, Tilemap floorMap, Tilemap obstacleMap, bool ignoreObstacles)
    {
        if(ignoreObstacles == false && obstacleMap.HasTile(new Vector3Int(to.x, to.y, 0)) == true)
        {
            return float.MaxValue;
        }

        float cost = 100f;

        foreach(Vector2Int position in new Vector2Int[] { from, to})
        {
            if (floorMap.HasTile(new Vector3Int(position.x, position.y, 0)) == true)
            {
                cost -= 1f;
            }

            if (obstacleMap.HasTile(new Vector3Int(position.x, position.y, 0)) == true)
            {
                cost += 5f;
            }
        }

        return cost;
    }



    private static List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Add all valid neighboring positions here
        neighbors.Add(new Vector2Int(position.x, position.y + 1));
        neighbors.Add(new Vector2Int(position.x, position.y - 1));
        neighbors.Add(new Vector2Int(position.x + 1, position.y));
        neighbors.Add(new Vector2Int(position.x - 1, position.y));

        return neighbors;
    }



    private static float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }



    private static void ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current, List<Vector2Int> lineCoordinates)
    {
        lineCoordinates.Clear();

        while (cameFrom.ContainsKey(current))
        {
            lineCoordinates.Add(current);
            current = cameFrom[current];
        }

        lineCoordinates.Reverse();
    }



    private struct PathNode : System.IComparable<PathNode>
    {
        public Vector2Int position;
        public float gScore;
        public float fScore;

        public PathNode(Vector2Int position, float gScore, float fScore)
        {
            this.position = position;
            this.gScore = gScore;
            this.fScore = fScore;
        }

        public int CompareTo(PathNode other)
        {
            return fScore.CompareTo(other.fScore);
        }
    }



    private class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> heap;

        public int Count => heap.Count;

        public PriorityQueue()
        {
            heap = new List<T>();
        }

        public void Enqueue(T item)
        {
            heap.Add(item);
            int currentIndex = heap.Count - 1;

            while (currentIndex > 0)
            {
                int parentIndex = (currentIndex - 1) / 2;

                if (heap[currentIndex].CompareTo(heap[parentIndex]) >= 0)
                    break;

                // Swap the elements
                T temp = heap[currentIndex];
                heap[currentIndex] = heap[parentIndex];
                heap[parentIndex] = temp;

                currentIndex = parentIndex;
            }
        }

        public T Dequeue()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Priority queue is empty.");

            T topItem = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            int currentIndex = 0;
            while (true)
            {
                int leftChildIndex = currentIndex * 2 + 1;
                int rightChildIndex = currentIndex * 2 + 2;
                int smallestChildIndex = -1;

                if (leftChildIndex < heap.Count)
                {
                    smallestChildIndex = leftChildIndex;

                    if (rightChildIndex < heap.Count && heap[rightChildIndex].CompareTo(heap[leftChildIndex]) < 0)
                    {
                        smallestChildIndex = rightChildIndex;
                    }
                }

                if (smallestChildIndex == -1 || heap[currentIndex].CompareTo(heap[smallestChildIndex]) <= 0)
                    break;

                // Swap the elements
                T temp = heap[currentIndex];
                heap[currentIndex] = heap[smallestChildIndex];
                heap[smallestChildIndex] = temp;

                currentIndex = smallestChildIndex;
            }

            return topItem;
        }
    }
}