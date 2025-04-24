using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveLoadManager : MonoBehaviour
{
    private Dictionary<Vector3Int, TileData> _tileDataMap;
    private Dictionary<Vector3Int, BuildingData> _buildingDataMap;
    private EncryptionManager _encryptionManager;

    [SerializeField] private Tilemap _fogTilemap;
    [SerializeField] private Tilemap _reliefTilemap;
    [SerializeField] private Tilemap _buildingTilemap;

    private PauseGame _pauseGame;

    private void Start()
    {
        _tileDataMap = TileManager.Instance.TileDataMap;
        _buildingDataMap = BuildingManager.Instance.BuildingsDataMap;
        _pauseGame = GetComponent<PauseGame>();
        _encryptionManager = new EncryptionManager();
    }

    public void Save()
    {
        SaveClaimedTiles();
        SaveResources();
        SaveBuildings();
        SaveBonuses();
        TogglePauseState();
    }

    public void Load()
    {
        LoadClaimedTiles();
        LoadBuildings();
        LoadResources();
        LoadBonuses();
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
                //tileData.Building = savedTile.tileData.Building;
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



    private void SaveResources()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("RessourceManager.Instance est null ! Impossible de sauvegarder les ressources.");
            return;
        }

        ResourceManager ressourceManager = ResourceManager.Instance;
        SaveResourceData saveRessourceData = new SaveResourceData(
            ressourceManager.GetResource(ResourceType.Mana), ressourceManager.GetResource(ResourceType.Gold),
            ressourceManager.GetResource(ResourceType.Wood), ressourceManager.GetResource(ResourceType.Stone),
            ressourceManager.UpdateInterval, ressourceManager.Tiles
        ); 
        SaveToFile(saveRessourceData, "resources.json");
    }

    private void LoadResources()
    {
        SaveResourceData saveRessourceData = LoadFromFile<SaveResourceData>("resources.json");
        if (saveRessourceData == null) return;

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("RessourceManager.Instance est null ! Assurez-vous qu'il est initialisé.");
            return;
        }

        ResourceManager ressourceManager = ResourceManager.Instance;
        ressourceManager.SetResource(ResourceType.Mana, saveRessourceData.Mana);
        ressourceManager.SetResource(ResourceType.Gold, saveRessourceData.Gold);
        ressourceManager.SetResource(ResourceType.Wood, saveRessourceData.Wood);
        ressourceManager.SetResource(ResourceType.Stone, saveRessourceData.Stone);
        ressourceManager.UpdateInterval = saveRessourceData.UpdateInterval;
        ressourceManager.Tiles = saveRessourceData.Tiles;

        Debug.Log("Ressources chargées avec succès !");
    }

    private void SaveBuildings()
    {
        List<SavedBuildingData> savedBuildings = new List<SavedBuildingData>();

        foreach (var building in _buildingDataMap)
        {
            BuildingData buildingData = building.Value;
            Vector3Int position = building.Key;
            savedBuildings.Add(new SavedBuildingData(position, buildingData));
        }
        SaveToFile(new BuildingSaveData(savedBuildings), "buildings_save.json");
    }



    private void LoadBuildings()
    {
        BuildingSaveData savedData = LoadFromFile<BuildingSaveData>("buildings_save.json");
        if (savedData == null || savedData.buildings == null) return;

        foreach (SavedBuildingData savedBuilding in savedData.buildings)
        {
            _buildingDataMap[savedBuilding.position] = savedBuilding.buildingData;
        }

        // Déclenchement des calculs sur les ressources pour mettre à jour l'interface
        ResourceManager.Instance.CalculRessourcesPerTurn();
        ResourceManager.Instance.CalculCapacity();
    }

    private void SaveBonuses()
    {
        var result = new List<SaveBonusEntry>();
        var all = ShrineBonusManager.Instance.GetAllKnownBonuses();

        

        foreach (var bonus in all)
        {
            bool activated = ShrineBonusManager.Instance.IsActivated(bonus);
            result.Add(new SaveBonusEntry(bonus.bonusName, activated));
        }

        SaveToFile(new BonusSaveData(result), "bonuses.json"); ;
    }

    private void LoadBonuses()
    {
        var data = LoadFromFile<BonusSaveData>("bonuses.json");
        if (data == null || data.bonuses == null || data.bonuses.Count == 0)
        {
            Debug.LogWarning("Aucun bonus chargé depuis bonuses.json !");
            return;
        }

        foreach (var entry in data.bonuses)
        {
            var bonus = ShrinePlacer.Instance.FindBonusDataByName(entry.BonusId);

            if (bonus != null)
            {
                ShrineBonusManager.Instance.RegisterBonus(bonus);

                if (entry.IsActivated)
                    ShrineBonusManager.Instance.ActivateBonus(bonus);
            }
        }
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
            //Debug.Log($"[LoadFromFile] {fileName} contenu déchiffré : {json}");
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

    private void UpdateTileOnMap(Vector3Int position, TileBase reliefTile, TileBase buildingTile)
    {
        if (reliefTile != null)
            _reliefTilemap.SetTile(position, reliefTile);

        if (buildingTile != null)
            _buildingTilemap.SetTile(position, buildingTile);
    }
}
