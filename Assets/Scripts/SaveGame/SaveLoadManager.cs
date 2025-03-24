using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveLoadManager : MonoBehaviour
{
    private Dictionary<Vector3Int, TileData> _tileDataMap;

    [SerializeField] private Tilemap _fogTilemap;
    [SerializeField] private Tilemap _reliefTilemap;
    [SerializeField] private Tilemap _buildingTilemap;

    private void Start()
    {
        _tileDataMap = TileManager.Instance.TileDataMap;
    }

    public void SaveClaimedTiles()
    {
        List<SavedTileData> savedTiles = new List<SavedTileData>();

        // Parcourir toutes les tuiles et ajouter celles qui sont réclamées
        foreach (var kvp in _tileDataMap)
        {
            TileData tileData = kvp.Value;
            if (tileData.IsClaimed)
            {
                Vector3Int position = kvp.Key;

                // Récupérer les tuiles de chaque Tilemap
                TileBase reliefTile = _reliefTilemap.GetTile(position);
                TileBase buildingTile = _buildingTilemap.GetTile(position);

                // Sauvegarder les données nécessaires
                SavedTileData savedTile = new SavedTileData(position, tileData, reliefTile, buildingTile);
                savedTiles.Add(savedTile);
            }
        }

        // Convertir en JSON
        string json = JsonUtility.ToJson(new TileSaveData(savedTiles), true);

        // Sauvegarder dans un fichier
        string filePath = Path.Combine(Application.persistentDataPath, "tile_save.json");
        File.WriteAllText(filePath, json);

        Debug.Log("Sauvegarde des tuiles réussie : " + filePath);
    }

    public void LoadClaimedTiles()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "tile_save.json");

        if (File.Exists(filePath))
        {
            // Lire le contenu du fichier
            string json = File.ReadAllText(filePath);

            // Désérialiser le JSON en un objet TileSaveData
            TileSaveData savedData = JsonUtility.FromJson<TileSaveData>(json);

            foreach (SavedTileData savedTile in savedData.tiles)
            {
                Vector3Int position = savedTile.position;
                _fogTilemap.SetTile(position, null);

                if (_tileDataMap.ContainsKey(position))
                {
                    TileData tileData = _tileDataMap[position];
                    //tileData = savedTile.tileData;
                    tileData.IsClaimed = true;
                    tileData.Ground = savedTile.tileData.Ground;
                    tileData.Relief = savedTile.tileData.Relief;
                    tileData.Building = savedTile.tileData.Building;
                    tileData.BuildingLevel = savedTile.tileData.BuildingLevel;
                    tileData.InitialFog = savedTile.tileData.InitialFog;
                    tileData.CurrentFog = savedTile.tileData.CurrentFog;
                    tileData.IsConnectedToCapital = savedTile.tileData.IsConnectedToCapital;

                    // Restaurer la tuile sur la tilemap
                    UpdateTileOnMap(position, savedTile.relief, savedTile.building);
                }
            }

            Debug.Log("Chargement des tuiles réussie.");
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée !");
        }
    }

    private void UpdateTileOnMap(Vector3Int position, TileBase reliefTile, TileBase buildingTile)
    {
        // Restaurer la tuile de chaque Tilemap
        if (reliefTile != null)
            _reliefTilemap.SetTile(position, reliefTile);

        if (buildingTile != null)
            _buildingTilemap.SetTile(position, buildingTile);


    }


}



