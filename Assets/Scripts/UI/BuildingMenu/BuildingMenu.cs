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

    // Stockage des boutons g�n�r�s
    private List<Button> _buildingButtons = new List<Button>();

    // R�f�rences pour gestion directe
    private Button _destroyButton;
    private Button _cancelButton;

    // utilis� pour suivre la selection de construction en cours
    private BuildingType _currentSelectedType = BuildingType.Other;

    // Mapping entre BuildingType et bouton correspondant
    private Dictionary<BuildingType, Button> _buttonMap = new Dictionary<BuildingType, Button>();

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

    private void GenerateConstructionButtons()
    {
        BuildingType[] allBuildings = (BuildingType[])Enum.GetValues(typeof(BuildingType));

        foreach (BuildingType type in allBuildings)
        {
            if (type == BuildingType.None || type == BuildingType.Other || type == BuildingType.Town || type == BuildingType.Capital || type == BuildingType.BonusShrine)
                continue;

            Button button = Instantiate(_buildingButtonPrefab, _content);

            // Trouver les �l�ments enfants
            Transform icon = button.transform.Find("Icon");
            TextMeshProUGUI title = button.transform.Find("Panel/Title")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI cost = button.transform.Find("Panel/Cost")?.GetComponent<TextMeshProUGUI>();

            // Assigner les valeurs
            if (title != null)
                title.text = GetDisplayName(type);

            if (cost != null)
                cost.text = GetCostString(type);

            if (icon != null)
            {
                // Si tu veux g�rer les ic�nes plus tard
                // icon.GetComponent<Image>().sprite = GetIcon(type);
            }


            button.onClick.AddListener(() => OnConstructionSelected(type));
            _buildingButtons.Add(button);
            _buttonMap[type] = button;
        }
    }


    private string GetDisplayName(BuildingType type)
    {
        return type switch
        {
            BuildingType.StoneMine => "Mine de pierre",
            BuildingType.WoodPile => "Tas de bois",
            BuildingType.Temple => "Temple",
            _ => type.ToString()
        };
    }

    private string GetCostString(BuildingType type)
    {
        // � adapter � ton syst�me de co�t r�el
        return type switch
        {
            BuildingType.StoneMine => "10 pierre",
            BuildingType.WoodPile => "5 bois",
            _ => ""
        };
    }

    // G�n�re les boutons sp�ciaux : Annuler et D�truire
    private void GenerateSpecialButtons()
    {
        //// Annuler
        //_cancelButton = Instantiate(_buildingButtonPrefab, _content);
        //_cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Annuler";
        //_cancelButton.onClick.AddListener(OnCancelClicked);
        //_buildingButtons.Add(_cancelButton);

        // D�truire
        _destroyButton = Instantiate(_buildingButtonPrefab, _content);
        _destroyButton.GetComponentInChildren<TextMeshProUGUI>().text = "D�truire";
        _destroyButton.onClick.AddListener(OnDestroyClicked);
        _buildingButtons.Add(_destroyButton);
    }

    private void OnConstructionSelected(BuildingType buildingType)
    {
        // Si d�j� s�lectionn� > annuler
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

        // D�truire (mode sp�cial = BuildingType.None)
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
}
