using UnityEngine;

public class BuildModeManager : MonoBehaviour
{
    public static BuildModeManager Instance { get; private set; }

    public BuildingType SelectedBuilding { get; private set; } = BuildingType.None;
    public bool IsInBuildMode => SelectedBuilding != BuildingType.None;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void EnterBuildMode(BuildingType buildingType)
    {
        SelectedBuilding = buildingType;
    }

    public void CancelBuildMode()
    {
        SelectedBuilding = BuildingType.None;
    }
}
