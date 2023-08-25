using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class ShadowCasting : IFieldOfViewCalculator
{
    private readonly Tilemap _obstaclesTilemap;
    private readonly int _angleStepInDegrees;



    public ShadowCasting(Tilemap obstaclesTilemap, int angleStepInDegrees)
    {
        if(obstaclesTilemap == null)
        {
            throw new System.ArgumentNullException(nameof(obstaclesTilemap));
        }
        _obstaclesTilemap = obstaclesTilemap;

        if(angleStepInDegrees < 1)
        {
            throw new System.ArgumentOutOfRangeException(nameof(angleStepInDegrees), $"{nameof(angleStepInDegrees)} must be between 1 and 90.");
        }
        _angleStepInDegrees = angleStepInDegrees;
    }



    public void CalculateFieldOfView(Vector3Int origin, int rangeLimit, List<Vector3Int> fieldOfView)
    {
        fieldOfView.Add(origin);

        float radianConversion = Mathf.PI / 180.0f;

        for (int angle = 0; angle < 360; angle += _angleStepInDegrees)
        {
            float angleInRadians = angle * radianConversion;
            float xDirection = Mathf.Cos(angleInRadians);
            float yDirection = Mathf.Sin(angleInRadians);

            for (int step = 1; step <= rangeLimit; step++)
            {
                int cellX = origin.x + (int) Mathf.Round(xDirection * step);
                int cellY = origin.y + (int) Mathf.Round(yDirection * step);

                Vector3Int cellPosition = new Vector3Int(cellX, cellY, 0);

                fieldOfView.Add(cellPosition);

                if (_obstaclesTilemap.HasTile(cellPosition) == true)
                {
                    // Impassable cell
                    break;
                }
            }
        }
    }
}