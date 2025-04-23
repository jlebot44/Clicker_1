// File: Scripts/Services/BuildingValidator.cs
using UnityEngine;
using System.Linq;

public static class BuildingValidatorService
{
    private static readonly Vector3Int[] directions = {
        Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
    };

    public static bool CanBuild(BuildingType type, Vector3Int cellPosition)
    {
        TileData tile = TileManager.Instance.DataManager.GetTileData(cellPosition);
        if (tile == null || tile.Building != BuildingType.None || !tile.IsClaimed)
            return false;

        bool isGrass = TileManager.Instance.DataManager.IsTargetGround(cellPosition, GroundType.Grass);
        bool isMountain = TileManager.Instance.DataManager.IsTargetRelief(cellPosition, ReliefType.Mountain);
        bool noRelief = TileManager.Instance.DataManager.IsTargetRelief(cellPosition, ReliefType.None);
        bool hasAdjacentRoadOrTown = directions.Any(dir => TileManager.Instance.DataManager.IsRoadOrTown(cellPosition + dir, true));
        bool hasAdjacentWood = directions.Any(dir => TileManager.Instance.DataManager.IsTargetRelief(cellPosition + dir, ReliefType.Wood));

        return type switch
        {
            BuildingType.Road => isGrass && noRelief && hasAdjacentRoadOrTown,
            BuildingType.Lumberjack => isGrass && noRelief && hasAdjacentWood && hasAdjacentRoadOrTown,
            BuildingType.Temple => isGrass && noRelief,
            BuildingType.StoneMine => isGrass && isMountain && hasAdjacentRoadOrTown,
            BuildingType.StonePile or BuildingType.WoodPile or BuildingType.ManaPile => isGrass && noRelief,
            _ => false
        };
    }

    public static bool CanEvolve(Vector3Int cellPosition)
    {
        var tileData = TileManager.Instance.DataManager.GetTileData(cellPosition);
        var buildingData = BuildingManager.Instance.GetBuildingData(cellPosition);
        var upgrade = BuildingManager.Instance.GetUpgradeData(tileData.Building, buildingData.Level + 1);
        return upgrade != null && BuildingManager.Instance.HasEnoughResourcesToEvol(upgrade);
    }
}
