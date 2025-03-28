using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; } // Singleton

    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _reliefTilemap;
    [SerializeField] private Tilemap _buildingTilemap;
    [SerializeField] private Tilemap _fogTilemap;

    // tuiles de route
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
    private Dictionary<int, TileBase> roadTypeMapping;

    // building
    [SerializeField] private TileBase _lumberjack;
    [SerializeField] private TileBase _temple;
    [SerializeField] private TileBase _stoneMine;

    private Dictionary<Vector3Int, TileData> _tileDataMap = new Dictionary<Vector3Int, TileData>();


    public Tilemap GroundTilemap { get => _groundTilemap; set => _groundTilemap = value; }
    public Tilemap ReliefTilemap { get => _reliefTilemap; set => _reliefTilemap = value; } 
    public Tilemap BuildingTilemap { get => _buildingTilemap; set => _buildingTilemap = value; }
    public Tilemap FogTilemap { get => _fogTilemap; set => _fogTilemap = value; }
    public Dictionary<Vector3Int, TileData> TileDataMap { get => _tileDataMap; set => _tileDataMap = value; }



    private Vector3Int[] _directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };



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

        roadTypeMapping = new Dictionary<int, TileBase>
        {
            { 0b0000, null }, // Pas de route
            { 0b1000, _horizontalRoadTile }, // Route seule à gauche
            { 0b0100, _horizontalRoadTile }, // Route seule à droite
            { 0b0010, _verticalRoadTile }, // Route seule en haut
            { 0b0001, _verticalRoadTile }, // Route seule en bas

            { 0b1100, _horizontalRoadTile }, // Route horizontale gauche-droite
            { 0b0011, _verticalRoadTile }, // Route verticale haut-bas

            // Coins
            { 0b1010, _cornerTopLeftTile }, // Coin haut-gauche
            { 0b0110, _cornerTopRightTile }, // Coin haut-droit
            { 0b1001, _cornerBottomLeftTile }, // Coin bas-gauche
            { 0b0101, _cornerBottomRightTile }, // Coin bas-droit

            // Formes en T
            { 0b1110, _tShapeTopTile }, // T vers le haut
            { 0b1101, _tShapeBottomTile }, // T vers le bas
            { 0b0111, _tShapeRightTile }, // T vers la droite
            { 0b1011, _tShapeLeftTile }, // T vers la gauche

            // Carrefour
            { 0b1111, _intersectionRoadTile }, // Carrefour à 4 voies
        };
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
                    tileData.IsClaimed = true;
                    tileData.IsConnectedToCapital = true;
                }

                // Ajout au dictionnaire
                TileDataMap[cellPosition] = tileData;
            }
        }
    }

    public TileData GetTileData(Vector3Int cellPosition)
    {
        return TileDataMap.ContainsKey(cellPosition) ? TileDataMap[cellPosition] : null;
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

    //public List<TileData> GetClaimedTowns()
    //{
    //    List<TileData> claimedTowns = new List<TileData>();

    //    foreach (var tile in TileDataMap.Values)
    //    {
    //        if (tile.IsClaimed && tile.Building == BuildingType.Town)
    //        {
    //            claimedTowns.Add(tile);
    //        }
    //    }
    //    return claimedTowns;
    //}


    // Méthode pour placer une tile sur la Tilemap
    public void PlaceBuildingTile(Vector3Int cellPosition, BuildingType buildingType)
    {
        // Si le type de construction est une route, appel de la méthode PlaceRoad
        if (buildingType == BuildingType.Road)
        {
            PlaceRoad(cellPosition);  // Appel à la méthode PlaceRoad pour gérer les routes
        }
        else
        {
            TileBase tileToPlace = null;

            // Sélectionner la bonne tile en fonction du type de construction
            switch (buildingType)
            {
                case BuildingType.Lumberjack:
                    tileToPlace = _lumberjack;
                    break;
                case BuildingType.Temple:
                    tileToPlace = _temple;
                    break;
                case BuildingType.StoneMine:
                    tileToPlace = _stoneMine;
                    break;
                // Ajoute d'autres cas pour d'autres types de bâtiments
                default:
                    Debug.LogWarning("Building type not found!");
                    break;
            }

            // Si une tile a été trouvée, place-la sur la Tilemap
            if (tileToPlace != null)
            {
                BuildingTilemap.SetTile(cellPosition, tileToPlace);
            }
        }
    }

    // Méthode pour placer une route et vérifier les tuiles adjacentes
    public void PlaceRoad(Vector3Int position)
    {
        // Placer la route sur la tuile actuelle
        UpdateTileForRoad(position);

        // Vérifier et mettre à jour les tuiles adjacentes si elles sont déjà des routes ou des villes
        foreach (var direction in _directions)
        {
            if (IsRoadOrTown(position + direction, false))
                UpdateTileForRoad(position + direction);
        }
    }

    // Méthode pour vérifier si la tuile est une route ou une ville
    
    public bool IsRoadOrTown(Vector3Int position, bool requiredIsConnectedToCapital)
    {
        TileData tileData = GetTileData(position);
        if (!requiredIsConnectedToCapital)
            return tileData != null && (tileData.Building == BuildingType.Road || tileData.Building == BuildingType.Town) && tileData.IsClaimed;
        else
            return tileData != null && (tileData.Building == BuildingType.Road || tileData.Building == BuildingType.Town) && (tileData.IsConnectedToCapital) && tileData.IsClaimed;
     }

    public bool isTargetReliefOnTile(Vector3Int position, ReliefType relief)
    {
        TileData tileData = GetTileData(position);
        return (tileData != null) && (tileData.Relief == relief) && tileData.IsClaimed;
    }

    public bool isTargetBuildingOnTile(Vector3Int position, BuildingType building)
    {
        TileData tileData = GetTileData(position);
        return (tileData != null) && (tileData.Building == building) && tileData.IsClaimed;
    }

    public bool isTargetGroundOnTile(Vector3Int position, GroundType ground)
    {
        TileData tileData = GetTileData(position);
        return (tileData != null) && (tileData.Ground == ground) && tileData.IsClaimed;
    }







    // Méthode pour mettre à jour la tuile (route ou autre type)
    private void UpdateTileForRoad(Vector3Int position)
    {
        // Représentation binaire des connexions aux routes/villes
        int roadConfig = 0b0000;

        foreach (var direction in _directions)
        {
            Vector3Int neighborPos = position + direction;
            TileData neighborTile = GetTileData(neighborPos);

            if (neighborTile != null)
            {
                // Vérifier si c'est une route ou une ville
                if ((neighborTile.Building == BuildingType.Road || neighborTile.Building == BuildingType.Town))
                {
                    // Mettre à jour la configuration binaire
                    roadConfig |= GetBinaryMask(direction);

                    // Connecter uniquement les routes et les villes
                    neighborTile.IsConnectedToCapital = true;
                }
            }
        }

        // Ne pas modifier une ville existante
        if (TileDataMap.ContainsKey(position) && TileDataMap[position].Building == BuildingType.Town)
            return;

        // Trouver la bonne tuile de route
        if (roadTypeMapping.TryGetValue(roadConfig, out TileBase roadTile))
        {
            _buildingTilemap.SetTile(position, roadTile);
        }
    }

    // Fonction pour obtenir le masque binaire d'une direction
    private int GetBinaryMask(Vector3Int direction)
    {
        if (direction == Vector3Int.left) return 0b1000;
        if (direction == Vector3Int.right) return 0b0100;
        if (direction == Vector3Int.up) return 0b0010;
        if (direction == Vector3Int.down) return 0b0001;
        return 0;
    }

}