using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFogRenderer
{
    private Tilemap _tilemap;

    public TileFogRenderer(Tilemap tilemap)
    {
        _tilemap = tilemap;
    }

    public void RevealFog(Vector3Int position)
    {
        _tilemap.SetTile(position, null);
    }

    public void SetFog(Vector3Int position, TileBase fogTile)
    {
        _tilemap.SetTile(position, fogTile);
    }
}
