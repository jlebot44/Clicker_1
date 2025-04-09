using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    [SerializeField] private GameObject _buildingPanel;
    [SerializeField] private Button _buildingButtonPrefab;


    private List<Button> _buildingButtons = new List<Button>();


    private void Awake()
    {
        TileClickHandler.OnTileSelected += UpdateConstructionOptions;
        //BuildingManager.OnBuildingConstructed += CloseUI;
    }

    private void OnDestroy()
    {
        TileClickHandler.OnTileSelected -= UpdateConstructionOptions;
        //BuildingManager.OnBuildingConstructed -= CloseUI;
    }


    private void UpdateConstructionOptions(Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);
        if (tileData == null || !tileData.IsClaimed)
        {
            ShowUI(false);
            return;
        }

        // Désactiver les anciens boutons au lieu de les détruire
        foreach (var button in _buildingButtons)
        {
            button.gameObject.SetActive(false);
        }

        List<string> availableConstructions = BuildingManager.Instance.GetAvailableConstructions(cellPosition);

        for (int i = 0; i < availableConstructions.Count; i++)
        {
            Button button;
            if (i < _buildingButtons.Count)
            {
                button = _buildingButtons[i]; // Réutiliser un bouton existant
                button.gameObject.SetActive(true);
            }
            else
            {
                button = Instantiate(_buildingButtonPrefab, _buildingPanel.transform);
                _buildingButtons.Add(button);
            }

            string constructionName = availableConstructions[i]; // Copie locale pour éviter le problème de capture de variable
            button.GetComponentInChildren<TextMeshProUGUI>().text = constructionName;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnConstructionSelected(constructionName, cellPosition));
        }

        _buildingPanel.SetActive(availableConstructions.Count > 0);
    }

    private void OnConstructionSelected(string construction, Vector3Int position)
    {
        // Convertir la chaîne en BuildingType
        if (Enum.TryParse(construction, out BuildingType buildingType))
        {
            BuildingManager.Instance.Build(buildingType, position);
        }
        else
        {
            Debug.LogWarning($"Construction inconnue: {construction}");
        }
        _buildingPanel.SetActive(false);

    }



    private void ShowUI(bool showInfo)
    {
        _buildingPanel.SetActive(showInfo);
    }


}
