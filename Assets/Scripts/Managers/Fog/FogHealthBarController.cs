using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogHealthBarController : MonoBehaviour
{
    [SerializeField] private Tilemap fogTilemap;
    [SerializeField] private HealthBarPoolManager poolManager;

    private Dictionary<Vector3Int, GameObject> healthBars = new Dictionary<Vector3Int, GameObject>();

    public void Spawn(Vector3Int cellPosition)
    {
        if (healthBars.ContainsKey(cellPosition)) return;

        Vector3 worldPosition = fogTilemap.GetCellCenterWorld(cellPosition);
        GameObject bar = poolManager.GetHealthBar(worldPosition);
        healthBars[cellPosition] = bar;
    }

    public void UpdateHealthBar(Vector3Int cellPosition, TileData tileData)
    {
        if (!healthBars.ContainsKey(cellPosition)) return;

        HealthBar bar = healthBars[cellPosition].GetComponent<HealthBar>();
        if (bar != null)
        {
            bar.SetHealth(tileData.CurrentFog, tileData.InitialFog);
        }
    }

    public void Destroy(Vector3Int cellPosition)
    {
        if (!healthBars.ContainsKey(cellPosition)) return;

        GameObject bar = healthBars[cellPosition];
        healthBars.Remove(cellPosition);
        poolManager.ReturnHealthBar(bar);
    }

    public bool Has(Vector3Int cellPosition) => healthBars.ContainsKey(cellPosition);
}