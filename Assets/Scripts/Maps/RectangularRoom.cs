using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class RectangularRoom
{
    public int id;
    public int x;
    public int y;
    public int width;
    public int height;



    public RectangularRoom(int x, int y, int width, int height, int id = 0)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.id = id;
    }



    public Vector2Int Center()
    {
        return new Vector2Int(x + width / 2, y + height / 2);
    }



    /// <summary>
    ///  Return the area of this room as a Bounds.
    /// </summary>
    public Bounds GetBounds()
    {
        return new Bounds(new Vector3(x, y, 0), new Vector3(width, height, 0));
    }



    /// <summary>
    /// Return the area of this room as BoundsInt
    /// </summary>
    public BoundsInt GetBoundsInt()
    {
        return new BoundsInt(new Vector3Int(x, y, 0), new Vector3Int(width, height, 0));
    }



    /// <summary>
    /// Return True if this room overlaps with another RectangularRoom.
    /// </summary>
    public bool Overlaps(List<RectangularRoom> otherRooms)
    {
        Bounds roomBounds = GetBounds();

        foreach (RectangularRoom otherRoom in otherRooms)
        {
            Bounds otherRoomBounds = otherRoom.GetBounds();

            if (roomBounds.Intersects(otherRoomBounds) == true)
            {
                return true;
            }
        }

        return false;
    }
}