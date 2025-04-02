using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

[System.Serializable]
public class BuildingSaveData
{
    public List<SavedBuildingData> buildings;

    public BuildingSaveData(List<SavedBuildingData> buildings)
    {
        this.buildings = buildings;
    }
}

// Classe de données pour chaque building sauvegardée
[System.Serializable]
public class SavedBuildingData
{
    public Vector3Int position;
    public BuildingData buildingData;
    public SavedBuildingData(Vector3Int position, BuildingData buildingData)
    {
        this.position = position;
        this.buildingData = buildingData;
    }
}
