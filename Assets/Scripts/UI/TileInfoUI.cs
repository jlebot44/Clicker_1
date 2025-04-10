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

    private void UpdateTileInfo(Vector3Int cellPposition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPposition);
        if (tileData == null || !tileData.IsClaimed)
        {
            ShowUI(false);
            return;
        }
        string buildingLevel = "None";        

        if ( tileData.Building == BuildingType.Town || tileData.Building == BuildingType.Capital)
        {
            BuildingData buildingData = BuildingManager.Instance.GetBuildingData(cellPposition);
            buildingLevel = buildingData.Level.ToString();                
        }


        
        _tileInfoPositionTMP.text = $"Position: {cellPposition}\n";
        _tileInfoGroundTMP.text = $"Terrain: {tileData.Ground}\n";
        _tileInfoReliefTMP.text = $"Relief: {tileData.Relief}\n";
        _tileInfoBuildingTMP.text = $"Bâtiment: {tileData.Building}";
        _tileInfoBuildingLevelTMP.text = $"Niveau : {buildingLevel}";
        _tileInfoIsConnectedTMP.text = $"Liaison Capital: {tileData.IsConnectedToCapital}";


        ShowUI(true);
    }
        

    private void ShowUI(bool showInfo)
    {
        _infoPanel.SetActive(showInfo);
    }
}
