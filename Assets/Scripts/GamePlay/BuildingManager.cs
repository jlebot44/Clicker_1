using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    private void Awake()
    {
        // Assure-toi qu'il y a une seule instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Retourne les constructions disponibles en fonction des données de la tuile
    public List<string> GetAvailableConstructions(Vector3Int cellPosition)
    {
        List<string> options = new List<string>();

        // Récupérer les données de la tuile via le TileManager en utilisant la position de la cellule
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);

        if (tileData != null && tileData.Building == BuildingType.None)
        {
            if (tileData.Ground == GroundType.Grass)
                options.Add("road");

            // Si tu veux ajouter plus de constructions basées sur le type de terrain :
            // if (tileData.Ground == GroundType.Grass) options.Add("Farm");
            // if (tileData.Ground == GroundType.Hill) options.Add("Mine");
        }

        return options;
    }

    // Logique de construction
    public void Build(string construction, Vector3Int cellPosition)
    {
        // Récupérer les données de la tuile
        TileData selectedTileData = TileManager.Instance.GetTileData(cellPosition);

        if (selectedTileData != null)
        {
            Debug.Log($"Construction de {construction} sur {cellPosition}");

            // Applique la construction à la tuile
            switch (construction)
            {
                case "road":
                    selectedTileData.Building = BuildingType.Road;
                    break;
                    // Ajouter d'autres types de constructions si nécessaire
                    // case "Farm":
                    //     selectedTileData.Building = BuildingType.Farm;
                    //     break;
                    // case "Mine":
                    //     selectedTileData.Building = BuildingType.Mine;
                    //     break;
            }

            // Placer la tile dans la Tilemap
            TileManager.Instance.PlaceBuildingTile(cellPosition, selectedTileData.Building);

            // Rafraîchir l'affichage après la construction
            TileClickHandler.Instance.UpdateTileInfo(selectedTileData);
        }
    }
}
