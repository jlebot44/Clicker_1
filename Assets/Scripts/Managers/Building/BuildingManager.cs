using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    // Événement pour notifier qu'une construction a été faite
    public static event Action OnBuildingConstructed;

    readonly Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    // Liste des coûts de construction pour chaque bâtiment
    [SerializeField] private List<BuildingCostData> buildingCostsList;

    // liste des coûts pour l'évolution des batiments
    [SerializeField] private List<BuildingUpgradeData> upgradeDataList;



    // Dictionnaire global des bâtiments
    [SerializeField] private Dictionary<Vector3Int, BuildingData> _buildingsDataMap = new Dictionary<Vector3Int, BuildingData>();

    public Dictionary<Vector3Int, BuildingData> BuildingsDataMap => _buildingsDataMap;


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
        AddBuilding(Vector3Int.zero, BuildingType.Capital);
    }

    public BuildingData GetBuildingData(Vector3Int cellPosition)
    {
        return _buildingsDataMap.ContainsKey(cellPosition) ? _buildingsDataMap[cellPosition] : null;
    }


    // Logique de construction
    public void Build(BuildingType construction, Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);
        if (construction == BuildingType.None)
        {
            DestroyBuilding(cellPosition);
            return;
        }

        BuildingCostData buildingCostData = GetBuildingCostData(construction);  // Obtient les coûts de construction

        if (HasEnoughResources(buildingCostData))
        { 
            // Place la construction dans le TileDataMap
            if (tileData != null)
            {
                tileData.Building = construction;
                DeductResources(buildingCostData);

                // Placer la construction sur la Tilemap
                TileManager.Instance.PlaceBuildingTile(cellPosition, tileData.Building);

                // Ajout du bâtiment dans le dictionnaire du BuildingManager
                AddBuilding(cellPosition, tileData.Building);

                // Envoi du message (pour la fermeture de l'UI)
                OnBuildingConstructed?.Invoke();
            }
        }
        else
        {
            // Afficher un message d'erreur ou une UI indiquant qu'il n'y a pas assez de ressources
            Debug.Log("Pas assez de ressources pour construire ce bâtiment !");
        }
    }

    private void DestroyBuilding(Vector3Int cellPosition)
    {
        // suppression de la construiction sur la tilemap
        TileManager.Instance.RemoveBuilding(cellPosition);

        // suppresion de la construction dans le TileDataMap
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);
        tileData.Building = BuildingType.None;

        // retrait du batiment du dictionnaire
        if (_buildingsDataMap.ContainsKey(cellPosition))
        {
            _buildingsDataMap.Remove(cellPosition);
        }

        // Envoi du message (pour la fermeture de l'ui)
        OnBuildingConstructed?.Invoke();

    }

    public void AddBuilding(Vector3Int position, BuildingType buildingType)
    {
        BuildingData buildingData = new BuildingData(buildingType);
        _buildingsDataMap[position] = buildingData;
    }

    // Obtient les données de coût pour un bâtiment donné
    public BuildingCostData GetBuildingCostData(BuildingType buildingType)
    {
        // Recherche dans une liste ou base de données des coûts pour chaque type de bâtiment
        return buildingCostsList.Find(cost => cost.buildingType == buildingType);

    }

    public BuildingUpgradeData GetUpgradeData(BuildingType buildingType, int targetLevel)
    {
        return upgradeDataList.Find(upgrade =>
            upgrade.buildingType == buildingType && upgrade.level == targetLevel);
    }




    // Vérifie si les ressources sont suffisantes
    private bool HasEnoughResources(BuildingCostData buildingCostData)
    {
        foreach (var resourceCost in buildingCostData.resourceCosts)
        {
            if (!ResourceManager.Instance.HasEnoughResources(resourceCost.resourceType, resourceCost.amount))
            {

                // !!!!!!!!!!!!!
                // TODO : implémenter une message pour indiquer qu'il manque des ressources
                //!!!!!!!!!!!!!!
                return false;  // Pas assez de cette ressource
            }
        }
        return true;  // Toutes les ressources sont suffisantes
    }

    // Déduit les ressources après la construction
    private void DeductResources(BuildingCostData buildingCostData)
    {
        foreach (var resourceCost in buildingCostData.resourceCosts)
        {
            ResourceManager.Instance.DeductResources(resourceCost.resourceType, resourceCost.amount);
        }
    }


    public bool CanBuild(BuildingType type, Vector3Int cellPosition)
    {
        TileData tile = TileManager.Instance.GetTileData(cellPosition);
        if (tile == null || tile.Building != BuildingType.None || !tile.IsClaimed)
            return false;

        // Pré-calculs de conditions
        bool isGrass = TileManager.Instance.isTargetGroundOnTile(cellPosition, GroundType.Grass);
        bool isMountain = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.Mountain);
        bool noRelief = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.None);
        bool hasAdjacentRoadOrTown = directions.Any(dir => TileManager.Instance.IsRoadOrTown(cellPosition + dir, true));
        bool hasAdjacentWood = directions.Any(dir => TileManager.Instance.isTargetReliefOnTile(cellPosition + dir, ReliefType.Wood));
        bool hasAdjacentMountain = directions.Any(dir => TileManager.Instance.isTargetReliefOnTile(cellPosition + dir, ReliefType.Mountain));

        // Règles de construction selon le type
        switch (type)
        {
            case BuildingType.Road:
                return isGrass && noRelief && hasAdjacentRoadOrTown;

            case BuildingType.Lumberjack:
                return isGrass && noRelief && hasAdjacentWood && hasAdjacentRoadOrTown;

            case BuildingType.Temple:
                return isGrass && noRelief;

            case BuildingType.StoneMine:
                return isGrass && isMountain && hasAdjacentRoadOrTown;

            case BuildingType.StonePile:
            case BuildingType.WoodPile:
            case BuildingType.ManaPile:
                return isGrass && noRelief;

            default:
                return false;
        }
    }

    public bool HasEnoughResourcesToEvol(BuildingUpgradeData upgradeData)
    {
        foreach (var cost in upgradeData.upgradeCosts)
        {
            if (!ResourceManager.Instance.HasEnoughResources(cost.resourceType, cost.amount))
            {
                return false;
            }
        }
        return true;
    }

    public void DeductResourcesForEvol(BuildingUpgradeData upgradeData)
    {
        foreach (var cost in upgradeData.upgradeCosts)
        {
            ResourceManager.Instance.DeductResources(cost.resourceType, cost.amount);
        }
    }

    public void UpgradeBuilding(Vector3Int position)
    {
        if (!_buildingsDataMap.ContainsKey(position)) return;

        BuildingData data = _buildingsDataMap[position];
        TileData tiledata = TileManager.Instance.GetTileData(position);

        // verification du niveau de la capital. Niveau ville max = Niveau capital - 1
        if (data.Type == BuildingType.Town)
        {
            int capitalLevel = GetCapitalLevel();
            if (data.Level + 1 >= capitalLevel)
            {
                Debug.Log("Le niveau de la ville ne peut pas dépasser celui de la capitale.");
                return;
            }
            
        }

        BuildingUpgradeData upgradeData = upgradeDataList.Find(u =>
            u.buildingType == data.Type && u.level == data.Level + 1);

        if (upgradeData == null)
        {
            Debug.Log($"Pas d'amélioration dispo pour {data.Type} au niveau {data.Level + 1}.");
            return;
        }

        if (!HasEnoughResourcesToEvol(upgradeData))
        {
            Debug.Log("Pas assez de ressources.");
            return;
        }

        DeductResourcesForEvol(upgradeData); 
        data.Level++;

        ResourceManager.Instance.CalculResources();

        Debug.Log($"Bâtiment {data.Type} évolué au niveau {data.Level}");
    }

    public int GetCapitalLevel()
    {
        if (_buildingsDataMap.TryGetValue(Vector3Int.zero, out BuildingData capitalData) &&
            capitalData.Type == BuildingType.Capital)
        {
            return capitalData.Level;
        }

        Debug.LogWarning("La capitale n'a pas été trouvée à la position (0,0).");
        return 1; // Valeur de secours
    }


}
