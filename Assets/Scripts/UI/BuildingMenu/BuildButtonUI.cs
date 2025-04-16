using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildButtonUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI costText;

    private BuildingType buildingType;

    // Appelé par BuildingMenu pour configurer le bouton
    public void Setup(BuildingType type, Sprite icon, int cost)
    {
        buildingType = type;

        if (iconImage != null) iconImage.sprite = icon;
        if (titleText != null) titleText.text = type.ToString();
        if (costText != null) costText.text = $"{cost} Gold";
    }

    // Hooké dans l’inspecteur (ou via code)
    public void OnClick()
    {
        BuildModeManager.Instance.EnterBuildMode(buildingType);
    }
}
