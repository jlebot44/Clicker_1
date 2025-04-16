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
    [SerializeField] private BuildingInfoPanelController _infoPanel;
    [SerializeField] private RectTransform _content;


    // Stockage des boutons générés
    private List<Button> _buildingButtons = new List<Button>();

    // Références pour gestion directe
    private Button _destroyButton;
    private Button _cancelButton;

    // utilisé pour suivre la selection de construction en cours
    private BuildingType _currentSelectedType = BuildingType.Other;

    // Mapping entre BuildingType et bouton correspondant
    private Dictionary<BuildingType, Button> _buttonMap = new Dictionary<BuildingType, Button>();

    private void OnEnable()
    {
        ResourceManager.OnResourceChanged += UpdateButtonStates;
        UpdateButtonStates(ResourceType.Gold, 0);
    }

    private void OnDisable()
    {
        ResourceManager.OnResourceChanged -= UpdateButtonStates;
    }


    private void Start()
    {
        GenerateConstructionButtons();
        GenerateSpecialButtons();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            OnCancelClicked(); 
            _currentSelectedType = BuildingType.Other;
        }
    }

    private void UpdateButtonStates(ResourceType type, int newValue)
    {
        foreach (var pair in _buttonMap)
        {
            BuildingType buildingType = pair.Key;
            Button button = pair.Value;

            BuildingCostData costData = GetCostData(buildingType);
            if (costData == null) continue;

            TextMeshProUGUI costText = button.transform.Find("Panel/Cost")?.GetComponent<TextMeshProUGUI>();
            if (costText == null) continue;

            bool hasEnough = BuildingResourceService.HasEnoughResources(costData.resourceCosts);

            // Changer la couleur du texte
            costText.color = hasEnough ? Color.white : Color.red;

            // Activer ou désactiver le bouton
            button.interactable = hasEnough;

        }
    }



    private void GenerateConstructionButtons()
    {
        BuildingType[] allBuildings = (BuildingType[])Enum.GetValues(typeof(BuildingType));

        foreach (BuildingType type in allBuildings)
        {
            if (type == BuildingType.None || type == BuildingType.Other || type == BuildingType.Town || type == BuildingType.Capital || type == BuildingType.BonusShrine)
                continue;

            var costData = GetCostData(type);
            if (costData == null)
            {
                Debug.LogWarning($"Pas de données de coût pour le type {type}");
                continue;
            }

            Button button = Instantiate(_buildingButtonPrefab, _content);

            Transform icon = button.transform.Find("Icon");
            TextMeshProUGUI title = button.transform.Find("Panel/Title")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI cost = button.transform.Find("Panel/Cost")?.GetComponent<TextMeshProUGUI>();

            if (title != null)
                title.text = costData.displayName;

            if (cost != null)
            {
                cost.text = GetCostString(costData);

                bool hasEnough = BuildingResourceService.HasEnoughResources(costData.resourceCosts);
                cost.color = hasEnough ? Color.white : Color.red;
            }

            if (icon != null && costData.icon != null)
                icon.GetComponent<Image>().sprite = costData.icon;

            button.onClick.AddListener(() => OnConstructionSelected(type));
            _buildingButtons.Add(button);
            _buttonMap[type] = button;
        }
    }


    private BuildingCostData GetCostData(BuildingType type)
    {
        Debug.Log(type.ToString());
        return BuildingManager.Instance.GetBuildingCostData(type);
    }



    // Génère les boutons spéciaux : Annuler et Détruire
    private void GenerateSpecialButtons()
    {
        // Détruire
        _destroyButton = Instantiate(_buildingButtonPrefab, _content);
        _destroyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Détruire";
        _destroyButton.onClick.AddListener(OnDestroyClicked);
        _buildingButtons.Add(_destroyButton);
    }

    private void OnConstructionSelected(BuildingType buildingType)
    {
        // Si déjà sélectionné > annuler
        if (_currentSelectedType == buildingType)
        {
            OnCancelClicked(); 
            _currentSelectedType = BuildingType.Other;
            return;
        }

        BuildModeManager.Instance.EnterBuildMode(buildingType);
        _currentSelectedType = buildingType;
        HighlightSelectedButton(buildingType);
        _infoPanel.Show(buildingType);
    }

    private void OnDestroyClicked()
    {
        BuildModeManager.Instance.EnterBuildMode(BuildingType.None);
        HighlightSelectedButton(BuildingType.None);
        _infoPanel.Hide();
    }

    private void OnCancelClicked()
    {
        BuildModeManager.Instance.CancelBuildMode();
        ResetAllHighlights();
        _infoPanel.Hide();
    }

    private void HighlightSelectedButton(BuildingType selected)
    {
        ResetAllHighlights();

        // Boutons normaux
        if (_buttonMap.TryGetValue(selected, out Button selectedButton))
        {
            var image = selectedButton.GetComponent<Image>();
            if (image != null) image.color = Color.green;
        }

        // Détruire (mode spécial = BuildingType.None)
        if (selected == BuildingType.None && BuildModeManager.Instance.IsInBuildMode)
        {
            var image = _destroyButton.GetComponent<Image>();
            if (image != null) image.color = Color.green;
        }
    }

    private void ResetAllHighlights()
    {
        foreach (var button in _buildingButtons)
        {
            var img = button.GetComponent<Image>();
            if (img != null) img.color = Color.white;
        }
    }

    public void ShowUI(bool show)
    {
        _buildingPanel.SetActive(show);
    }

    private string GetCostString(BuildingCostData costData)
    {
        List<string> parts = new List<string>();
        foreach (var res in costData.resourceCosts)
        {
            parts.Add($"{res.amount} {res.resourceType}");
        }

        return string.Join("\n", parts);
    }

}
