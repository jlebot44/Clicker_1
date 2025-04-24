using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvolvePanelController : MonoBehaviour
{
    [SerializeField] private Button _evolveButton;
    [SerializeField] private TextMeshProUGUI _evolveButtonText;

    private Vector3Int _currentPosition;
    private System.Action _onEvolveConfirmed;

    public void Show(Vector3Int cellPos, Transform parent, System.Action onEvolveConfirmed)
    {
        _currentPosition = cellPos;
        _onEvolveConfirmed = onEvolveConfirmed;

        transform.SetParent(parent, false);
        transform.position = TileManager.Instance.BuildingTilemap.GetCellCenterWorld(cellPos) + Vector3.up * 0.5f;
        gameObject.SetActive(true);

        UpdateButtonText(cellPos);

        _evolveButton.onClick.RemoveAllListeners();
        _evolveButton.onClick.AddListener(() =>
        {
            _onEvolveConfirmed?.Invoke();
            Hide();
        });
    }


    private void UpdateButtonText(Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.DataManager.GetTileData(cellPosition);
        BuildingData buildingData = BuildingManager.Instance.GetBuildingData(cellPosition);

        if (tileData == null || buildingData == null)
        {
            _evolveButtonText.text = "Évolution indisponible";
            _evolveButton.interactable = false;
            return;
        }

        int nextLevel = buildingData.Level + 1;
        int capitalLevel = BuildingManager.Instance.GetCapitalLevel();

        if (nextLevel > capitalLevel && BuildingQueryService.GetBuildingType(cellPosition) != BuildingType.Capital)
        {
            _evolveButtonText.text = $"Impossible : la capitale doit être au niveau {nextLevel} ou plus";
            _evolveButton.interactable = false;
            return;
        }

        var upgradeData = BuildingManager.Instance.GetUpgradeData(BuildingQueryService.GetBuildingType(cellPosition), nextLevel);

        if (upgradeData == null)
        {
            _evolveButtonText.text = "Max niveau atteint";
            _evolveButton.interactable = false;
            return;
        }

        bool hasEnough = BuildingManager.Instance.HasEnoughResourcesToEvol(upgradeData);

        string costText = $"Évoluer au niveau {nextLevel}\n";
        foreach (var cost in upgradeData.upgradeCosts)
        {
            bool enough = ResourceManager.Instance.HasEnoughResources(cost.resourceType, cost.amount);
            string colorTag = enough ? "white" : "red";
            costText += $"<color={colorTag}>{cost.resourceType}: {cost.amount}</color>\n";
        }

        _evolveButtonText.text = costText.Trim();
        _evolveButton.interactable = hasEnough;
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
