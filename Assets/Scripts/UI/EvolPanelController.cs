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


    private void UpdateButtonText(Vector3Int cellPos)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPos);
        BuildingData buildingData = BuildingManager.Instance.GetBuildingData(cellPos);
        Debug.Log(cellPos);

        if (tileData == null || buildingData == null)
        {
            _evolveButtonText.text = "Évolution indisponible";
            _evolveButton.interactable = false;
            return;
        }

        int nextLevel = buildingData.Level + 1;
        var upgradeData = BuildingManager.Instance.GetUpgradeData(tileData.Building, nextLevel);

        if (upgradeData == null)
        {
            _evolveButtonText.text = "Max niveau atteint";
            _evolveButton.interactable = false;
            return;
        }

        string costText = $"Évoluer au niveau {nextLevel}\n";
        foreach (var cost in upgradeData.upgradeCosts)
        {
            costText += $"{cost.resourceType}: {cost.amount}  \n";
        }

        _evolveButtonText.text = costText.Trim();
        _evolveButton.interactable = true;
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
