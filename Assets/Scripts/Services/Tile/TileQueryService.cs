using UnityEngine;

public static class TileQueryService
{
    public static bool IsTargetGround(Vector3Int cellPosition, GroundType expectedGroundType)
    {
        TileData tile = TileManager.Instance.DataManager.GetTileData(cellPosition);
        return tile != null && tile.Ground == expectedGroundType;
    }

    public static bool IsTargetRelief(Vector3Int cellPosition, ReliefType expectedReliefType)
    {
        TileData tile = TileManager.Instance.DataManager.GetTileData(cellPosition);
        return tile != null && tile.Relief == expectedReliefType;
    }


}