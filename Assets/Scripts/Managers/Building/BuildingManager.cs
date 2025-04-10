using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    // �v�nement pour notifier qu'une construction a �t� faite
    public static event Action OnBuildingConstructed;

    readonly Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    // Liste des co�ts de construction pour chaque b�timent
    public List<BuildingCostData> buildingCostsList;



    // Dictionnaire global des b�timents
    [SerializeField] private Dictionary<Vector3Int, BuildingData> _buildingsDataMap = new Dictionary<Vector3Int, BuildingData>();

    public Dictionary<Vector3Int, BuildingData> BuildingsDataMap => _buildingsDataMap;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        AddBuilding(Vector3Int.zero, BuildingType.Town);
    }

    // Retourne les constructions disponibles
    //public List<string> GetAvailableConstructions(Vector3Int cellPosition)
    //{
    //    List<string> options = new List<string>();

    //    // Pr�-calcul des conditions
    //    bool isGrass = TileManager.Instance.isTargetGroundOnTile(cellPosition, GroundType.Grass);
    //    bool isMountain = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.Mountain);
    //    bool noRelief = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.None);
    //    bool hasAdjacentRoadOrTown = directions.Any(dir => TileManager.Instance.IsRoadOrTown(cellPosition + dir, true));
    //    bool hasAdjacentWood = directions.Any(dir => TileManager.Instance.isTargetReliefOnTile(cellPosition + dir, ReliefType.Wood));
    //    bool hasAdjacentMountain = directions.Any(dir => TileManager.Instance.isTargetReliefOnTile(cellPosition + dir, ReliefType.Mountain));

    //    if (TileManager.Instance.GetTileData(cellPosition) != null)
    //    {
    //        if (TileManager.Instance.isTargetBuildingOnTile(cellPosition, BuildingType.None))
    //        {
    //            // Ajout des constructions possibles
    //            if (isGrass && noRelief && hasAdjacentRoadOrTown)
    //                options.Add(BuildingType.Road.ToString());
    //            if (isGrass && noRelief && hasAdjacentWood && hasAdjacentRoadOrTown)
    //                options.Add(BuildingType.Lumberjack.ToString());
    //            if (isGrass && noRelief)
    //                options.Add(BuildingType.Temple.ToString());
    //            if (isGrass && isMountain && hasAdjacentRoadOrTown)
    //                options.Add(BuildingType.StoneMine.ToString());
    //            if (isGrass && noRelief)
    //                options.Add(BuildingType.StonePile.ToString());
    //            if (isGrass && noRelief)
    //                options.Add(BuildingType.WoodPile.ToString());
    //            if (isGrass && noRelief)
    //                options.Add(BuildingType.ManaPile.ToString());
    //        }
    //        else
    //        {
    //            options.Add(BuildingType.None.ToString()); // Utilisation de l'�num�ration pour "destroy"
    //        }
    //    }
    //    return options;
    //}

    // Logique de construction
    public void Build(BuildingType construction, Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);
        if (construction == BuildingType.None)
        {
            DestroyBuilding(cellPosition);
            return;
        }

        BuildingCostData buildingCostData = GetBuildingCostData(construction);  // Obtient les co�ts de construction

        if (HasEnoughResources(buildingCostData))
        { 
            // Place la construction dans le TileDataMap
            if (tileData != null)
            {
                tileData.Building = construction;
                DeductResources(buildingCostData);

                // Placer la construction sur la Tilemap
                TileManager.Instance.PlaceBuildingTile(cellPosition, tileData.Building);

                // Ajout du b�timent dans le dictionnaire du BuildingManager
                AddBuilding(cellPosition, tileData.Building);

                // Envoi du message (pour la fermeture de l'UI)
                OnBuildingConstructed?.Invoke();
            }
        }
        else
        {
            // Afficher un message d'erreur ou une UI indiquant qu'il n'y a pas assez de ressources
            Debug.Log("Pas assez de ressources pour construire ce b�timent !");
        }
    }

    private void DestroyBuilding(Vector3Int cellPosition)
    {
        // suppression de la construiction sur la tilemap
        TileManager.Instance.RemoveBuilding(cellPosition);

        // suppresion de la construction dans le TileDataMap
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);
        tileData.Building = BuildingType.None;

        // retrait du batiment du dictionnaire
        if (_buildingsDataMap.ContainsKey(cellPosition))
        {
            _buildingsDataMap.Remove(cellPosition);
        }

        // Envoi du message (pour la fermeture de l'ui)
        OnBuildingConstructed?.Invoke();

    }

    public void AddBuilding(Vector3Int position, BuildingType buildingType)
    {
        BuildingData buildingData = new BuildingData(buildingType);
        _buildingsDataMap[position] = buildingData;
    }

    // Obtient les donn�es de co�t pour un b�timent donn�
    private BuildingCostData GetBuildingCostData(BuildingType buildingType)
    {
        // Recherche dans une liste ou base de donn�es des co�ts pour chaque type de b�timent
        return buildingCostsList.Find(cost => cost.buildingType == buildingType);
    }


    // V�rifie si les ressources sont suffisantes
    private bool HasEnoughResources(BuildingCostData buildingCostData)
    {
        foreach (var resourceCost in buildingCostData.resourceCosts)
        {
            if (!ResourceManager.Instance.HasEnoughResources(resourceCost.resourceType, resourceCost.amount))
            {

                // !!!!!!!!!!!!!
                // TODO : impl�menter une message pour indiquer qu'il manque des ressources
                //!!!!!!!!!!!!!!
                return false;  // Pas assez de cette ressource
            }
        }
        return true;  // Toutes les ressources sont suffisantes
    }

    // D�duit les ressources apr�s la construction
    private void DeductResources(BuildingCostData buildingCostData)
    {
        foreach (var resourceCost in buildingCostData.resourceCosts)
        {
            ResourceManager.Instance.DeductResources(resourceCost.resourceType, resourceCost.amount);
        }
    }


    public bool CanBuild(BuildingType type, Vector3Int cellPosition)
    {
        TileData tile = TileManager.Instance.GetTileData(cellPosition);
        if (tile == null || tile.Building != BuildingType.None || !tile.IsClaimed)
            return false;

        // Pr�-calculs de conditions
        bool isGrass = TileManager.Instance.isTargetGroundOnTile(cellPosition, GroundType.Grass);
        bool isMountain = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.Mountain);
        bool noRelief = TileManager.Instance.isTargetReliefOnTile(cellPosition, ReliefType.None);
        bool hasAdjacentRoadOrTown = directions.Any(dir => TileManager.Instance.IsRoadOrTown(cellPosition + dir, true));
        bool hasAdjacentWood = directions.Any(dir => TileManager.Instance.isTargetReliefOnTile(cellPosition + dir, ReliefType.Wood));
        bool hasAdjacentMountain = directions.Any(dir => TileManager.Instance.isTargetReliefOnTile(cellPosition + dir, ReliefType.Mountain));

        // R�gles de construction selon le type
        switch (type)
        {
            case BuildingType.Road:
                return isGrass && noRelief && hasAdjacentRoadOrTown;

            case BuildingType.Lumberjack:
                return isGrass && noRelief && hasAdjacentWood && hasAdjacentRoadOrTown;

            case BuildingType.Temple:
                return isGrass && noRelief;

            case BuildingType.StoneMine:
                return isGrass && isMountain && hasAdjacentRoadOrTown;

            case BuildingType.StonePile:
            case BuildingType.WoodPile:
            case BuildingType.ManaPile:
                return isGrass && noRelief;

            default:
                return false;
        }
    }


}
