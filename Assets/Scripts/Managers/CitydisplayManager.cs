using System.Collections.Generic;
using UnityEngine;

public class CityLevelDisplayManager : MonoBehaviour
{

    [SerializeField] private GameObject _levelDisplayPrefab;
    private Dictionary<Vector3Int, CityLevelMarker> _markers = new();

    private void Start()
    {
        BuildingManager.OnBuildingConstructed += RefreshMarkers;
        RefreshMarkers();
    }

    private void OnDestroy()
    {
        BuildingManager.OnBuildingConstructed -= RefreshMarkers;
    }

    private void RefreshMarkers()
    {
        foreach (var pair in BuildingManager.Instance.BuildingsDataMap)
        {
            var pos = pair.Key;
            var data = pair.Value;

            if (data.Type == BuildingType.Town || data.Type == BuildingType.Capital)
            {
                if (!_markers.ContainsKey(pos))
                {
                    var go = Instantiate(_levelDisplayPrefab, TileManager.Instance.BuildingTilemap.GetCellCenterWorld(pos), Quaternion.identity, transform);
                    var marker = go.GetComponent<CityLevelMarker>();
                    _markers[pos] = marker;
                }

                _markers[pos].SetLevel(data.Level);
            }
        }
    }

    public void ForceRefresh()
    {
        RefreshMarkers();
    }
}
