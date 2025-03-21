using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    // Événement pour notifier qu'une construction a été faite
    public static event Action<Vector3Int, TileData> OnBuildingConstructed;

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

    // Retourne les constructions disponibles
    public List<string> GetAvailableConstructions(Vector3Int cellPosition)
    {
        List<string> options = new List<string>();
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);

        if (tileData != null && tileData.Building == BuildingType.None)
        {
            if (tileData.Ground == GroundType.Grass && tileData.Relief == ReliefType.None && (TileManager.Instance.IsRoadOrTown(cellPosition + Vector3Int.up, true) || TileManager.Instance.IsRoadOrTown(cellPosition + Vector3Int.down, true) || TileManager.Instance.IsRoadOrTown(cellPosition + Vector3Int.left, true) || TileManager.Instance.IsRoadOrTown(cellPosition + Vector3Int.right, true)))
                options.Add("road");
        }

        return options;
    }

    // Logique de construction
    public void Build(string construction, Vector3Int cellPosition)
    {
        TileData selectedTileData = TileManager.Instance.GetTileData(cellPosition);

        if (selectedTileData != null)
        {
            switch (construction)
            {
                case "road":
                    selectedTileData.Building = BuildingType.Road;
                    break;
            }

            // Placer la construction sur la Tilemap
            TileManager.Instance.PlaceBuildingTile(cellPosition, selectedTileData.Building);

            OnBuildingConstructed?.Invoke(cellPosition, selectedTileData);
        }
    }
}
