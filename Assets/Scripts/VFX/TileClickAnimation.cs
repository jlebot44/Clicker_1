using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class TileClickAnimation : MonoBehaviour
{
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.2f;

    [SerializeField] private float pressDuration = 0.15f;
    [SerializeField] private float pressIntensity = 0.1f; // ex: 0.1 = scale à 0.9
    [SerializeField] private AnimationCurve pressCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Dictionary<Vector3Int, Coroutine> _runningAnimations = new Dictionary<Vector3Int, Coroutine>();

    public void ShakeTile(Vector3Int cellPosition, Tilemap tilemap)
    {
        StartCoroutine(ShakeTileCoroutine(cellPosition, tilemap));
    }

    public void PressTile(Vector3Int cellPosition, Tilemap tilemap)
    {
        {
            if (_runningAnimations.TryGetValue(cellPosition, out Coroutine existing))
            {
                StopCoroutine(existing);
                tilemap.SetTransformMatrix(cellPosition, Matrix4x4.identity); // Reset au cas où
            }

            Coroutine anim = StartCoroutine(AnimateTilePress(cellPosition, tilemap));
            _runningAnimations[cellPosition] = anim;
        }
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

    private IEnumerator AnimateTilePress(Vector3Int cellPosition, Tilemap tilemap)
    {
        Matrix4x4 originalMatrix = tilemap.GetTransformMatrix(cellPosition);
        Vector3 position = originalMatrix.GetPosition();

        float elapsed = 0f;

        while (elapsed < pressDuration)
        {
            float t = elapsed / pressDuration;
            float curveValue = pressCurve.Evaluate(t); // entre 0 et 1
            float scale = 1f - (pressIntensity * (1f - curveValue)); // scale descend puis remonte

            Matrix4x4 animMatrix = Matrix4x4.TRS(
                position,
                Quaternion.identity,
                new Vector3(scale, scale, 1f)
            );

            tilemap.SetTransformMatrix(cellPosition, animMatrix);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fin = état d’origine
        tilemap.SetTransformMatrix(cellPosition, originalMatrix);

        // Nettoyage de la référence
        _runningAnimations.Remove(cellPosition);
    }

}
