using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System;

public class TileClickHandler : MonoBehaviour
{
    public static TileClickHandler Instance { get; private set; }

    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TileClickAnimation _tileClickAnimation;
    [SerializeField] private EvolvePanelController evolvePanelPrefab;

    private EvolvePanelController currentEvolvePanel;



    public static event Action<Vector3Int> OnTileSelected;

    private void Awake()
    {
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
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = _tilemap.WorldToCell(mouseWorldPos);

            TileData selectedTileData = TileManager.Instance.GetTileData(cellPosition);

            if (selectedTileData == null) return;

            _tileClickAnimation.PressTile(cellPosition, _tilemap);

            if (BuildModeManager.Instance.IsInBuildMode)
            {
                TryBuildAt(cellPosition);
            }
            else
            {
                HandleTileSelected(cellPosition); // appelle en interne
                OnTileSelected?.Invoke(cellPosition); // si d'autres scripts écoutent, on garde
            }
        }
    }

    private void TryBuildAt(Vector3Int cellPosition)
    {
        BuildingType selected = BuildModeManager.Instance.SelectedBuilding;
        if (selected == BuildingType.None)
        {
            // Si on est en mode destruction et qu’il y a un bâtiment
            TileData tileData = TileManager.Instance.GetTileData(cellPosition);
            if (tileData != null && tileData.Building != BuildingType.None)
            {
                Debug.Log($"Try destroy at {cellPosition} — building: {tileData.Building}");
                BuildingManager.Instance.Build(BuildingType.None, cellPosition);
            }
            return;
        }

        if (BuildingManager.Instance.CanBuild(selected, cellPosition))
        {
            BuildingManager.Instance.Build(selected, cellPosition);
        }
        else
        {
            Debug.Log("Impossible de construire ici !");
        }
    }

    private void HandleTileSelected(Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);

        // ici on filtre l'evolution sur les ville claimed et reliés à la capitale. A faire evoluer si on permet l'evolution d'autre building
        if (tileData == null
            || !(tileData.Building == BuildingType.Town || tileData.Building == BuildingType.Capital)
            || !tileData.IsClaimed
            || (tileData.Building == BuildingType.Town && !tileData.IsConnectedToCapital))
        {
            HideEvolvePanel();
            return;
        }

        ShowEvolvePanel(cellPosition);
    }

    private void ShowEvolvePanel(Vector3Int cellPos)
    {
        if (currentEvolvePanel != null)
            Destroy(currentEvolvePanel.gameObject);

        currentEvolvePanel = Instantiate(evolvePanelPrefab);

        // 1. Appel avec la vraie position logique
        currentEvolvePanel.Show(cellPos, transform, () =>
        {
            BuildingManager.Instance.UpgradeBuilding(cellPos);
        });

        // 2. Positionnement visuel du panneau légèrement au-dessus
        currentEvolvePanel.transform.position =
            TileManager.Instance.BuildingTilemap.GetCellCenterWorld(cellPos) + Vector3.up * 2.5f;
    }

    private void HideEvolvePanel()
    {
        if (currentEvolvePanel != null)
        {
            Destroy(currentEvolvePanel.gameObject);
            currentEvolvePanel = null;
        }
    }



}