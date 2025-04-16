using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuildingCost", menuName = "Buildings/Building Cost Data")]
public class BuildingCostData : ScriptableObject
{
    public BuildingType buildingType;
    public Sprite icon;
    public string displayName;
    public List<ResourceCost> resourceCosts; // Liste des ressources et de leur co�t
}

[System.Serializable]
public class ResourceCost
{
    public ResourceType resourceType; // Nom de la ressource (ex: "Gold", "Wood", etc.)
    public int amount;          // Quantit� n�cessaire
}
