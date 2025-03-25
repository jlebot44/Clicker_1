using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    // Événement pour notifier qu'une construction a été faite
    public static event Action<Vector3Int, TileData> OnBuildingConstructed;

    Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

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
            // sol classique, absence de relief, presencde d'au moins une case connectée adjacente
            if (tileData.Ground == GroundType.Grass && tileData.Relief == ReliefType.None &&
                directions.Any(dir => TileManager.Instance.IsRoadOrTown(cellPosition + dir, true))) 
            {
                options.Add("road");
            }
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

            // Décrémenter le nombre de tuile generatrice de mana
            RessourceManager.Instance.ManaGen--;

            OnBuildingConstructed?.Invoke(cellPosition, selectedTileData);
        }
    }
}
