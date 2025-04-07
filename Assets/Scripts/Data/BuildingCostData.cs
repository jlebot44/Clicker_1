using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuildingCost", menuName = "Buildings/Building Cost Data")]
public class BuildingCostData : ScriptableObject
{
    public BuildingType buildingType;
    public List<ResourceCost> resourceCosts; // Liste des ressources et de leur coût
}

[System.Serializable]
public class ResourceCost
{
    public string resourceName; // Nom de la ressource (ex: "Gold", "Wood", etc.)
    public int amount;          // Quantité nécessaire
}
