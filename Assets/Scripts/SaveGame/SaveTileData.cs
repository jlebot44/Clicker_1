using UnityEngine;

[System.Serializable]
public class SavedTileData
{
    public Vector3Int position;
    public GroundType ground;
    public ReliefType relief;
    public BuildingType building;
    public int buildingLevel;
    public int initialFog;
    public int currentFog;
    public bool isConnectedToCapital;

    public SavedTileData(Vector3Int pos, TileData tileData)
    {
        position = pos;
        ground = tileData.Ground;
        relief = tileData.Relief;
        building = tileData.Building;
        buildingLevel = tileData.BuildingLevel;
        initialFog = tileData.InitialFog;
        currentFog = tileData.CurrentFog;
        isConnectedToCapital = tileData.IsConnectedToCapital;
    }
}