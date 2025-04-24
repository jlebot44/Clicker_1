using UnityEngine.Tilemaps;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.VisualScripting;
using System;

public class RoadTileRenderer
{

    private Vector3Int[] _directions = {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };


    public void PlaceRoad(Vector3Int position)
    {
        // 1. Enregistrer la route
        if (!BuildingQueryService.HasBuilding(position))
            BuildingManager.Instance.AddBuilding(position, BuildingType.Road);
        foreach (var dir in _directions)
        {
            Vector3Int adj = position + dir;
            if (BuildingQueryService.IsRoadOrTown(adj, true))
                UpdateTileForRoad(adj);
        }
        UpdateTileForRoad(position);
    }

    private void UpdateTileForRoad(Vector3Int position)
    {
        int roadConfig = 0;
        Debug.Log("test position : " + position);
        foreach (var dir in _directions)
        {
            Vector3Int neighbor = position + dir;
            Debug.Log(neighbor + " : " +BuildingQueryService.GetBuildingType(neighbor) + " | " + (BuildingQueryService.IsRoadOrTown(neighbor, true)));
            var data = TileManager.Instance.DataManager.GetTileData(neighbor);
            if (BuildingQueryService.IsRoadOrTown(neighbor, true))
            {
                roadConfig |= GetBinaryMask(dir);
                data.IsConnectedToCapital = true;
            }
        }

        var current = TileManager.Instance.DataManager.GetTileData(position);
        if (current != null && (BuildingQueryService.GetBuildingType(position) == BuildingType.Town || BuildingQueryService.GetBuildingType(position) == BuildingType.Capital)) return;

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

    public void UpdateSurroundingRoads(Vector3Int position)
    {
        foreach (var dir in _directions)
        {
            Vector3Int neighbor = position + dir;
            if (BuildingQueryService.IsRoadOrTown(neighbor))
            {
                UpdateTileForRoad(neighbor);
            }
        }
    }
}
