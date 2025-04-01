using UnityEngine;
using UnityEngine.Tilemaps;

public class TileClickAnimation : MonoBehaviour
{
    private Vector3 originalScale = Vector3.one;

    [SerializeField] private Tilemap _fogTilemap; // Tilemap du brouillard



    public void AnimateTile(Vector3Int tilePos)
    {
        Vector3 worldPos = _fogTilemap.GetCellCenterWorld(tilePos);

        // Créer un objet temporaire pour l'animation
        GameObject tempTile = new GameObject("TileAnimation");
        tempTile.transform.position = worldPos;
        tempTile.transform.localScale = originalScale;

        // Ajouter un SpriteRenderer pour voir l’animation
        SpriteRenderer sr = tempTile.AddComponent<SpriteRenderer>();
        sr.sprite = _fogTilemap.GetSprite(tilePos);
        sr.sortingOrder = 10; // Pour être au-dessus du Tilemap

        // Modifier l'alpha de la couleur (exemple avec une transparence de 0.5)
        Color initialColor = sr.color;
        initialColor.a = 0.50f;
        sr.color = initialColor;


        // Randomiser l'effet "pop" (agrandissement)
        float randomScale = Random.Range(0.6f, 0.9f); // Randomiser l'échelle (entre 1.1 et 1.5)
        iTween.ScaleTo(tempTile, iTween.Hash(
            "scale", originalScale * randomScale,
            "time", 0.2f,
            "easetype", iTween.EaseType.easeOutQuad,
            "oncomplete", "StartShaking",
            "oncompletetarget", gameObject,
            "oncompleteparams", tempTile
        ));

        // Randomiser l'angle de rotation
        float randomRotation = Random.Range(-15f, 15f); // Randomiser la rotation (entre -15° et 15°)
        iTween.RotateTo(tempTile, iTween.Hash(
            "z", randomRotation,
            "time", 0.2f,
            "easetype", iTween.EaseType.easeOutSine
        ));

        // Détruire l'objet temporaire après animation
        Destroy(tempTile, 0.5f);


    }

    // Cette méthode démarre le tremblement après l'agrandissement
    void StartShaking(GameObject tempTile)
    {
        // Randomiser l'amplitude du tremblement
        float shakeX = Random.Range(0.05f, 0.2f);  // Amplitude du tremblement en X (aléatoire)
        float shakeY = Random.Range(0.05f, 0.2f);  // Amplitude du tremblement en Y (aléatoire)

        // Ajouter un effet de tremblement
        iTween.ShakePosition(tempTile, iTween.Hash(
            "x", shakeX,  // Amplitude du tremblement en X
            "y", shakeY,  // Amplitude du tremblement en Y
            "time", 0.2f,  // Durée du tremblement
            "easetype", iTween.EaseType.easeOutQuad
        ));

        // Après le tremblement, revenir à la taille normale
        ShrinkBack(tempTile);
    }

    // Retour à la taille normale et rotation
    void ShrinkBack(GameObject tileObject)
    {
        // Retour à la taille normale
        iTween.ScaleTo(tileObject, iTween.Hash(
            "scale", originalScale,
            "time", 0.2f,
            "easetype", iTween.EaseType.easeInQuad
        ));

        // Retour à la rotation normale
        iTween.RotateTo(tileObject, iTween.Hash(
            "z", 0f,
            "time", 0.2f,
            "easetype", iTween.EaseType.easeInSine
        ));
    }
}
