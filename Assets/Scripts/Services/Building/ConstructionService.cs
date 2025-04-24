using UnityEngine;

public static class ConstructionService
{
    public static bool TryConstruct(BuildingType type, Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.DataManager.GetTileData(cellPosition);
        if (tileData == null)
        {
            Debug.LogWarning("Tuile introuvable.");
            return false;
        }

        if (!BuildingManager.Instance.CanBuild(type, cellPosition))
        {
            UIManager.Instance.ShowFloatingText("Impossible de construire ici !", cellPosition, Color.red);
            return false;
        }

        var costData = BuildingManager.Instance.GetBuildingCostData(type);
        if (!BuildingResourceService.HasEnoughResources(costData.resourceCosts))
        {
            Debug.Log("Pas assez de ressources.");
            return false;
        }

        // Appliquer
        //tileData.Building = type;
        BuildingResourceService.DeductResources(costData.resourceCosts);
        TileManager.Instance.BuildingRenderer.PlaceBuilding(cellPosition, type);
        BuildingManager.Instance.AddBuilding(cellPosition, type);
        BuildingManager.NotifyConstruction();

        return true;
    }

    public static bool TryDestroy(Vector3Int cellPosition)
    {
        if (BuildingQueryService.GetBuildingType(cellPosition) == BuildingType.None)
        {
            UIManager.Instance.ShowFloatingText("Aucun batiment à détruire ici", cellPosition, Color.red);
            return false;
        }

        if (BuildingQueryService.GetBuildingType(cellPosition) == BuildingType.Town || BuildingQueryService.GetBuildingType(cellPosition) == BuildingType.Capital)
        {
            UIManager.Instance.ShowFloatingText("Impossible de supprimer une ville", cellPosition, Color.red);
            return false;
        }

        if (BuildingQueryService.GetBuildingType(cellPosition) == BuildingType.Road)
        {
            UIManager.Instance.ShowFloatingText("Impossible de supprimer une route pour le moment", cellPosition, Color.red);
            return false;
        }

        // Visuel
        TileManager.Instance.BuildingRenderer.RemoveBuilding(cellPosition);

        // Données
        //tileData.Building = BuildingType.None;
        BuildingManager.Instance.BuildingsDataMap.Remove(cellPosition);

        BuildingManager.NotifyConstruction();

        return true;
    }
}
