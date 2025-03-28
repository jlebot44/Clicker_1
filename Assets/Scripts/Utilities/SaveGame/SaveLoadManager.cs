using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveLoadManager : MonoBehaviour
{
    private Dictionary<Vector3Int, TileData> _tileDataMap;
    private EncryptionManager _encryptionManager;

    [SerializeField] private Tilemap _fogTilemap;
    [SerializeField] private Tilemap _reliefTilemap;
    [SerializeField] private Tilemap _buildingTilemap;

    private PauseGame _pauseGame;

    private void Start()
    {
        _tileDataMap = TileManager.Instance.TileDataMap;
        _pauseGame = GetComponent<PauseGame>();
        _encryptionManager = new EncryptionManager();
    }

    public void Save()
    {
        SaveClaimedTiles();
        SaveResources();
        TogglePauseState();
    }

    public void Load()
    {
        LoadClaimedTiles();
        LoadResources();
        TogglePauseState();
    }

    private void TogglePauseState()
    {
        if (_pauseGame != null)
        {
            _pauseGame.IsPaused = !_pauseGame.IsPaused;
        }
    }

    private void SaveClaimedTiles()
    {
        List<SavedTileData> savedTiles = new List<SavedTileData>();

        foreach (var kvp in _tileDataMap)
        {
            TileData tileData = kvp.Value;
            if (tileData.IsClaimed)
            {
                Vector3Int position = kvp.Key;
                TileBase reliefTile = _reliefTilemap.GetTile(position);
                TileBase buildingTile = _buildingTilemap.GetTile(position);

                savedTiles.Add(new SavedTileData(position, tileData, reliefTile, buildingTile));
            }
        }

        SaveToFile(new TileSaveData(savedTiles), "tile_save.json");
    }

    private void LoadClaimedTiles()
    {
        TileSaveData savedData = LoadFromFile<TileSaveData>("tile_save.json");
        if (savedData == null || savedData.tiles == null) return;

        foreach (SavedTileData savedTile in savedData.tiles)
        {
            Vector3Int position = savedTile.position;
            _fogTilemap.SetTile(position, null);

            if (_tileDataMap.TryGetValue(position, out TileData tileData))
            {
                tileData.IsClaimed = true;
                tileData.Ground = savedTile.tileData.Ground;
                tileData.Relief = savedTile.tileData.Relief;
                tileData.Building = savedTile.tileData.Building;
                tileData.InitialFog = savedTile.tileData.InitialFog;
                tileData.CurrentFog = savedTile.tileData.CurrentFog;
                tileData.IsConnectedToCapital = savedTile.tileData.IsConnectedToCapital;

                UpdateTileOnMap(position, savedTile.relief, savedTile.building);
            }
            else
            {
                Debug.LogWarning($"Position non trouvée dans TileDataMap : {position}");
            }
        }
    }

    private void UpdateTileOnMap(Vector3Int position, TileBase reliefTile, TileBase buildingTile)
    {
        if (reliefTile != null)
            _reliefTilemap.SetTile(position, reliefTile);

        if (buildingTile != null)
            _buildingTilemap.SetTile(position, buildingTile);
    }

    private void SaveResources()
    {
        if (RessourceManager.Instance == null)
        {
            Debug.LogError("RessourceManager.Instance est null ! Impossible de sauvegarder les ressources.");
            return;
        }

        RessourceManager ressourceManager = RessourceManager.Instance;
        SaveRessourceData saveRessourceData = new SaveRessourceData(
            ressourceManager.Mana, ressourceManager.Gold,
            ressourceManager.Wood, ressourceManager.Stone,
            ressourceManager.UpdateInterval, ressourceManager.Tiles
        ); 
        SaveToFile(saveRessourceData, "resources.json");
    }

    private void LoadResources()
    {
        SaveRessourceData saveRessourceData = LoadFromFile<SaveRessourceData>("resources.json");
        if (saveRessourceData == null) return;

        if (RessourceManager.Instance == null)
        {
            Debug.LogError("RessourceManager.Instance est null ! Assurez-vous qu'il est initialisé.");
            return;
        }

        RessourceManager ressourceManager = RessourceManager.Instance;
        ressourceManager.Mana = saveRessourceData.Mana;
        ressourceManager.Gold = saveRessourceData.Gold;
        ressourceManager.Wood = saveRessourceData.Wood;
        ressourceManager.Stone = saveRessourceData.Stone;
        ressourceManager.UpdateInterval = saveRessourceData.UpdateInterval;
        ressourceManager.Tiles = saveRessourceData.Tiles;

        Debug.Log("Ressources chargées avec succès !");
    }

    private void SaveToFile<T>(T data, string fileName)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            string cryptedJson = _encryptionManager.Encrypt(json);
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(filePath, cryptedJson);
            Debug.Log($"Sauvegarde réussie : {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erreur lors de la sauvegarde de {fileName} : {ex.Message}");
        }
    }

    private T LoadFromFile<T>(string fileName) where T : class
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Aucune sauvegarde trouvée : {fileName}");
            return null;
        }

        try
        {
            string cryptedJson = File.ReadAllText(filePath);
            string json = _encryptionManager.Decrypt(cryptedJson);
            T data = JsonUtility.FromJson<T>(json);

            if (data == null)
            {
                Debug.LogError($"Erreur de désérialisation : {fileName}");
            }

            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erreur lors du chargement de {fileName} : {ex.Message}");
            return null;
        }
    }
}
