using System.Collections;
using UnityEngine;

public class CameraEffectManager : MonoBehaviour
{
    public IEnumerator Shake(Camera cam, float duration = 0.2f, float magnitude = 0.1f)
    {
        Vector3 originalPos = cam.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cam.transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = originalPos;
    }
}
