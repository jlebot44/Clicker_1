using UnityEngine;
using TMPro; // N'oublie pas d'importer TextMesh Pro

public class RessourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI tilesText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI manaGenText;

    private void OnEnable()
    {
        // S'abonne à l'événement
        RessourceManager.OnManaChanged += UpdateManaDisplay; 
        RessourceManager.OnTilesChanged += UpdateTilesDisplay;
        RessourceManager.OnGoldChanged += UpdateGoldDisplay;
        RessourceManager.OnWoodChanged += UpdateWoodDisplay;
        RessourceManager.OnStoneChanged += UpdateStoneDisplay;
        //RessourceManager.OnManaGenChanged += UpdateManaGenDisplay;
    }

    private void OnDisable()
    {
        // Se désabonne quand l'objet est désactivé
        RessourceManager.OnManaChanged -= UpdateManaDisplay;
        RessourceManager.OnTilesChanged -= UpdateTilesDisplay;
        RessourceManager.OnGoldChanged -= UpdateGoldDisplay;
        RessourceManager.OnWoodChanged -= UpdateWoodDisplay;
        RessourceManager.OnStoneChanged -= UpdateStoneDisplay;
        //RessourceManager.OnManaGenChanged -= UpdateManaGenDisplay;
    }

    private void UpdateManaDisplay(int newMana)
    {
        manaText.text = newMana.ToString(); // Met à jour l'affichage du mana
    }

    
    private void UpdateTilesDisplay(int newTiles)
    {
        tilesText.text = newTiles.ToString(); // Met à jour l'affichage du nombre de tuiles révélées
    }

    private void UpdateGoldDisplay(int newGold)
    {
        goldText.text = newGold.ToString(); // Met à jour l'affichage du nombre de tuiles révélées
    }

    private void UpdateWoodDisplay(int newWood)
    {
        woodText.text = newWood.ToString(); // Met à jour l'affichage du nombre de tuiles révélées
    }

    private void UpdateStoneDisplay(int newStone)
    {
        stoneText.text = newStone.ToString(); // Met à jour l'affichage du nombre de tuiles révélées
    }

    //private void UpdateManaGenDisplay(int newManaGen)
    //{
    //    manaGenText.text = newManaGen.ToString(); // Met à jour l'affichage du nombre de tuiles révélées
    //}
}