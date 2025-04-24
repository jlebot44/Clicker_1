using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    [Header("Tilemaps")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _reliefTilemap;
    [SerializeField] private Tilemap _buildingTilemap;
    [SerializeField] private Tilemap _fogTilemap;

    [Header("Route Tiles")]
    [SerializeField] private TileBase _horizontalRoadTile;
    [SerializeField] private TileBase _verticalRoadTile;
    [SerializeField] private TileBase _cornerTopLeftTile;
    [SerializeField] private TileBase _cornerTopRightTile;
    [SerializeField] private TileBase _cornerBottomLeftTile;
    [SerializeField] private TileBase _cornerBottomRightTile;
    [SerializeField] private TileBase _tShapeTopTile;
    [SerializeField] private TileBase _tShapeLeftTile;
    [SerializeField] private TileBase _tShapeRightTile;
    [SerializeField] private TileBase _tShapeBottomTile;
    [SerializeField] private TileBase _intersectionRoadTile;

    [Header("Building Tiles")]
    [SerializeField] private TileBase _lumberjack;
    [SerializeField] private TileBase _temple;
    [SerializeField] private TileBase _stoneMine;
    [SerializeField] private TileBase _woodPile;
    [SerializeField] private TileBase _stonePile;
    [SerializeField] private TileBase _manaPile;

    [Header("FX")]
    [SerializeField] private GameObject dustEffectPrefab;

    private Dictionary<Vector3Int, TileData> _tileDataMap = new();
    private Dictionary<int, TileBase> _roadTypeMapping;
    public Dictionary<BuildingType, TileBase> BuildingTiles { get; private set; }



    public Tilemap GroundTilemap => _groundTilemap;
    public Tilemap ReliefTilemap => _reliefTilemap;
    public Tilemap BuildingTilemap => _buildingTilemap;
    public Tilemap FogTilemap => _fogTilemap;
    public Dictionary<Vector3Int, TileData> TileDataMap => _tileDataMap;
    public Dictionary<int, TileBase> RoadTypeMapping => _roadTypeMapping;
    public GameObject DustEffectPrefab => dustEffectPrefab;

    public TileDataManager DataManager { get; private set; }
    public RoadTileRenderer RoadRenderer { get; private set; }
    public BuildingTileRenderer BuildingRenderer { get; private set; }



    private Dictionary<TileBase, BuildingType> _tileToBuildingTypeMap;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("TileManager déjà existant ! Suppression de l'instance en double.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _roadTypeMapping = new()
        {
            { 0b0000, null },
            { 0b1000, _horizontalRoadTile }, { 0b0100, _horizontalRoadTile },
            { 0b0010, _verticalRoadTile }, { 0b0001, _verticalRoadTile },
            { 0b1100, _horizontalRoadTile }, { 0b0011, _verticalRoadTile },
            { 0b1010, _cornerTopLeftTile }, { 0b0110, _cornerTopRightTile },
            { 0b1001, _cornerBottomLeftTile }, { 0b0101, _cornerBottomRightTile },
            { 0b1110, _tShapeTopTile }, { 0b1101, _tShapeBottomTile },
            { 0b0111, _tShapeRightTile }, { 0b1011, _tShapeLeftTile },
            { 0b1111, _intersectionRoadTile }
        };

        BuildingTiles = new()
        {
            { BuildingType.Lumberjack, _lumberjack },
            { BuildingType.Temple, _temple },
            { BuildingType.StoneMine, _stoneMine },
            { BuildingType.WoodPile, _woodPile },
            { BuildingType.StonePile, _stonePile },
            { BuildingType.ManaPile, _manaPile }
        };

        DataManager = new TileDataManager();
        RoadRenderer = new RoadTileRenderer();
        BuildingRenderer = new BuildingTileRenderer();
    }

    private void Start()
    {
        InitializeTiles();
    }

    private void InitializeTiles()
    {
        BoundsInt bounds = _groundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                TileBase groundTile = _groundTilemap.GetTile(pos);
                TileBase reliefTile = _reliefTilemap.GetTile(pos);
                TileBase buildingTile = _buildingTilemap.GetTile(pos);

                GroundType groundType = groundTile != null ? ClassifyGroundType(groundTile.name) : GroundType.None;
                ReliefType reliefType = reliefTile != null ? ClassifyReliefType(reliefTile.name) : ReliefType.None;

                int fogLevel = Mathf.CeilToInt((Mathf.Abs(pos.x) + Mathf.Abs(pos.y)) * GetFogMultiplier(reliefType));

                TileData tileData = new TileData(groundType, reliefType, fogLevel);

                if (pos == Vector3Int.zero)
                {
                    tileData.IsClaimed = true;
                    tileData.IsConnectedToCapital = true;
                }

                _tileDataMap[pos] = tileData;
            }
        }
    }

    private float GetFogMultiplier(ReliefType relief)
    {
        return relief switch
        {
            ReliefType.Wood => 1.5f,
            ReliefType.Mountain => 3.5f,
            ReliefType.River => 2f,
            _ => 1f
        };
    }

    private GroundType ClassifyGroundType(string tileName)
    {
        if (string.IsNullOrEmpty(tileName)) return GroundType.None;
        tileName = tileName.ToLower();
        if (tileName.Contains("grass")) return GroundType.Grass;
        return GroundType.Other;
    }

    private ReliefType ClassifyReliefType(string tileName)
    {
        if (string.IsNullOrEmpty(tileName)) return ReliefType.None;
        tileName = tileName.ToLower();
        if (tileName.Contains("mountain")) return ReliefType.Mountain;
        if (tileName.Contains("river")) return ReliefType.River;
        if (tileName.Contains("tree")) return ReliefType.Wood;
        return ReliefType.Other;
    }

}
