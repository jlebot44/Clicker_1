using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    // Événement pour notifier qu'une construction a été faite
    public static event Action<Vector3Int> OnBuildingConstructed;

    Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    // Dictionnaire global des bâtiments
    private Dictionary<Vector3Int, BuildingData> _buildingsDataMap = new Dictionary<Vector3Int, BuildingData>();

    public Dictionary<Vector3Int, BuildingData> BuildingsDataMap { get => _buildingsDataMap; set => _buildingsDataMap = value; }

    


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        AddBuilding(Vector3Int.zero, BuildingType.Town);
    }

    // Retourne les constructions disponibles
    public List<string> GetAvailableConstructions(Vector3Int cellPosition)
    {
        List<string> options = new List<string>();
        
        
        // Pré-calcul des conditions
        bool isGrass = TileManager.Instance.isTargetGroundOnTile(cellPosition, GroundType.Grass); // sol dur
        bool isMountain = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.Mountain); // présence de montagne
        bool noRelief = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.None); // Absence de relief 
        bool hasAdjacentRoadOrTown = directions.Any(dir => TileManager.Instance.IsRoadOrTown(cellPosition + dir, true)); // Présence d'au moins une case connectée adjacente
        bool hasAdjacentWood = directions.Any(dir => TileManager.Instance.isTargetReliefOnTile(cellPosition + dir, ReliefType.Wood));
        bool hasAdjacentMountain = directions.Any(dir => TileManager.Instance.isTargetReliefOnTile(cellPosition + dir, ReliefType.Mountain));


        if (TileManager.Instance.GetTileData(cellPosition) != null && TileManager.Instance.isTargetBuildingOnTile(cellPosition, BuildingType.None))
        {
            
            if (isGrass && noRelief && hasAdjacentRoadOrTown) 
                options.Add("road");            
            if (isGrass && noRelief && hasAdjacentWood && hasAdjacentRoadOrTown)
                options.Add("lumberjack");            
            if (isGrass && noRelief)
                options.Add("temple");
            if (isGrass && isMountain && hasAdjacentRoadOrTown)
                options.Add("stoneMine");
        }

        return options;
    }

    // Logique de construction
    public void Build(string construction, Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);
        if (tileData != null)
        {
            switch (construction)
            {
                case "road":
                    tileData.Building = BuildingType.Road;
                    break;
                case "lumberjack":
                    tileData.Building = BuildingType.Lumberjack;
                    break;
                case "temple":
                    tileData.Building = BuildingType.Temple;
                    break;
                case "stoneMine":
                    tileData.Building = BuildingType.StoneMine;
                    break;

            }

            // Placer la construction sur la Tilemap
            TileManager.Instance.PlaceBuildingTile(cellPosition, TileManager.Instance.GetTileData(cellPosition).Building);

            // ajout du batiment dans le dictionnaire du BuildingManager
            AddBuilding(cellPosition, tileData.Building);

            // Envoi du message 
            OnBuildingConstructed?.Invoke(cellPosition);


        }
    }

    public void AddBuilding(Vector3Int position, BuildingType buildingType)
    {
        BuildingData buildingData = new BuildingData(buildingType);
        _buildingsDataMap[position] = buildingData;
    }
}
