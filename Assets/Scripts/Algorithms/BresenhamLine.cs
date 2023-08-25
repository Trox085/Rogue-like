using System.Collections.Generic;
using UnityEngine;



public static class BresenhamLine
{
    public static void Compute(Vector2Int start, Vector2Int end, List<Vector2Int> lineCoordinates)
    {
        // Calculate the absolute differences in x and y coordinates
        int deltaX = Mathf.Abs(end.x - start.x);
        int deltaY = Mathf.Abs(end.y - start.y);

        // Determine the step direction for x and y
        int stepX, stepY;
        if (start.x < end.x)
        {
            // Move to the right
            stepX = 1;
        }
        else
        {
            // Move to the left
            stepX = -1;
        }

        if (start.y < end.y)
        {
            // Move upwards
            stepY = 1;
        }
        else
        {
            // Move downwards
            stepY = -1;
        }

        // Initialize the Bresenham algorithm error and squared line width
        int error = deltaX - deltaY;

        // Determine the maximum change along x or y
        int maxDelta = Mathf.Max(deltaX, deltaY);

        // Set the initial coordinates
        int currentX = start.x;
        int currentY = start.y;

        // Iterate through the line points using Bresenham's algorithm
        for (int i = 0; i <= maxDelta; i++)
        {
            // Add the current point to the line coordinates list
            lineCoordinates.Add(new Vector2Int(currentX, currentY));

            // Exit the loop if end point is reached
            if (currentX == end.x && currentY == end.y)
            {
                break;
            }

            // Calculate the double error for efficient calculations
            int doubleError = error * 2;

            // Adjust the error and move along x if needed
            if (doubleError > -deltaY)
            {
                error -= deltaY;
                currentX += stepX;
            }

            // Adjust the error and move along y if needed
            if (doubleError < deltaX)
            {
                error += deltaX;
                currentY += stepY;
            }
        }
    }
}