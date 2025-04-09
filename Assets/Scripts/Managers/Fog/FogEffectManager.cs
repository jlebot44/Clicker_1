using UnityEngine;
using UnityEngine.Tilemaps;

public class FogEffectManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxNewTile;
    [SerializeField] private AudioClip sfxNewTown;
    [SerializeField] private GameObject vfxNewTile;
    [SerializeField] private GameObject captureEffectPrefab;

    public void PlayTileRevealVFX(Vector3Int cellPosition, Tilemap tilemap)
    {
        if (vfxNewTile == null) return;
        Vector3 worldPos = tilemap.GetCellCenterWorld(cellPosition);
        var fx = Instantiate(vfxNewTile, worldPos, Quaternion.identity);
        Destroy(fx, 1.5f);
    }

    public void PlayTileRevealSFX()
    {
        if (audioSource != null && sfxNewTile != null)
            audioSource.PlayOneShot(sfxNewTile);
    }

    public void PlayTownRevealSFX()
    {
        if (audioSource != null && sfxNewTown != null)
            audioSource.PlayOneShot(sfxNewTown);
    }

    public void PlayCaptureEffect(Vector3Int cellPosition, Tilemap tilemap)
    {
        if (captureEffectPrefab == null) return;
        Vector3 worldPos = tilemap.GetCellCenterWorld(cellPosition);
        GameObject effect = Instantiate(captureEffectPrefab, worldPos, Quaternion.identity);
        Destroy(effect, 1.5f);
    }
}