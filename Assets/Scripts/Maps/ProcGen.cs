using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;



internal sealed class ProcGen
{
    /// <summary>
    /// Generate a new dungeon map.
    /// </summary>
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, List<RectangularRoom> rooms)
    {
        MapManager mapManager = MapManager.Instance;

        Tilemap obstacleMap = mapManager.ObstacleMap;
        Tilemap floorMap = mapManager.FloorMap;

        TileBase floorTile = mapManager.FloorTile;
        TileBase wallTile = mapManager.WallTile;



        // Generate the rooms.
        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
            int roomHeight = Random.Range(roomMinSize, roomMaxSize);

            int roomX = Random.Range(0, mapWidth - roomWidth - 1);
            int roomY = Random.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight, rooms.Count);

            // Check if this room intersects with any other rooms
            if (newRoom.Overlaps(rooms) == true)
            {
                continue;
            }


            // If there are no intersections then the room is valid.
            Debug.Log($"Generating room #{rooms.Count + 1}.");

            // Dig out this room's inner area and build the walls.
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        // Set the wall tile
                        if (SetWallTileIfEmpty(new Vector3Int(x, y, 0), floorMap, obstacleMap, wallTile) == true)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y, 0), floorMap, obstacleMap, floorTile);
                    }
                }
            }

            if (rooms.Count > 0)
            {
                // Dig out a tunnel between this room and the previous one.
                Debug.Log($"Tunneling between room #{rooms.Count} and #{rooms.Count + 1}.");
                TunnelBetween(rooms[rooms.Count - 1], newRoom);
            }

            rooms.Add(newRoom);
        }


        
        if (rooms.Count > 0)
        {
            // Place the player in a random room.

            int startingRoomIndex = Random.Range(0, rooms.Count - 1);
            RectangularRoom startingRoom = rooms[startingRoomIndex];
            Vector2Int centerOfStartingRoom = startingRoom.Center();
            mapManager.CreatePlayer(centerOfStartingRoom);
        }
    }



    private Color GenerateRandomBlackContrastingColor()
    {
        Color neonColor = Color.black;
        float maxAttempts = 1000;  // Maximum attempts to find a suitable color
        int attempts = 0;

        while (IsNeonContrasting(neonColor, Color.black) == false && attempts < maxAttempts)
        {
            neonColor = new Color(Random.value, Random.value, Random.value);
            attempts++;
        }

        return neonColor;
    }



    private bool IsNeonContrasting(Color color1, Color color2)
    {
        float luminanceThreshold = 0.5f;  // Adjust as needed

        float luminance1 = color1.r * 0.299f + color1.g * 0.587f + color1.b * 0.114f;
        float luminance2 = color2.r * 0.299f + color2.g * 0.587f + color2.b * 0.114f;

        return Mathf.Abs(luminance1 - luminance2) >= luminanceThreshold;
    }



    private void SetFloorTile(Vector3Int position, Tilemap floorMap, Tilemap obstacleMap, TileBase floorTile)
    {
        SetFloorTile(position, floorMap, obstacleMap, floorTile, Color.grey);
    }



    private void SetFloorTile(Vector3Int position, Tilemap floorMap, Tilemap obstacleMap, TileBase floorTile, Color color)
    {
        // Clear any obstacles.
        TileBase obstacleTile = obstacleMap.GetTile(position);
        if (obstacleTile != null)
        {
            obstacleMap.SetTile(position, null);
        }

        // Set the floor tile
        floorMap.SetTile(position, floorTile);
        floorMap.SetTileFlags(position, TileFlags.None);
        floorMap.SetColor(position, color);
    }



    /// <summary>
    /// Return 
    /// </summary>
    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
    {
        MapManager mapManager = MapManager.Instance;

        Tilemap obstacleMap = mapManager.ObstacleMap;
        Tilemap floorMap = mapManager.FloorMap;

        TileBase floorTile = mapManager.FloorTile;
        TileBase wallTile = mapManager.WallTile;


        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();

        List<Vector2Int> tunnelCoords = new List<Vector2Int>();

        float randomTunnelValue = Random.value;
        if(randomTunnelValue < 0.25f)
        {
            Debug.Log("Bresenham Line");

            // Create an L-shaped tunnel between the two points.
            Vector2Int tunnelCorner;

            if (Random.value < 0.5f)
            {
                // Move horizontally, then vertically.
                tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
            }
            else
            {
                // Move vertically, then horizontally.
                tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
            }

            BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
            BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);
        }
        else
        {
            // Create a direct connection between the two points.

            if (randomTunnelValue < 0.50f)
            {
                Debug.Log("Breadth First Search");
                BreadthFirstSearch.FindPath(oldRoomCenter, newRoomCenter, true, tunnelCoords);
            }
            else if (randomTunnelValue < 0.75f)
            {
                Debug.Log("Intersecting Coordinates");
                IntersectingCoordinates.Compute(oldRoomCenter, newRoomCenter, true, tunnelCoords);
            }
            else
            {
                Debug.Log("A*");
                AStar.FindPath(oldRoomCenter, newRoomCenter, true, tunnelCoords);
            }
        }


        StringBuilder sb = new StringBuilder();
        foreach (Vector2Int position in tunnelCoords)
        {
            if (sb.Length > 0)
            {
                sb.Append(" -> ");
            }
            sb.Append($"{position}");
        }
        Debug.Log(sb.ToString());



        // Set the tiles for this tunnel.
        //for (int i = 0; i < tunnelCoords.Count; i++)
        //{
        //    Vector2Int tunnelCoord = tunnelCoords[i];

        //    SetFloorTile(new Vector3Int(tunnelCoord.x, tunnelCoord.y, 0), floorMap, obstacleMap, floorTile);
        //}
        //tunnelCoords.Sort(delegate (Vector2Int a, Vector2Int b)
        //{
        //    int compareValue = a.x.CompareTo(b.x);
        //    if (compareValue != 0)
        //    {
        //        return compareValue;
        //    }

        //    compareValue = a.y.CompareTo(b.y);
        //    return compareValue;
        //});

        Color tunnelColor = GenerateRandomBlackContrastingColor();

        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            Vector2Int tunnelCoord = tunnelCoords[i];

            SetFloorTile(new Vector3Int(tunnelCoord.x, tunnelCoord.y, 0), floorMap, obstacleMap, floorTile, tunnelColor);

            //Set the wall tiles around this tile to be walls.
            for (int x = tunnelCoord.x - 1; x <= tunnelCoord.x + 1; x++)
            {
                for (int y = tunnelCoord.y - 1; y <= tunnelCoord.y + 1; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);

                    if (SetWallTileIfEmpty(position, floorMap, obstacleMap, wallTile) == true)
                    {
                        //Debug.Log($"Position ({x}, {y}) is not empty.");
                        continue;
                    }
                }
            }
        }
    }



    



    /// <summary>
    /// Sets the floor map position to a wall tile if no other tile has yet been placed in the position.
    /// </summary>
    /// <param name="position">The position to set as a wall tile.</param>
    /// <param name="floorMap">The floor map.</param>
    /// <param name="obstacleMap">The obstacle map.</param>
    /// <param name="wallTile">The wall tile.</param>
    /// <returns>Returns <c>true</c> if position has been set to a wall tile; <c>false</c> otherwise.</returns>
    private bool SetWallTileIfEmpty(Vector3Int position, Tilemap floorMap, Tilemap obstacleMap, TileBase wallTile)
    {
        TileBase tile = floorMap.GetTile(position);
        if(tile != null)
        {
            return true;
        }

        obstacleMap.SetTile(position, wallTile);
        return false;
    }
}