using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using NUnit.Framework.Constraints;


public class TileInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private TextMeshProUGUI _tileInfoPositionTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoGroundTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoReliefTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoBuildingTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoBuildingLevelTMP;
    [SerializeField] private TextMeshProUGUI _tileInfoIsConnectedTMP;






    private void Awake()
    {
        TileClickHandler.OnTileSelected += UpdateTileInfo;
    }

    private void OnDestroy()
    {
        TileClickHandler.OnTileSelected -= UpdateTileInfo;
    }

    private void UpdateTileInfo(Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.DataManager.GetTileData(cellPosition);
        if (tileData == null || !tileData.IsClaimed)
        {
            ShowUI(false);
            return;
        }

        BuildingType buildingType = BuildingQueryService.GetBuildingType(cellPosition);
        int? buildingLevel = BuildingQueryService.GetLevel(cellPosition);

        _tileInfoPositionTMP.text = $"Position: {cellPosition}\n";
        _tileInfoGroundTMP.text = $"Terrain: {tileData.Ground}\n";
        _tileInfoReliefTMP.text = $"Relief: {tileData.Relief}\n";
        _tileInfoBuildingTMP.text = $"Bâtiment: {buildingType}";
        _tileInfoBuildingLevelTMP.text = $"Niveau : {buildingLevel}";
        _tileInfoIsConnectedTMP.text = $"Liaison Capital: {tileData.IsConnectedToCapital}";

        ShowUI(true);
    }
        

    private void ShowUI(bool showInfo)
    {
        _infoPanel.SetActive(showInfo);
    }
}
