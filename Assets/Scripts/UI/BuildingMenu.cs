using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _buildingPanel;
    [SerializeField] private Button _buildingButtonPrefab;

    private List<Button> _buildingButtons = new List<Button>();

    private Button _activeButton;

    private Dictionary<BuildingType, Button> _buttonMap = new Dictionary<BuildingType, Button>();

    private void Start()
    {
        GenerateButtons();
    }

    // Génère une fois tous les boutons de bâtiment au démarrage
    private void GenerateButtons()
    {
        BuildingType[] allBuildings = (BuildingType[])Enum.GetValues(typeof(BuildingType));

        foreach (BuildingType type in allBuildings)
        {
            if (type == BuildingType.None || type == BuildingType.Other)
                continue; // On ignore les types invalides

            Button button = Instantiate(_buildingButtonPrefab, _buildingPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = type.ToString();

            button.onClick.AddListener(() => OnConstructionSelected(type));
            _buildingButtons.Add(button);
            _buttonMap[type] = button;
        }

        // Bouton Annuler
        Button cancelButton = Instantiate(_buildingButtonPrefab, _buildingPanel.transform);
        cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Annuler";
        cancelButton.onClick.AddListener(OnCancelClicked);
        _buildingButtons.Add(cancelButton);
    }

    private void OnConstructionSelected(BuildingType buildingType)
    {
        BuildModeManager.Instance.EnterBuildMode(buildingType);
        HighlightSelectedButton(buildingType);
    }

    private void UpdateButtonHighlight(string selectedName)
    {
        foreach (var button in _buildingButtons)
        {
            bool isActive = button.GetComponentInChildren<TextMeshProUGUI>().text == selectedName;

            ColorBlock colors = button.colors;
            colors.normalColor = isActive ? Color.green : Color.white;  // Surbrillance verte
            button.colors = colors;

            // Tu peux aussi modifier le texte, la taille ou ajouter une icône ici
        }
    }

    private void HighlightSelectedButton(BuildingType selected)
    {
        foreach (var pair in _buttonMap)
        {
            Button button = pair.Value;
            Image image = button.GetComponent<Image>();

            if (image != null)
            {
                image.color = pair.Key == selected ? Color.green : Color.white;
            }
        }
    }

    private void OnCancelClicked()
    {
        BuildModeManager.Instance.CancelBuildMode();
        HighlightSelectedButton(BuildingType.None); // désélectionne tout
    }

    public void ShowUI(bool show)
    {
        _buildingPanel.SetActive(show);
    }
}
