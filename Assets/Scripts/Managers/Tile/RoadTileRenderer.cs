using UnityEngine.Tilemaps;
using UnityEngine;

public class RoadTileRenderer
{
    public void PlaceRoad(Vector3Int position)
    {
        UpdateTileForRoad(position);
        foreach (var dir in TileManager.Instance.Directions)
        {
            Vector3Int adj = position + dir;
            if (TileManager.Instance.DataManager.IsRoadOrTown(adj, false))
                UpdateTileForRoad(adj);
        }
    }

    private void UpdateTileForRoad(Vector3Int position)
    {
        int roadConfig = 0;
        foreach (var dir in TileManager.Instance.Directions)
        {
            Vector3Int neighbor = position + dir;
            var data = TileManager.Instance.DataManager.GetTileData(neighbor);
            if (data != null && (data.Building == BuildingType.Road || data.Building == BuildingType.Town || data.Building == BuildingType.Capital))
            {
                roadConfig |= GetBinaryMask(dir);
                data.IsConnectedToCapital = true;
            }
        }

        var current = TileManager.Instance.DataManager.GetTileData(position);
        if (current != null && (current.Building == BuildingType.Town || current.Building == BuildingType.Capital)) return;

        if (TileManager.Instance.RoadTypeMapping.TryGetValue(roadConfig, out TileBase tile))
        {
            TileManager.Instance.BuildingTilemap.SetTile(position, tile);
        }
    }

    private int GetBinaryMask(Vector3Int direction)
    {
        if (direction == Vector3Int.left) return 0b1000;
        if (direction == Vector3Int.right) return 0b0100;
        if (direction == Vector3Int.up) return 0b0010;
        if (direction == Vector3Int.down) return 0b0001;
        return 0;
    }
}
