using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TileClickHandler : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private GameObject _infoPanel;

    [SerializeField] private GameObject _constructionPanel; // Panel pour afficher les options de construction
    [SerializeField] private Button _constructionButtonPrefab; // Prefab de bouton pour construire


    [SerializeField] private TextMeshProUGUI _tileInfoPoditionTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoGroundTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoReliefTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoBuildingTMP;

    private Vector3Int _cellPosition;
    private TileData _selectedTileData;

    private List<Button> _constructionButtons = new List<Button>(); // Liste pour stocker les boutons

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _cellPosition = _tilemap.WorldToCell(mouseWorldPos);

            _selectedTileData = TileManager.Instance.GetTileData(_cellPosition);

            if (_selectedTileData != null)
            {
                if(_selectedTileData.isClaimed)
                {
                    ShowTileInfo();
                    UpdateConstructionOptions();
                }
                else
                {
                    _infoPanel.SetActive(false);
                }
                
               
            }
        }
    }

    void ShowTileInfo()
    {
        if (_selectedTileData == null) return;

        // Met à jour l'affichage
        _tileInfoPoditionTMP.text = $"Position: {_cellPosition}\n";
        _tileInfoGroundTMP.text = $"Terrain: {_selectedTileData.Ground}\n";
        _tileInfoReliefTMP.text = $"Relief: {_selectedTileData.Relief}\n";
        _tileInfoBuildingTMP.text = $"Batiment: {_selectedTileData.Building} (Niveau: {_selectedTileData.BuildingLevel})";

        //// Active/Désactive les boutons selon le contexte
        //_upgradeButton.gameObject.SetActive(_selectedTileData.Building != BuildingType.None);
        //_buildButton.gameObject.SetActive(_selectedTileData.Building == BuildingType.None);

        _infoPanel.SetActive(true);
    }

    void UpdateConstructionOptions()
    {
        // Supprimer les anciens boutons
        foreach (var button in _constructionButtons)
        {
            Destroy(button.gameObject);
        }
        _constructionButtons.Clear();

        List<string> availableConstructions = GetAvailableConstructions(_selectedTileData);

        // Créer de nouveaux boutons pour chaque construction possible
        foreach (string construction in availableConstructions)
        {
            Button newButton = Instantiate(_constructionButtonPrefab, _constructionPanel.transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = construction;
            newButton.onClick.AddListener(() => Build(construction));
            _constructionButtons.Add(newButton);
        }

        // Afficher ou cacher le panel selon si des constructions sont disponibles
        _constructionPanel.SetActive(availableConstructions.Count > 0);
    }

    List<string> GetAvailableConstructions(TileData tileData)
    {
        List<string> options = new List<string>();

        if (tileData.Building == BuildingType.None)
        {
            if (tileData.Ground == GroundType.Grass) options.Add("road");
            //if (tileData.Ground == "Grass") options.Add("Ferme");
            //if (tileData.Ground == "Hill") options.Add("Mine");
        }

        return options;
    }

    void Build(string construction)
    {
        Debug.Log($"Construction de {construction} sur {_cellPosition}");

        // Appliquer la construction à la tuile
        switch (construction)
        {
            case "road":
                _selectedTileData.Building = BuildingType.Road;
                break;
            //case "Ferme":
            //    _selectedTileData.Building = BuildingType.Farm;
            //    break;
            //case "Mine":
            //    _selectedTileData.Building = BuildingType.Other;
            //    break;
        }

        // Rafraîchir l'affichage après construction
        ShowTileInfo();
        UpdateConstructionOptions();
    }
}



