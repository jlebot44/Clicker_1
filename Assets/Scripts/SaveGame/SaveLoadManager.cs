using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{

    private Dictionary<Vector3Int, TileData> _tileDataMap;


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
                SavedTileData savedTile = new SavedTileData(kvp.Key, tileData);
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

    public void LoadClaimedTiles()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "tile_save.json");

        // Vérifier si le fichier existe
        if (File.Exists(filePath))
        {
            // Lire le contenu du fichier
            string json = File.ReadAllText(filePath);

            // Désérialiser le JSON en un objet TileSaveData
            TileSaveData savedData = JsonUtility.FromJson<TileSaveData>(json);

            // Parcourir les tuiles sauvegardées et rétablir leur état
            foreach (SavedTileData savedTile in savedData.tiles)
            {
                Vector3Int position = savedTile.position;
                if (_tileDataMap.ContainsKey(position))
                {
                    TileData tileData = _tileDataMap[position];
                    tileData.IsClaimed = true;
                    tileData.Ground = savedTile.ground;
                    tileData.Relief = savedTile.relief;
                    tileData.Building = savedTile.building;
                    tileData.BuildingLevel = savedTile.buildingLevel;
                    tileData.InitialFog = savedTile.initialFog;
                    tileData.CurrentFog = savedTile.currentFog;
                    tileData.IsConnectedToCapital = savedTile.isConnectedToCapital; 

                    // Restaurer la tuile sur la tilemap
                    // (Optionnel) Tu peux ici aussi restaurer visuellement la tuile sur la Tilemap, si nécessaire.
                    //UpdateTileOnMap(position, tileData);
                }
            }

            Debug.Log("Chargement des tuiles réussie.");
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée !");
        }
    }

}
