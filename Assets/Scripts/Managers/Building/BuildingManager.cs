using System;
using System.Collections.Generic;
using UnityEngine;


public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    // Événement pour notifier qu'une construction a été faite
    public static event Action OnBuildingConstructed;

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
    public void Build(BuildingType type, Vector3Int position)
    {
        if (type == BuildingType.None)
        {
            ConstructionService.TryDestroy(position);
        }
        else
        {
            ConstructionService.TryConstruct(type, position);
        }
    }


    public void AddBuilding(Vector3Int position, BuildingType buildingType)
    {
        BuildingData buildingData = new BuildingData(buildingType);
        _buildingsDataMap[position] = buildingData;
    }

    public void AddShrineBuilding(Vector3Int position, ShrineBonusData bonusData)
    {
        ShrineBuildingData shrineData = new ShrineBuildingData(BuildingType.BonusShrine, bonusData);
        _buildingsDataMap[position] = shrineData;
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


    public bool CanBuild(BuildingType type, Vector3Int cellPosition)
    {
        return BuildingValidatorService.CanBuild(type, cellPosition);     
    }

    public bool HasEnoughResourcesToEvol(BuildingUpgradeData upgradeData)
    {
        return BuildingResourceService.HasEnoughResources(upgradeData.upgradeCosts);
    }

    public void DeductResourcesForEvol(BuildingUpgradeData upgradeData)
    {
        BuildingResourceService.DeductResources(upgradeData.upgradeCosts);
    }

    public void UpgradeBuilding(Vector3Int position)
    {
        BuildingUpgradeService.TryUpgrade(position);
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

    public static void NotifyConstruction()
    {
        OnBuildingConstructed?.Invoke();
    }
}
