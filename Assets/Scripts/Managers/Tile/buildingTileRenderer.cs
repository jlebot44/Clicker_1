using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class BuildingTileRenderer
{
    public void PlaceBuilding(Vector3Int position, BuildingType type)
    {
        // Si c’est une route, on délègue au système de route
        if (type == BuildingType.Road)
        {
            TileManager.Instance.RoadRenderer.PlaceRoad(position);
            return;
        }

        // Récupère la tuile correspondante via le dictionnaire exposé
        if (!TileManager.Instance.BuildingTiles.TryGetValue(type, out TileBase tileToPlace) || tileToPlace == null)
        {
            Debug.LogWarning($"Tile for building type {type} not found.");
            return;
        }

        // Lance l’animation via le TileManager
        TileManager.Instance.StartCoroutine(AnimatePlacement(position, tileToPlace));
    }

    public void RemoveBuilding(Vector3Int position)
    {
        TileManager.Instance.BuildingTilemap.SetTile(position, null);
    }

    private IEnumerator AnimatePlacement(Vector3Int pos, TileBase tile)
    {
        var tm = TileManager.Instance;
        Vector3 worldPos = tm.BuildingTilemap.GetCellCenterWorld(pos);

        GameObject ghost = new("GhostTile");
        ghost.transform.position = worldPos + Vector3.up * 0.12f;

        var sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = (tile as Tile)?.sprite;
        sr.sortingOrder = 10;

        float liftHeight = 0.03f, liftDur = 0.04f, pauseDur = 0.15f;
        float fallDur = 0.02f, bounceHeight = 0.01f, bounceDur = 0.03f;
        Vector3 top = worldPos + Vector3.up * (0.12f + liftHeight);

        float t = 0;
        while (t < liftDur)
        {
            ghost.transform.position = Vector3.Lerp(worldPos + Vector3.up * 0.12f, top, Mathf.Sin(t / liftDur * Mathf.PI * 0.5f));
            t += Time.deltaTime;
            yield return null;
        }

        ghost.transform.position = top;
        yield return new WaitForSeconds(pauseDur);

        t = 0;
        while (t < fallDur)
        {
            ghost.transform.position = Vector3.Lerp(top, worldPos, 1 - Mathf.Cos(t / fallDur * Mathf.PI * 0.5f));
            t += Time.deltaTime;
            yield return null;
        }

        ghost.transform.position = worldPos;

        if (tm.DustEffectPrefab != null)
        {
            GameObject dust = Object.Instantiate(tm.DustEffectPrefab, worldPos, Quaternion.identity);
            Object.Destroy(dust, 2f);
        }

        t = 0;
        while (t < bounceDur)
        {
            ghost.transform.position = worldPos + Vector3.up * Mathf.Sin(t / bounceDur * Mathf.PI) * bounceHeight;
            t += Time.deltaTime;
            yield return null;
        }

        tm.BuildingTilemap.SetTile(pos, tile);
        Object.Destroy(ghost);
    }
}
