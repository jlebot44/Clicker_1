using UnityEngine;

public static class ConstructionService
{
    public static bool TryConstruct(BuildingType type, Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);
        if (tileData == null)
        {
            Debug.LogWarning("Tuile introuvable.");
            return false;
        }

        if (!BuildingManager.Instance.CanBuild(type, cellPosition))
        {
            Debug.Log("Conditions de construction non remplies.");
            return false;
        }

        var costData = BuildingManager.Instance.GetBuildingCostData(type);
        if (!BuildingResourceService.HasEnoughResources(costData.resourceCosts))
        {
            Debug.Log("Pas assez de ressources.");
            return false;
        }

        // Appliquer
        tileData.Building = type;
        BuildingResourceService.DeductResources(costData.resourceCosts);
        TileManager.Instance.PlaceBuildingTile(cellPosition, type);
        BuildingManager.Instance.AddBuilding(cellPosition, type);
        BuildingManager.NotifyConstruction();

        return true;
    }

    public static bool TryDestroy(Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);
        if (tileData == null || tileData.Building == BuildingType.None)
        {
            UIManager.Instance.ShowFloatingText("Aucun batiment à détruire ici", cellPosition, Color.red);
            return false;
        }

        if (tileData.Building == BuildingType.Town || tileData.Building == BuildingType.Capital)
        {
            UIManager.Instance.ShowFloatingText("Impossible de supprimer une ville", cellPosition, Color.red);
            return false;
        }

        if (tileData.Building == BuildingType.Road)
        {
            UIManager.Instance.ShowFloatingText("Impossible de supprimer une route pour le moment", cellPosition, Color.red);
            return false;
        }

        // Visuel
        TileManager.Instance.RemoveBuilding(cellPosition);

        // Données
        tileData.Building = BuildingType.None;
        BuildingManager.Instance.BuildingsDataMap.Remove(cellPosition);

        BuildingManager.NotifyConstruction();

        return true;
    }
}
