using System.Collections.Generic;
using UnityEngine;

public static class BuildingResourceService
{
    public static bool HasEnoughResources(List<ResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            if (!ResourceManager.Instance.HasEnoughResources(cost.resourceType, cost.amount))
            {
                
                return false;
            }
        }
        return true;
    }

    public static bool HasEnoughResources(List<UpgradeResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            if (!ResourceManager.Instance.HasEnoughResources(cost.resourceType, cost.amount))
            {
                
                return false;
            }
        }
        return true;
    }

    public static void DeductResources(List<ResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            ResourceManager.Instance.DeductResources(cost.resourceType, cost.amount);
        }
    }

    public static void DeductResources(List<UpgradeResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            ResourceManager.Instance.DeductResources(cost.resourceType, cost.amount);
        }
    }
}
