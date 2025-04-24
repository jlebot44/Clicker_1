using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildPreview : MonoBehaviour
{
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private TileBase validPreviewTile;
    [SerializeField] private TileBase invalidPreviewTile;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {

        if (!BuildModeManager.Instance.IsInBuildMode)
        {
            previewTilemap.ClearAllTiles();
            return;
        }

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = TileManager.Instance.GroundTilemap.WorldToCell(mouseWorld);
        previewTilemap.ClearAllTiles();
        BuildingType selected = BuildModeManager.Instance.SelectedBuilding;
        bool canBuild = BuildingValidatorService.CanBuild(selected, cellPos);
        TileBase tile = canBuild ? validPreviewTile : invalidPreviewTile;
        previewTilemap.SetTile(cellPos, tile);
    }
}
