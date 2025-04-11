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
        TileData tile = TileManager.Instance.GetTileData(cellPosition);
        if (tile == null || tile.Building != BuildingType.None || !tile.IsClaimed)
            return false;

        bool isGrass = TileManager.Instance.isTargetGroundOnTile(cellPosition, GroundType.Grass);
        bool isMountain = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.Mountain);
        bool noRelief = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.None);
        bool hasAdjacentRoadOrTown = directions.Any(dir => TileManager.Instance.IsRoadOrTown(cellPosition + dir, true));
        bool hasAdjacentWood = directions.Any(dir => TileManager.Instance.isTargetReliefOnTile(cellPosition + dir, ReliefType.Wood));

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
        var tileData = TileManager.Instance.GetTileData(cellPosition);
        var buildingData = BuildingManager.Instance.GetBuildingData(cellPosition);

        int capitalLevel = BuildingManager.Instance.GetCapitalLevel();
        if (buildingData.Type != BuildingType.Capital && buildingData.Level + 1 >= capitalLevel)
        {
            Debug.Log("Le niveau de la ville ne peut pas dépasser celui de la capitale.");
            return false;
        }

        var upgrade = BuildingManager.Instance.GetUpgradeData(tileData.Building, buildingData.Level + 1);
        return upgrade != null && BuildingManager.Instance.HasEnoughResourcesToEvol(upgrade);
    }
}
