using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class Entity : MonoBehaviour
{
    [SerializeField] private bool _IsSentient = false;

    [SerializeField] private int _FieldOfViewRange = 8;

    [SerializeField] private List<Vector3Int> _FieldOfView;



    private IFieldOfViewCalculator _FieldOfViewCalculator;



    public bool IsSentient
    {
        get
        {
            return _IsSentient;
        }
    }



    private void Start()
    {
        if(_IsSentient == true)
        {
            GameManager gameManager = GameManager.Instance;
            gameManager.AddEntity(this);

            _FieldOfView = new List<Vector3Int>();

            MapManager mapManager = MapManager.Instance;
            Tilemap obstacleMap = mapManager.ObstacleMap;

            _FieldOfViewCalculator = new BresenhamsLineAlgorithm(obstacleMap);

            UpdateFieldOfView();
        }
    }



    private void OnDestroy()
    {
        if (_IsSentient == true)
        {
            GameManager gameManager = GameManager.Instance;
            gameManager.RemoveEntity(this);
        }
    }



    public void Move(Vector2 direction)
    {
        transform.position += (Vector3)direction;
    }



    public void UpdateFieldOfView()
    {
        //Debug.Log("UpdateFieldOfView");

        MapManager mapManager = MapManager.Instance;
        Tilemap floorMap = mapManager.FloorMap;

        Vector3Int gridPosition = floorMap.WorldToCell(transform.position);

        _FieldOfView.Clear();
        _FieldOfViewCalculator.CalculateFieldOfView(gridPosition, _FieldOfViewRange, _FieldOfView);
        //_VisibilityAlgorithm.Compute(gridPosition, _FieldOfViewRange, _FieldOfView);

        Player player = GetComponent<Player>();
        if(player != null)
        {
            mapManager.UpdateFogMap(_FieldOfView);
            mapManager.SetEntitiesVisibilities();
        }
    }
}