using UnityEngine;
using TMPro;
using System; // N'oublie pas d'importer TextMesh Pro

public class RessourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tilesText;

    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;


    [SerializeField] private TextMeshProUGUI manaPerTurnText;
    [SerializeField] private TextMeshProUGUI goldPerTurnText;
    [SerializeField] private TextMeshProUGUI woodPerTurnText;
    [SerializeField] private TextMeshProUGUI stonePerTurnText;

    [SerializeField] private TextMeshProUGUI manaCapacityText;
    [SerializeField] private TextMeshProUGUI goldCapacityText;
    [SerializeField] private TextMeshProUGUI woodCapacityText;
    [SerializeField] private TextMeshProUGUI stoneCapacityText;


    private void OnEnable()
    {
        // S'abonne � l'�v�nement
        RessourceManager.OnTilesChanged += UpdateTilesDisplay;

        RessourceManager.OnManaChanged += UpdateManaDisplay;
        RessourceManager.OnGoldChanged += UpdateGoldDisplay;
        RessourceManager.OnWoodChanged += UpdateWoodDisplay;
        RessourceManager.OnStoneChanged += UpdateStoneDisplay;

        RessourceManager.OnManaPerTurnChanged += UpdateManaPerTurnDisplay;
        RessourceManager.OnGoldPerTurnChanged += UpdateGoldPerTurnDisplay;
        RessourceManager.OnWoodPerTurnChanged += UpdateWoodPerTurnDisplay;
        RessourceManager.OnStonePerTurnChanged += UpdateStonePerTurnDisplay;

        RessourceManager.OnManaCapacityChanged += UpdateManaCapacityDisplay;
        RessourceManager.OnGoldCapacityChanged += UpdateGoldCapacityDisplay;
        RessourceManager.OnWoodCapacityChanged += UpdateWoodCapacityDisplay;
        RessourceManager.OnStoneCapacityChanged += UpdateStoneCapacityDisplay;
    }

    private void OnDisable()
    {
        // Se d�sabonne quand l'objet est d�sactiv�
        RessourceManager.OnTilesChanged -= UpdateTilesDisplay;

        RessourceManager.OnManaChanged -= UpdateManaDisplay;
        RessourceManager.OnGoldChanged -= UpdateGoldDisplay;
        RessourceManager.OnWoodChanged -= UpdateWoodDisplay;
        RessourceManager.OnStoneChanged -= UpdateStoneDisplay;

        RessourceManager.OnManaPerTurnChanged -= UpdateManaPerTurnDisplay;
        RessourceManager.OnGoldPerTurnChanged -= UpdateGoldPerTurnDisplay;
        RessourceManager.OnWoodPerTurnChanged -= UpdateWoodPerTurnDisplay;
        RessourceManager.OnStonePerTurnChanged -= UpdateStonePerTurnDisplay;

        RessourceManager.OnManaCapacityChanged -= UpdateManaCapacityDisplay;
        RessourceManager.OnGoldCapacityChanged -= UpdateGoldCapacityDisplay;
        RessourceManager.OnWoodCapacityChanged -= UpdateWoodCapacityDisplay;
        RessourceManager.OnStoneCapacityChanged -= UpdateStoneCapacityDisplay;
    }


    private void UpdateTilesDisplay(int newTiles)
    {
        tilesText.text = newTiles.ToString(); // Met � jour l'affichage du nombre de tuiles r�v�l�es
    }


    private void UpdateManaDisplay(int newMana)
    {
        manaText.text = newMana.ToString(); // Met � jour l'affichage du mana
    }

    private void UpdateGoldDisplay(int newGold)
    {
        goldText.text = newGold.ToString(); // Met � jour l'affichage de l'or
    }

    private void UpdateWoodDisplay(int newWood)
    {
        woodText.text = newWood.ToString(); // Met � jour l'affichage du bois
    }

    private void UpdateStoneDisplay(int newStone)
    {
        stoneText.text = newStone.ToString(); // Met � jour l'affichage de la pierre
    }



    private void UpdateManaPerTurnDisplay(int newManaPerTurn)
    {
        manaPerTurnText.text = "+"+newManaPerTurn.ToString(); // Met � jour l'affichage du gain de mana par tour
    }

    private void UpdateGoldPerTurnDisplay(int newGoldPerTurn)
    {
        goldPerTurnText.text = "+" + newGoldPerTurn.ToString(); // Met � jour l'affichage du gain d'or par tour
    }

    private void UpdateWoodPerTurnDisplay(int newWoodPerTurn)
    {
        woodPerTurnText.text = "+" + newWoodPerTurn.ToString(); // Met � jour l'affichage du gain de bois par tour
    }

    private void UpdateStonePerTurnDisplay(int newStonePerTurn)
    {
        stonePerTurnText.text = "+" + newStonePerTurn.ToString(); // Met � jour l'affichage du gain de pierre
    }


    private void UpdateManaCapacityDisplay(int newManaCapacity)
    {
        manaCapacityText.text = " | " + newManaCapacity.ToString(); // Met � jour l'affichage du mana
    }

    private void UpdateGoldCapacityDisplay(int newGoldCapacity)
    {
        goldCapacityText.text = " | " + newGoldCapacity.ToString(); // Met � jour l'affichage de l'or
    }

    private void UpdateWoodCapacityDisplay(int newWoodCapacity)
    {
        woodCapacityText.text = " | " + newWoodCapacity.ToString(); // Met � jour l'affichage du bois
    }

    private void UpdateStoneCapacityDisplay(int newStoneCapacity)
    {
        stoneCapacityText.text = " | " + newStoneCapacity.ToString(); // Met � jour l'affichage de la pierre
    }
}

