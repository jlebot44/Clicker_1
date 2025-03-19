using UnityEngine;
using TMPro; // N'oublie pas d'importer TextMesh Pro

public class RessourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI tilesText;

    private void OnEnable()
    {
        // S'abonne � l'�v�nement
        RessourceManager.OnManaChanged += UpdateManaDisplay; 
        RessourceManager.OnTilesChanged += UpdateTilesDisplay;
    }

    private void OnDisable()
    {
        // Se d�sabonne quand l'objet est d�sactiv�
        RessourceManager.OnManaChanged -= UpdateManaDisplay; 
        RessourceManager.OnTilesChanged -= UpdateTilesDisplay;
    }

    private void UpdateManaDisplay(int newMana)
    {
        manaText.text = newMana.ToString(); // Met � jour l'affichage du mana
    }

    
    private void UpdateTilesDisplay(int newTiles)
    {
        tilesText.text = newTiles.ToString(); // Met � jour l'affichage du nombre de tuiles r�v�l�es
    }

}