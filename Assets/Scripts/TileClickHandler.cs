using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TileClickHandler : MonoBehaviour
{
    public static TileClickHandler Instance { get; private set; }

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

    private void Awake()
    {
        // Assure-toi qu'il n'y a qu'une seule instance de TileClickHandler
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

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

        // Obtenir les options de construction via le ConstructionManager
        List<string> availableConstructions = BuildingManager.Instance.GetAvailableConstructions(_cellPosition);

        // Créer de nouveaux boutons pour chaque construction possible
        foreach (string construction in availableConstructions)
        {
            Button newButton = Instantiate(_constructionButtonPrefab, _constructionPanel.transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = construction;
            newButton.onClick.AddListener(() => BuildingManager.Instance.Build(construction, _cellPosition));
            _constructionButtons.Add(newButton);
        }

        // Afficher ou cacher le panel selon si des constructions sont disponibles
        _constructionPanel.SetActive(availableConstructions.Count > 0);
    }

    public void UpdateTileInfo(TileData tileData)
    {
        // Cette méthode met à jour l'info sur la tuile après une construction
        if (tileData != null)
        {
            _tileInfoPoditionTMP.text = $"Position: {_cellPosition}\n";
            _tileInfoGroundTMP.text = $"Terrain: {tileData.Ground}\n";
            _tileInfoReliefTMP.text = $"Relief: {tileData.Relief}\n";
            _tileInfoBuildingTMP.text = $"Bâtiment: {tileData.Building} (Niveau: {tileData.BuildingLevel})";
        }
    }
}



