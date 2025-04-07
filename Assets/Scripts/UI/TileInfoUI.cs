using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class TileInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private TextMeshProUGUI _tileInfoPositionTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoGroundTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoReliefTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoBuildingTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoIsConnectedTMP;

    [SerializeField] private GameObject _buildingPanel;
    [SerializeField] private Button _buildingButtonPrefab;

    private List<Button> _buildingButtons = new List<Button>();

    private void Awake()
    {
        TileClickHandler.OnTileSelected += UpdateTileInfo;
        BuildingManager.OnBuildingConstructed += CloseUI;
    }

    private void OnDestroy()
    {
        TileClickHandler.OnTileSelected -= UpdateTileInfo;
        BuildingManager.OnBuildingConstructed -= CloseUI;
    }

    private void UpdateTileInfo(Vector3Int cellPposition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPposition);
        if (tileData == null || !tileData.IsClaimed)
        {
            ShowUI(false);
            return;
        }
        
        _tileInfoPositionTMP.text = $"Position: {cellPposition}\n";
        _tileInfoGroundTMP.text = $"Terrain: {tileData.Ground}\n";
        _tileInfoReliefTMP.text = $"Relief: {tileData.Relief}\n";
        _tileInfoBuildingTMP.text = $"Bâtiment: {tileData.Building}";
        _tileInfoIsConnectedTMP.text = $"Liaison Capital: {tileData.IsConnectedToCapital}";


        ShowUI(true);
        UpdateConstructionOptions(cellPposition);
    }

    private void UpdateConstructionOptions(Vector3Int cellPosition)
    {
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
    }

    private void CloseUI()
    {
        ShowUI(false);
    }

    private void ShowUI(bool showInfo)
    {
        _infoPanel.SetActive(showInfo);
        _buildingPanel.SetActive(showInfo);
    }
}
