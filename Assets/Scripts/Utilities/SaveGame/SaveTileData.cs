using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;

// Classe de conteneur pour sérialiser les tuiles sauvegardées
[System.Serializable]
public class TileSaveData
{
    public List<SavedTileData> tiles;

    public TileSaveData(List<SavedTileData> tiles)
    {
        this.tiles = tiles;
    }
}

// Classe de données pour chaque tuile sauvegardée
[System.Serializable]
public class SavedTileData
{
    public Vector3Int position;
    public TileData tileData;
    public TileBase relief;
    public TileBase building;
    

    public SavedTileData(Vector3Int position, TileData tileData, TileBase building, TileBase relief)
    {
        this.position = position;
        this.tileData = tileData;
        this.building = building;
        this.relief = relief;
    }
}
