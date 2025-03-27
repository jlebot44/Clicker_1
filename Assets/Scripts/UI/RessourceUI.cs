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
        // S'abonne � l'�v�nement
        RessourceManager.OnManaChanged += UpdateManaDisplay; 
        RessourceManager.OnTilesChanged += UpdateTilesDisplay;
        RessourceManager.OnGoldChanged += UpdateGoldDisplay;
        RessourceManager.OnWoodChanged += UpdateWoodDisplay;
        RessourceManager.OnStoneChanged += UpdateStoneDisplay;
        //RessourceManager.OnManaGenChanged += UpdateManaGenDisplay;
    }

    private void OnDisable()
    {
        // Se d�sabonne quand l'objet est d�sactiv�
        RessourceManager.OnManaChanged -= UpdateManaDisplay;
        RessourceManager.OnTilesChanged -= UpdateTilesDisplay;
        RessourceManager.OnGoldChanged -= UpdateGoldDisplay;
        RessourceManager.OnWoodChanged -= UpdateWoodDisplay;
        RessourceManager.OnStoneChanged -= UpdateStoneDisplay;
        //RessourceManager.OnManaGenChanged -= UpdateManaGenDisplay;
    }

    private void UpdateManaDisplay(int newMana)
    {
        manaText.text = newMana.ToString(); // Met � jour l'affichage du mana
    }

    
    private void UpdateTilesDisplay(int newTiles)
    {
        tilesText.text = newTiles.ToString(); // Met � jour l'affichage du nombre de tuiles r�v�l�es
    }

    private void UpdateGoldDisplay(int newGold)
    {
        goldText.text = newGold.ToString(); // Met � jour l'affichage du nombre de tuiles r�v�l�es
    }

    private void UpdateWoodDisplay(int newWood)
    {
        woodText.text = newWood.ToString(); // Met � jour l'affichage du nombre de tuiles r�v�l�es
    }

    private void UpdateStoneDisplay(int newStone)
    {
        stoneText.text = newStone.ToString(); // Met � jour l'affichage du nombre de tuiles r�v�l�es
    }

    //private void UpdateManaGenDisplay(int newManaGen)
    //{
    //    manaGenText.text = newManaGen.ToString(); // Met � jour l'affichage du nombre de tuiles r�v�l�es
    //}
}