using UnityEngine;

public class TileDataManager
{
    public TileData GetTileData(Vector3Int position)
    {
        return TileManager.Instance.TileDataMap.ContainsKey(position)
            ? TileManager.Instance.TileDataMap[position]
            : null;
    }

    public void SetTileData(Vector3Int pos, TileData data) => TileManager.Instance.TileDataMap[pos] = data;
}