using UnityEngine;
using TMPro; // N'oublie pas d'importer TextMesh Pro

public class RessourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI tilesText;

    private void OnEnable()
    {
        // S'abonne à l'événement
        RessourceManager.OnManaChanged += UpdateManaDisplay; 
        RessourceManager.OnTilesChanged += UpdateTilesDisplay;
    }

    private void OnDisable()
    {
        // Se désabonne quand l'objet est désactivé
        RessourceManager.OnManaChanged -= UpdateManaDisplay; 
        RessourceManager.OnTilesChanged -= UpdateTilesDisplay;
    }

    private void UpdateManaDisplay(int newMana)
    {
        manaText.text = newMana.ToString(); // Met à jour l'affichage du mana
    }

    
    private void UpdateTilesDisplay(int newTiles)
    {
        tilesText.text = newTiles.ToString(); // Met à jour l'affichage du nombre de tuiles révélées
    }

}