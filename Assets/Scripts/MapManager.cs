using UnityEngine;
using UnityEngine.Tilemaps;



public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [SerializeField] private int _Width = 80;
    [SerializeField] private int _Height = 45;

    [SerializeField] private TileBase _FloorTile;
    [SerializeField] private TileBase _WallTile;

    [SerializeField] private Tilemap _FloorMap;
    [SerializeField] private Tilemap _ObstacleMap;



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



    private void Start()
    {
        Vector3Int centerTile = new Vector3Int(_Width / 2, _Height / 2, 0);

        BoundsInt wallBounds = new BoundsInt(new Vector3Int(29, 28, 0), new Vector3Int(3, 1, 0));

        for (int x = 0; x < wallBounds.size.x; x++)
        {
            for (int y = 0; y < wallBounds.size.y; y++)
            {
                Vector3Int wallPosition = new Vector3Int(wallBounds.min.x + x, wallBounds.min.y + y, 0);
                _ObstacleMap.SetTile(wallPosition, _WallTile);
            }
        }

        Instantiate(Resources.Load<GameObject>("Player"), new Vector3(40 + 0.5f, 25 + 0.5f, 0), Quaternion.identity).name = "Player";
        Instantiate(Resources.Load<GameObject>("NPC"), new Vector3(40 - 5.5f, 25 + 0.5f, 0), Quaternion.identity).name = "NPC";

        Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        Camera.main.orthographicSize = 27;
    }



    ///<summary>Return True if x and y are inside of the bounds of this map. </summary>
    public bool InBounds(int x, int y)
    {
        return 0 <= x && x < _Width && 0 <= y && y < _Height;
    }
}