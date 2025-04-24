using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public static class BuildingQueryService
{
    public static BuildingType GetBuildingType(Vector3Int pos)
    {
        return BuildingManager.Instance.GetBuildingData(pos)?.Type ?? BuildingType.None;
    }

    public static int? GetLevel(Vector3Int pos)
    {
        return BuildingManager.Instance.GetBuildingData(pos)?.Level;
    }

    public static bool IsType(Vector3Int pos, BuildingType type)
    {
        return GetBuildingType(pos) == type;
    }

    public static bool HasBuilding(Vector3Int pos)
    {
        return GetBuildingType(pos) != BuildingType.None;
    }

    public static BuildingType DetectBuildingTypeFromTilemap(Vector3Int pos)
    {
        TileBase tile = TileManager.Instance.BuildingTilemap.GetTile(pos);
        if (tile == null) return BuildingType.None;

        string tileName = tile.name?.ToLower();
        if (string.IsNullOrEmpty(tileName)) return BuildingType.None;

        if (tileName.Contains("town")) return BuildingType.Town;
        if (tileName.Contains("capital")) return BuildingType.Capital;
        if (tileName.Contains("shrine")) return BuildingType.BonusShrine;

        return BuildingType.Other;
    }

    public static bool IsRoadOrTown(Vector3Int cellPosition, bool includeCapital = false)
    {
        return BuildingManager.Instance.GetBuildingData(cellPosition) != null && (
            GetBuildingType(cellPosition) == BuildingType.Road ||
            GetBuildingType(cellPosition) == BuildingType.Town ||
            (includeCapital && GetBuildingType(cellPosition) == BuildingType.Capital));
    }


}