using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Buildings/Building Upgrade Data", fileName = "BuildingUpgradeData")]
public class BuildingUpgradeData : ScriptableObject
{
    public BuildingType buildingType;
    public int level;
    public List<UpgradeResourceCost> upgradeCosts;
}

[System.Serializable]
public class UpgradeResourceCost
{
    public ResourceType resourceType;
    public int amount;
}