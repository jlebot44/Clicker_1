using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveLoadManager : MonoBehaviour
{
    private Dictionary<Vector3Int, TileData> _tileDataMap;

    [SerializeField] private EncryptionManager _encryptionManager;

    [SerializeField] private Tilemap _fogTilemap;
    [SerializeField] private Tilemap _reliefTilemap;
    [SerializeField] private Tilemap _buildingTilemap;

    // pour relancer le jeu
    private PauseGame pauseGame;



    private void Start()
    {
        _tileDataMap = TileManager.Instance.TileDataMap;
        pauseGame = GetComponent<PauseGame>();
        _encryptionManager = new EncryptionManager();        

    }


    public void Save()
    {
        SaveClaimedTiles();
        SaveResources();

        //relancer le jeu après la sauvegarde
        pauseGame.IsPaused = !pauseGame.IsPaused;
    }

    public void Load()
    {
        LoadClaimedTiles();
        LoadResources();

        //relancer le jeu après le chargement
        pauseGame.IsPaused = !pauseGame.IsPaused;
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

        // Cryptage de la chaine de caractère
        string cryptedJson = _encryptionManager.Encrypt(json);

        // Sauvegarder dans un fichier
        string filePath = Path.Combine(Application.persistentDataPath, "tile_save.json");
        File.WriteAllText(filePath, cryptedJson);

        Debug.Log("Sauvegarde des tuiles réussie : " + filePath);
    }

    public void LoadClaimedTiles()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "tile_save.json");

        if (File.Exists(filePath))
        {
            // Lire le contenu du fichier
            string CryptedJson = File.ReadAllText(filePath);

            // decryptage du fichier
            string json = _encryptionManager.Decrypt(CryptedJson);

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

    private void SaveResources()
    {
        RessourceManager ressourceManager = RessourceManager.Instance;
        SaveRessourceData SaveRessourceData = new SaveRessourceData(ressourceManager.Mana, ressourceManager.ManaPerLevel, ressourceManager.UpdateInterval, ressourceManager.Tiles);
        string json = JsonUtility.ToJson(SaveRessourceData, true);
        string cryptedJson = _encryptionManager.Encrypt(json);
        string filePath = Path.Combine(Application.persistentDataPath, "resources.json");
        File.WriteAllText(filePath, cryptedJson);
        Debug.Log("Ressources sauvegardées : " + filePath);
    }

    private void LoadResources()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "resources.json");
        if (File.Exists(filePath))
        {
            string cryptedJson = File.ReadAllText(filePath);
            string json = _encryptionManager.Decrypt(cryptedJson);  
            SaveRessourceData SaveRessourceData = JsonUtility.FromJson<SaveRessourceData>(json);
            RessourceManager ressourceManager = RessourceManager.Instance;
            ressourceManager.Mana = SaveRessourceData.mana;
            ressourceManager.ManaPerLevel = SaveRessourceData.manaPerLevel;
            ressourceManager.UpdateInterval = SaveRessourceData.updateInterval;
            ressourceManager.Tiles = SaveRessourceData.tiles;

            Debug.Log("Ressources chargées");
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde de ressources trouvée !");
        }
    }



}



