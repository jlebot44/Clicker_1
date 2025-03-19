using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; } // Singleton

    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _reliefTilemap;
    [SerializeField] private Tilemap _buildingTilemap;
    [SerializeField] private Tilemap _fogTilemap;
    [SerializeField] private Tilemap _roadTilemap;


    public Tilemap GroundTilemap { get => _groundTilemap; set => _groundTilemap = value; }
    public Tilemap ReliefTilemap { get => _reliefTilemap; set => _reliefTilemap = value; } 
    public Tilemap BuildingTilemap { get => _buildingTilemap; set => _buildingTilemap = value; }
    public Tilemap FogTilemap { get => _fogTilemap; set => _fogTilemap = value; }

    private Dictionary<Vector3Int, TileData> tileDataMap = new Dictionary<Vector3Int, TileData>();



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("TileManager déjà existant ! Suppression de l'instance en double.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeTiles();
    }

    void InitializeTiles()
    {
        BoundsInt bounds = GroundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                TileBase groundTile = GroundTilemap.GetTile(cellPosition);
                TileBase reliefTile = ReliefTilemap.GetTile(cellPosition);
                TileBase buildingTile = BuildingTilemap.GetTile(cellPosition);

                GroundType groundType = groundTile != null ? ClassifyGroundType(groundTile.name) : GroundType.None;
                ReliefType reliefType = reliefTile != null ? ClassifyReliefType(reliefTile.name) : ReliefType.None;
                BuildingType buildingType = buildingTile != null ? ClassifyBuildingType(buildingTile.name) : BuildingType.None;

                // Calcul du niveau de brouillard : |X| + |Y|
                int fogLevel = Mathf.Abs(cellPosition.x) + Mathf.Abs(cellPosition.y);

                // Création de l'objet TileData
                TileData tileData = new TileData(groundType, reliefType, buildingType, fogLevel);

                // Si la tuile est à la position (0,0,0), on la "claim" et active le lien à la capital
                if (cellPosition == Vector3Int.zero)
                {
                    tileData.isClaimed = true;
                    tileData.isConnectedToCapital = true;
                }

                // Ajout au dictionnaire
                tileDataMap[cellPosition] = tileData;
            }
        }
    }

    public TileData GetTileData(Vector3Int cellPosition)
    {
        return tileDataMap.ContainsKey(cellPosition) ? tileDataMap[cellPosition] : null;
    }

    private BuildingType ClassifyBuildingType(string tileName)
    {
        if (string.IsNullOrEmpty(tileName))
            return BuildingType.None;

        tileName = tileName.ToLower(); // Convertir en minuscule pour éviter les erreurs de casse

        //  ---- compléter ici en fonction des types de batiments ----
        if (tileName.Contains("town")) return BuildingType.Town;
        return BuildingType.Other; // Si le nom ne correspond à rien
    }

    private ReliefType ClassifyReliefType(string tileName)
    {
        if (string.IsNullOrEmpty(tileName))
            return ReliefType.None;

        tileName = tileName.ToLower(); // Convertir en minuscule pour éviter les erreurs de casse

        //  ---- compléter ici en fonction des types de batiments ----
        if (tileName.Contains("mountain")) return ReliefType.Mountain;
        if (tileName.Contains("river")) return ReliefType.River;
        if (tileName.Contains("tree")) return ReliefType.Wood;
        return ReliefType.Other; // Si le nom ne correspond à rien
    }

    private GroundType ClassifyGroundType(string tileName)
    {
        if (string.IsNullOrEmpty(tileName))
            return GroundType.None;

        tileName = tileName.ToLower(); // Convertir en minuscule pour éviter les erreurs de casse

        //  ---- compléter ici en fonction des types de batiments ----
        if (tileName.Contains("grass")) return GroundType.Grass;
        return GroundType.Other; // Si le nom ne correspond à rien
    }

    public List<TileData> GetClaimedTowns()
    {
        List<TileData> claimedTowns = new List<TileData>();

        foreach (var tile in tileDataMap.Values)
        {
            if (tile.isClaimed && tile.Building == BuildingType.Town)
            {
                claimedTowns.Add(tile);
            }
        }
        return claimedTowns;
    }



}