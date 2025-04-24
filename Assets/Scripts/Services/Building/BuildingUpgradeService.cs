using UnityEngine;

public class BuildingUpgradeService
{
    public static void TryUpgrade(Vector3Int position)
    {
        if (!BuildingManager.Instance.BuildingsDataMap.ContainsKey(position))
            return;

        if (!BuildingValidatorService.CanEvolve(position))
            return;

        var buildingData = BuildingManager.Instance.BuildingsDataMap[position];

        var upgradeData = BuildingManager.Instance.GetUpgradeData(BuildingQueryService.GetBuildingType(position), buildingData.Level + 1);
        if (upgradeData == null) return;

        BuildingManager.Instance.DeductResourcesForEvol(upgradeData);
        buildingData.Level++;

        ResourceManager.Instance.CalculResources();

        BuildingManager.NotifyConstruction();
    }

}
