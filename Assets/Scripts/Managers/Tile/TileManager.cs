using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;


// TileManager ne g�re que l'affichage des tuiles.
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
    [SerializeField] private TileBase _woodPile;
    [SerializeField] private TileBase _stonePile;
    [SerializeField] private TileBase _manaPile;

    private Dictionary<Vector3Int, TileData> _tileDataMap = new Dictionary<Vector3Int, TileData>();

    [SerializeField] private GameObject dustEffectPrefab;


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
            Debug.LogWarning("TileManager d�j� existant ! Suppression de l'instance en double.");
            Destroy(gameObject);
        }

        roadTypeMapping = new Dictionary<int, TileBase>
        {
            { 0b0000, null }, // Pas de route
            { 0b1000, _horizontalRoadTile }, // Route seule � gauche
            { 0b0100, _horizontalRoadTile }, // Route seule � droite
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
            { 0b1111, _intersectionRoadTile }, // Carrefour � 4 voies
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

                // Cr�ation de l'objet TileData
                TileData tileData = new TileData(groundType, reliefType, buildingType, fogLevel);

                // Si la tuile est � la position (0,0,0), on la "claim" et active le lien � la capital
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

        tileName = tileName.ToLower(); // Convertir en minuscule pour �viter les erreurs de casse

        //  ---- compl�ter ici en fonction des types de batiments ----
        if (tileName.Contains("town")) return BuildingType.Town;
        if (tileName.Contains("capital")) return BuildingType.Capital;
        if (tileName.Contains("shrine")) return BuildingType.BonusShrine;
        return BuildingType.Other; // Si le nom ne correspond � rien
    }

    private ReliefType ClassifyReliefType(string tileName)
    {
        if (string.IsNullOrEmpty(tileName))
            return ReliefType.None;

        tileName = tileName.ToLower(); // Convertir en minuscule pour �viter les erreurs de casse

        //  ---- compl�ter ici en fonction des types de batiments ----
        if (tileName.Contains("mountain")) return ReliefType.Mountain;
        if (tileName.Contains("river")) return ReliefType.River;
        if (tileName.Contains("tree")) return ReliefType.Wood;
        return ReliefType.Other; // Si le nom ne correspond � rien
    }

    private GroundType ClassifyGroundType(string tileName)
    {
        if (string.IsNullOrEmpty(tileName))
            return GroundType.None;

        tileName = tileName.ToLower(); // Convertir en minuscule pour �viter les erreurs de casse

        //  ---- compl�ter ici en fonction des types de batiments ----
        if (tileName.Contains("grass")) return GroundType.Grass;
        return GroundType.Other; // Si le nom ne correspond � rien
    }


    // M�thode pour placer une tile sur la Tilemap
    public void PlaceBuildingTile(Vector3Int cellPosition, BuildingType buildingType)
    {
        if (buildingType == BuildingType.Road)
        {
            PlaceRoad(cellPosition);
        }
        else
        {
            TileBase tileToPlace = null;

            switch (buildingType)
            {
                case BuildingType.Lumberjack: tileToPlace = _lumberjack; break;
                case BuildingType.Temple: tileToPlace = _temple; break;
                case BuildingType.StoneMine: tileToPlace = _stoneMine; break;
                case BuildingType.WoodPile: tileToPlace = _woodPile; break;
                case BuildingType.StonePile: tileToPlace = _stonePile; break;
                case BuildingType.ManaPile: tileToPlace = _manaPile; break;
                default:
                    Debug.LogWarning("Building type not found!");
                    return;
            }

            if (tileToPlace != null)
            {
                StartCoroutine(AnimateTilePlacement(cellPosition, tileToPlace));
            }
        }
    }


    private IEnumerator AnimateTilePlacement(Vector3Int cellPosition, TileBase tileBase)
    {
        Vector3 worldPos = _buildingTilemap.GetCellCenterWorld(cellPosition);

        GameObject ghost = new GameObject("GhostTile");
        ghost.transform.position = worldPos + Vector3.up * 0.12f;
        var spriteRenderer = ghost.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tileBase is Tile tile ? tile.sprite : null;
        spriteRenderer.sortingOrder = 10;

        float liftHeight = 0.03f;
        float liftDuration = 0.04f;
        float pauseDuration = 0.15f;
        float fallDuration = 0.02f;
        float bounceHeight = 0.01f;
        float bounceDuration = 0.03f;

        Vector3 topPosition = worldPos + Vector3.up * (0.12f + liftHeight);
        Vector3 basePosition = worldPos;

        // Mont�e ultra br�ve
        float t = 0;
        while (t < liftDuration)
        {
            float progress = t / liftDuration;
            float ease = Mathf.Sin(progress * Mathf.PI * 0.5f); // ease-out
            ghost.transform.position = Vector3.Lerp(worldPos + Vector3.up * 0.12f, topPosition, ease);
            t += Time.deltaTime;
            yield return null;
        }

        // Pause dramatique
        ghost.transform.position = topPosition;
        yield return new WaitForSeconds(pauseDuration);

        // Chute violente
        t = 0;
        while (t < fallDuration)
        {
            float progress = t / fallDuration;
            float ease = 1 - Mathf.Cos(progress * Mathf.PI * 0.5f); // ease-in
            ghost.transform.position = Vector3.Lerp(topPosition, basePosition, ease);
            t += Time.deltaTime;
            yield return null;
        }

        // Impact final
        ghost.transform.position = basePosition;

        // Instanciation de l'effet de poussi�re
        if (dustEffectPrefab != null)
        {

            GameObject dust = Instantiate(dustEffectPrefab, basePosition, Quaternion.identity);
            Destroy(dust, 2f); // Nettoyage apr�s
        }

        // Rebond minimal
        t = 0;
        while (t < bounceDuration)
        {
            float progress = t / bounceDuration;
            float ease = Mathf.Sin(progress * Mathf.PI);
            ghost.transform.position = basePosition + Vector3.up * ease * bounceHeight;
            t += Time.deltaTime;
            yield return null;
        }
    }







    // Retirer la tuile lors de la destruction
    public void RemoveBuilding(Vector3Int cellPosition)
    {
        BuildingTilemap.SetTile(cellPosition, null);
    }

    // M�thode pour placer une route et v�rifier les tuiles adjacentes
    public void PlaceRoad(Vector3Int position)
    {
        // Placer la route sur la tuile actuelle
        UpdateTileForRoad(position);

        // V�rifier et mettre � jour les tuiles adjacentes si elles sont d�j� des routes ou des villes
        foreach (var direction in _directions)
        {
            if (IsRoadOrTown(position + direction, false))
                UpdateTileForRoad(position + direction);
        }
    }

    // M�thode pour v�rifier si la tuile est une route ou une ville
    
    public bool IsRoadOrTown(Vector3Int position, bool requiredIsConnectedToCapital)
    {
        TileData tileData = GetTileData(position);
        if (!requiredIsConnectedToCapital)
            return tileData != null && (tileData.Building == BuildingType.Road || tileData.Building == BuildingType.Town || tileData.Building == BuildingType.Capital) && tileData.IsClaimed;
        else
            return tileData != null && (tileData.Building == BuildingType.Road || tileData.Building == BuildingType.Town || tileData.Building == BuildingType.Capital) && (tileData.IsConnectedToCapital) && tileData.IsClaimed;
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

    // M�thode pour mettre � jour la tuile (route ou autre type)
    private void UpdateTileForRoad(Vector3Int position)
    {
        // Repr�sentation binaire des connexions aux routes/villes
        int roadConfig = 0b0000;

        foreach (var direction in _directions)
        {
            Vector3Int neighborPos = position + direction;
            TileData neighborTile = GetTileData(neighborPos);

            if (neighborTile != null)
            {
                // V�rifier si c'est une route ou une ville
                if ((neighborTile.Building == BuildingType.Road || neighborTile.Building == BuildingType.Town || neighborTile.Building == BuildingType.Capital))
                {
                    // Mettre � jour la configuration binaire
                    roadConfig |= GetBinaryMask(direction);

                    // Connecter uniquement les routes et les villes
                    neighborTile.IsConnectedToCapital = true;
                }
            }
        }

        // Ne pas modifier une ville existante
        if (TileDataMap.ContainsKey(position) && TileDataMap[position].Building == BuildingType.Town || TileDataMap[position].Building == BuildingType.Capital)
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