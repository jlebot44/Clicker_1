using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class TileClickAnimation : MonoBehaviour
{
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.2f;

    public void ShakeTile(Vector3Int tilePosition, Tilemap tilemap)
    {
        StartCoroutine(ShakeTileCoroutine(tilePosition, tilemap));
    }

    private IEnumerator ShakeTileCoroutine(Vector3Int tilePosition, Tilemap tilemap)
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0f
            );

            tilemap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(randomOffset, Quaternion.identity, Vector3.one));

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Remettre la tuile à sa position d'origine
        tilemap.SetTransformMatrix(tilePosition, Matrix4x4.identity);
    }
}
