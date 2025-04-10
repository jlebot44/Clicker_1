using UnityEngine;

public class BuildModeManager : MonoBehaviour
{
    public static BuildModeManager Instance { get; private set; }

    public BuildingType SelectedBuilding { get; private set; } = BuildingType.None;
    private bool _isInBuildMode = false;

    public bool IsInBuildMode => _isInBuildMode;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void EnterBuildMode(BuildingType buildingType)
    {
        SelectedBuilding = buildingType;
        _isInBuildMode = true;
    }

    public void CancelBuildMode()
    {
        _isInBuildMode = false;
        SelectedBuilding = BuildingType.None;
    }
}
