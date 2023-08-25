using System.Collections.Generic;
using UnityEngine;



public interface IFieldOfViewCalculator
{
    public void CalculateFieldOfView(Vector3Int origin, int rangeLimit, List<Vector3Int> fieldOfView);
}