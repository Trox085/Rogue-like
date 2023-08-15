using System.Collections.Generic;
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

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight, mapManager.Rooms.Count);

            // Check if this room intersects with any other rooms
            if (newRoom.Overlaps(rooms) == true)
            {
                continue;
            }


            // If there are no intersections then the room is valid.
            Debug.Log($"Generating room #{rooms.Count + 1})");

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
                        // Clear any obstacles.
                        TileBase obstacleTile = obstacleMap.GetTile(new Vector3Int(x, y, 0));
                        if(obstacleTile != null)
                        {
                            obstacleMap.SetTile(new Vector3Int(x, y, 0), null);
                        }

                        // Set the floor tile
                        floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
                    }
                }
            }

            if (mapManager.Rooms.Count > 0)
            {
                // Dig out a tunnel between this room and the previous one.
                TunnelBetween(mapManager.Rooms[mapManager.Rooms.Count - 1], newRoom);
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

        List<Vector2Int> tunnelCoords;
        if (Random.value < 0.75f)
        {
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

            List<Vector2Int> bresenhamLineCoords = GenerateBresenhamLine(oldRoomCenter, tunnelCorner, 1);

            tunnelCoords = new List<Vector2Int>();
            tunnelCoords.AddRange(bresenhamLineCoords);

            bresenhamLineCoords = GenerateBresenhamLine(tunnelCorner, newRoomCenter, 1);
            tunnelCoords.AddRange(bresenhamLineCoords);
        }
        else
        {
            // Create a direct connection between the two points.
            tunnelCoords = GenerateBresenhamLine(oldRoomCenter, newRoomCenter, 0.5f);
        }
            
        
        

        // Set the tiles for this tunnel.
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            Vector2Int tunnelCoord = tunnelCoords[i];

            // Clear any obstacles for the position.
            if (obstacleMap.HasTile(new Vector3Int(tunnelCoord.x, tunnelCoord.y, 0)))
            {
                obstacleMap.SetTile(new Vector3Int(tunnelCoord.x, tunnelCoord.y, 0), null);
            }

            // Set the floor tile.
            floorMap.SetTile(new Vector3Int(tunnelCoord.x, tunnelCoord.y, 0), floorTile);

            // Set the wall tiles around this tile to be walls.
            for (int x = tunnelCoord.x - 1; x <= tunnelCoord.x + 1; x++)
            {
                for (int y = tunnelCoord.y - 1; y <= tunnelCoord.y + 1; y++)
                {
                    if (SetWallTileIfEmpty(new Vector3Int(x, y, 0), floorMap, obstacleMap, wallTile))
                    {
                        continue;
                    }
                }
            }
        }
    }



    private List<Vector2Int> GenerateBresenhamLine(Vector2Int start, Vector2Int end, float lineWidth)
    {
        List<Vector2Int> linePoints = new List<Vector2Int>();

        int dx = Mathf.Abs(end.x - start.x);
        int sx = (start.x < end.x) ? 1 : -1;
        int dy = Mathf.Abs(end.y - start.y);
        int sy = (start.y < end.y) ? 1 : -1;
        int err = dx - dy;
        int e2, x2, y2;

        float ed = (dx + dy == 0) ? 1 : Mathf.Sqrt(dx * dx + dy * dy);

        lineWidth = (lineWidth + 1) / 2;

        while (start.x != end.x || start.y != end.y)
        {
            linePoints.Add(start);

            e2 = err;
            x2 = start.x;

            if (2 * e2 >= -dx)
            {
                for (e2 += dy, y2 = start.y; e2 < ed * lineWidth && (end.y != y2 || dx > dy); e2 += dx)
                {
                    linePoints.Add(new Vector2Int(x2, y2 += sy));
                }

                if (start.x == end.x)
                {
                    linePoints.Add(start); // Add the end point
                    break;
                }

                e2 = err;
                err -= dy;
                start.x += sx;
            }

            if (2 * e2 <= dy)
            {
                for (e2 = dx - e2; e2 < ed * lineWidth && (end.x != x2 || dx < dy); e2 += dy)
                {
                    linePoints.Add(new Vector2Int(x2 += sx, start.y));
                }

                if (start.y == end.y)
                {
                    linePoints.Add(start); // Add the end point
                    break;
                }

                err += dx;
                start.y += sy;
            }
        }

        return linePoints;
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
        TileBase tile = floorMap.GetTile(new Vector3Int(position.x, position.y, 0));
        if(tile != null)
        {
            return true;
        }

        obstacleMap.SetTile(new Vector3Int(position.x, position.y, 0), wallTile);
        return false;
    }
}