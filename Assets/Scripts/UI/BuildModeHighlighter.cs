using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildModeHighlighter : MonoBehaviour
{
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase highlightTile;
    [SerializeField] private int highlightRadius = 3;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!BuildModeManager.Instance.IsInBuildMode)
        {
            highlightTilemap.ClearAllTiles();
            return;
        }

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int centerCell = TileManager.Instance.GroundTilemap.WorldToCell(mouseWorldPos);

        highlightTilemap.ClearAllTiles();

        for (int x = -highlightRadius; x <= highlightRadius; x++)
        {
            for (int y = -highlightRadius; y <= highlightRadius; y++)
            {
                Vector3Int offset = new Vector3Int(x, y, 0);
                Vector3Int testCell = centerCell + offset;

                BuildingType selected = BuildModeManager.Instance.SelectedBuilding;

                if (BuildingManager.Instance.CanBuild(selected, testCell))
                {
                    highlightTilemap.SetTile(testCell, highlightTile);
                }
            }
        }
    }
}
