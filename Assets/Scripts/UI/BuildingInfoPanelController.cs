using TMPro;
using UnityEngine;

public class BuildingInfoPanelController : MonoBehaviour
{
    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private TextMeshProUGUI _buildingNameText;
    [SerializeField] private TextMeshProUGUI _buildingCostText;
    [SerializeField] private TextMeshProUGUI _buildingDescText;
    [SerializeField] private TextMeshProUGUI _buildingPrereqText;

    public void Show(BuildingType type)
    {
        var costData = BuildingManager.Instance.GetBuildingCostData(type);
        _buildingNameText.text = type.ToString();

        if (costData != null)
        {
            _buildingCostText.text = string.Join("\n", costData.resourceCosts.ConvertAll(c => $"{c.amount} {c.resourceType}"));
        }
        else
        {
            _buildingCostText.text = "Coût inconnu";
        }

        _panelRoot.SetActive(true);
    }

    public void Hide()
    {
        _panelRoot.SetActive(false);
    }
}
