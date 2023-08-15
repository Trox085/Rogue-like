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


    [Header("Colors")]
    [SerializeField] 
    private Color32 darkColor = new Color32(0, 0, 0, 0);

    [Header("Colors")]
    [SerializeField] 
    private Color32 lightColor = new Color32(255, 255, 255, 255);


    [Header("Tiles")]
    [SerializeField] 
    private TileBase _FloorTile;
    
    [Header("Tiles")]
    [SerializeField] 
    private TileBase _WallTile;


    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap _FloorMap;

    [Header("Tilemaps")]
    [SerializeField] 
    private Tilemap _ObstacleMap;


    [Header("Features")]
    [SerializeField] 
    private List<RectangularRoom> _Rooms = new List<RectangularRoom>();


    

    
    



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



    private void Start()
    {
        ProcGen procGen = new ProcGen();
        procGen.GenerateDungeon(_Width, _Height, roomMaxSize, roomMinSize, maxRooms, _Rooms);

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
    }
}