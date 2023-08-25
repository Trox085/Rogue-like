using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class MapManager : MonoBehaviour
{
    public static MapManager Instance;




    [Header("Map Settings")]
    [SerializeField] 
    private int _Width = 7168;

    [Header("Map Settings")]
    [SerializeField]
    private int _Height = 4096;

    [Header("Map Settings")]
    [SerializeField]
    private int roomMaxSize = 10;

    [Header("Map Settings")]
    [SerializeField]
    private int roomMinSize = 6;

    [Header("Map Settings")]
    [SerializeField]
    private int maxRooms = 30;


    [Header("Tiles")]
    [SerializeField] 
    private TileBase _FloorTile;

    [Header("Tiles")]
    [SerializeField]
    private TileBase _WallTile;

    [Header("Tiles")]
    [SerializeField]
    private TileBase _FogTile;


    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap _FloorMap;

    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap _ObstacleMap;

    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap _FogMap;



    [Header("Features")]
    [SerializeField]
    private bool _EnableFieldOfView = true;

    [Header("Features")]
    [SerializeField]
    private List<RectangularRoom> _Rooms = new List<RectangularRoom>();

    [Header("Features")]
    [SerializeField]
    private List<Vector3Int> _VisibleTiles = new List<Vector3Int>();

    [Header("Features")]
    [SerializeField]
    private Dictionary<Vector3Int, TileData> _Tiles = new Dictionary<Vector3Int, TileData>();


    private List<GameObject> _GeneratedObjects = new List<GameObject>();




    public Tilemap FloorMap
    {
        get
        {
            return _FloorMap;
        }
    }



    public Tilemap ObstacleMap
    {
        get
        {
            return _ObstacleMap;
        }
    }



    public TileBase FloorTile
    {
        get
        {
            return _FloorTile;
        }
    }



    public TileBase WallTile
    {
        get
        {
            return _WallTile;
        }
    }



    public List<RectangularRoom> Rooms
    {
        get
        {
            return _Rooms;
        }
    }



    private void Awake()
    {
        // Keep the first instance
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        // Self destruct any duplicate instances
        Destroy(gameObject);
    }



    public void Restart()
    {
        Start();
    }



    private void Start()
    {
        _Rooms.Clear();
        _VisibleTiles.Clear();
        _Tiles.Clear();

        _FloorMap.ClearAllTiles();
        _ObstacleMap.ClearAllTiles();
        _FogMap.ClearAllTiles();

        foreach (GameObject gameObject in _GeneratedObjects)
        {
            Destroy(gameObject);
        }
        _GeneratedObjects.Clear();

        ProcGen procGen = new ProcGen();
        procGen.GenerateDungeon(_Width, _Height, roomMaxSize, roomMinSize, maxRooms, _Rooms);

        AddTileMapToDictionary(_FloorMap);
        AddTileMapToDictionary(_ObstacleMap);

        SetupFogMap();

        //Instantiate(Resources.Load<GameObject>("NPC"), new Vector3(40 - 5.5f, 25 + 0.5f, 0), Quaternion.identity).name = "NPC";

        Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        Camera.main.orthographicSize = 27;
    }



    ///<summary>Return True if x and y are inside of the bounds of this map. </summary>
    public bool InBounds(int x, int y)
    {
        return 0 <= x && x < _Width && 0 <= y && y < _Height;
    }



    public void CreatePlayer(Vector2 position)
    {
        if(InBounds(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)) == false)
        {
            Debug.LogError("Player created outside of map bounds.");
        }

        GameObject playerResource = Resources.Load<GameObject>("Player");
        GameObject player = Instantiate(playerResource, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        player.name = "Player";

        _GeneratedObjects.Add(player);
    }



    public void UpdateFogMap(List<Vector3Int> playerFieldOfView)
    {
        if (_EnableFieldOfView == false)
        {
            foreach(KeyValuePair<Vector3Int, TileData> kvp in _Tiles)
            {
                Vector3Int position = kvp.Key;
                TileData tileData = kvp.Value;

                tileData.isVisible = true;
                _FogMap.SetColor(position, Color.clear);
                _VisibleTiles.Add(position);
            }
            return;
        }

        // Clear all visible tiles, marking them as explored as needed
        foreach (Vector3Int position in _VisibleTiles)
        {
            TileData tileData;
            if (_Tiles.TryGetValue(position, out tileData) == true)
            {
                if (tileData.isExplored == false)
                {
                    tileData.isExplored = true;
                }

                tileData.isVisible = false;
            }

            _FogMap.SetColor(position, new Color(1.0f, 1.0f, 1.0f, 0.5f));
        }
        _VisibleTiles.Clear();
        
        // Set tiles in the player field of view to visible.
        foreach(Vector3Int position in playerFieldOfView)
        {
            TileData tileData;
            if(_Tiles.TryGetValue(position, out tileData) == true)
            {
                tileData.isVisible = true;
            }
            
            _FogMap.SetColor(position, Color.clear);
            _VisibleTiles.Add(position);
        }
    }



    public void SetEntitiesVisibilities()
    {
        GameManager gameManager = GameManager.Instance;

        List<Entity> entities = gameManager.Entities;

        foreach(Entity entity in entities)
        {
            if (entity == null)
            {
                continue;
            }

            // Skip the player
            Player player = entity.GetComponent<Player>();
            if (player != null)
            {
                continue;
            }

            // Toggle the sprite renderer for each entity based on if their position is visible to the player
            Vector3Int entityPosition = _FloorMap.WorldToCell(entity.transform.position);

            SpriteRenderer spriteRenderer = entity.GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = (_EnableFieldOfView == false || _VisibleTiles.Contains(entityPosition));
        }
    }



    private void AddTileMapToDictionary(Tilemap tilemap)
    {
        BoundsInt tilemapCellBounds = tilemap.cellBounds;
        BoundsInt.PositionEnumerator tilemapCellBoundsAllPositionsWithin = tilemapCellBounds.allPositionsWithin;
        foreach(Vector3Int position in tilemapCellBoundsAllPositionsWithin)
        {
            // Skip any position that has no tile.
            if(tilemap.HasTile(position) == false)
            {
                continue;
            }

            TileData tile = new TileData();
            _Tiles.Add(position, tile);
        }
    }



    private void SetupFogMap()
    {
        foreach(Vector3Int position in _Tiles.Keys)
        {
            _FogMap.SetTile(position, _FogTile);
            _FogMap.SetTileFlags(position, TileFlags.None);
        }
    }
}