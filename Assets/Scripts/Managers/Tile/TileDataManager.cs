using UnityEngine;

public class TileDataManager
{
    public TileData GetTileData(Vector3Int position)
    {
        return TileManager.Instance.TileDataMap.ContainsKey(position)
            ? TileManager.Instance.TileDataMap[position]
            : null;
    }

    public bool IsRoadOrTown(Vector3Int position, bool requiredConnectedToCapital)
    {
        var data = GetTileData(position);
        if (data == null || !data.IsClaimed) return false;

        bool isRouteOrVille = data.Building == BuildingType.Road ||
                              data.Building == BuildingType.Town ||
                              data.Building == BuildingType.Capital;

        return requiredConnectedToCapital ? isRouteOrVille && data.IsConnectedToCapital : isRouteOrVille;
    }

    public bool IsTargetRelief(Vector3Int pos, ReliefType relief) =>
        GetTileData(pos) is TileData data && data.Relief == relief && data.IsClaimed;

    public bool IsTargetBuilding(Vector3Int pos, BuildingType building) =>
        GetTileData(pos) is TileData data && data.Building == building && data.IsClaimed;

    public bool IsTargetGround(Vector3Int pos, GroundType ground) =>
        GetTileData(pos) is TileData data && data.Ground == ground && data.IsClaimed;

    public void SetTileData(Vector3Int pos, TileData data) => TileManager.Instance.TileDataMap[pos] = data;
}