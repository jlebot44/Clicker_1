using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TownUpgradePanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _levelTMP;
    [SerializeField] private TMP_Text _costTMP;
    [SerializeField] private Button _evolveButton;

    private Vector3Int _currentPosition;

    public void Show(Vector3Int cellPos)
    {
        _currentPosition = cellPos;
        UpdateUI();
        gameObject.SetActive(true);

        _evolveButton.onClick.RemoveAllListeners();
        _evolveButton.onClick.AddListener(() =>
        {
            BuildingUpgradeService.TryUpgrade(_currentPosition);
            Hide();
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        var tileData = TileManager.Instance.DataManager.GetTileData(_currentPosition);
        var buildingData = BuildingManager.Instance.GetBuildingData(_currentPosition);

        if (tileData == null || buildingData == null)
        {
            _levelTMP.text = "Évolution indisponible";
            _costTMP.text = "-";
            _evolveButton.interactable = false;
            return;
        }

        int nextLevel = buildingData.Level + 1;
        int capitalLevel = BuildingManager.Instance.GetCapitalLevel();

        if (nextLevel > capitalLevel && buildingData.Type != BuildingType.Capital)
        {
            _levelTMP.text = $"Capitale requise niveau {nextLevel}";
            _costTMP.text = "-";
            _evolveButton.interactable = false;
            return;
        }

        var upgradeData = BuildingManager.Instance.GetUpgradeData(buildingData.Type, nextLevel);
        if (upgradeData == null)
        {
            _levelTMP.text = "Max niveau atteint";
            _costTMP.text = "-";
            _evolveButton.interactable = false;
            return;
        }

        _levelTMP.text = $"Évolution vers niveau {nextLevel}";
        _costTMP.text = GetCostText(upgradeData);
        _evolveButton.interactable = BuildingManager.Instance.HasEnoughResourcesToEvol(upgradeData);
    }

    private string GetCostText(BuildingUpgradeData upgradeData)
    {
        string text = "";
        foreach (var cost in upgradeData.upgradeCosts)
        {
            bool enough = ResourceManager.Instance.HasEnoughResources(cost.resourceType, cost.amount);
            string color = enough ? "white" : "red";
            text += $"<color={color}>{cost.resourceType}: {cost.amount}</color>\n";
        }
        return text.TrimEnd();
    }
}
