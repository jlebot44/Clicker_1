using UnityEngine;
using TMPro;
using System; // N'oublie pas d'importer TextMesh Pro

public class RessourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tilesText;

    [SerializeField] private TextMeshProUGUI _manaText;
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _woodText;
    [SerializeField] private TextMeshProUGUI _stoneText;


    [SerializeField] private TextMeshProUGUI _manaPerTurnText;
    [SerializeField] private TextMeshProUGUI _goldPerTurnText;
    [SerializeField] private TextMeshProUGUI _woodPerTurnText;
    [SerializeField] private TextMeshProUGUI _stonePerTurnText;

    //[SerializeField] private TextMeshProUGUI _manaCapacityText;
    //[SerializeField] private TextMeshProUGUI _goldCapacityText;
    //[SerializeField] private TextMeshProUGUI _woodCapacityText;
    //[SerializeField] private TextMeshProUGUI _stoneCapacityText;

    private int _mana;
    private int _gold;
    private int _wood;
    private int _stone;

    private int _manaCapacity;
    private int _goldCapacity;
    private int _woodCapacity;
    private int _stoneCapacity;
    
    




    private void OnEnable()
    {
        // S'abonne à l'événement
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
        // Se désabonne quand l'objet est désactivé
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
        _tilesText.text = newTiles.ToString(); // Met à jour l'affichage du nombre de tuiles révélées
    }


    private void UpdateManaDisplay(int newMana)
    {
        _mana = newMana;
        _manaText.text = _mana.ToString() + " / " + _manaCapacity.ToString(); // Met à jour l'affichage du mana
    }

    private void UpdateGoldDisplay(int newGold)
    {
        _gold = newGold;
        _goldText.text = _gold.ToString() + " / " + _manaCapacity.ToString(); // Met à jour l'affichage de l'or
    }

    private void UpdateWoodDisplay(int newWood)
    {
        _wood = newWood;
        _woodText.text = _wood.ToString() + " / " + _woodCapacity.ToString(); // Met à jour l'affichage du bois
    }

    private void UpdateStoneDisplay(int newStone)
    {
        _stone = newStone;
        _stoneText.text = _stone.ToString() + " / " + _stoneCapacity.ToString(); // Met à jour l'affichage de la pierre
    }



    private void UpdateManaPerTurnDisplay(int newManaPerTurn)
    {
        _manaPerTurnText.text = newManaPerTurn.ToString(); // Met à jour l'affichage du gain de mana par tour
    }

    private void UpdateGoldPerTurnDisplay(int newGoldPerTurn)
    {

        _goldPerTurnText.text = newGoldPerTurn.ToString(); // Met à jour l'affichage du gain d'or par tour
    }

    private void UpdateWoodPerTurnDisplay(int newWoodPerTurn)
    {
        _woodPerTurnText.text = newWoodPerTurn.ToString(); // Met à jour l'affichage du gain de bois par tour
    }

    private void UpdateStonePerTurnDisplay(int newStonePerTurn)
    {
        _stonePerTurnText.text = newStonePerTurn.ToString(); // Met à jour l'affichage du gain de pierre
    }


    private void UpdateManaCapacityDisplay(int newManaCapacity)
    {
        _manaCapacity = newManaCapacity;
        _manaText.text = _mana.ToString() + " / " + _manaCapacity.ToString(); // Met à jour l'affichage du mana
    }

    private void UpdateGoldCapacityDisplay(int newGoldCapacity)
    {
        _goldCapacity = newGoldCapacity;
        _goldText.text = _gold.ToString() + " / " + _goldCapacity.ToString(); // Met à jour l'affichage de l'or
    }

    private void UpdateWoodCapacityDisplay(int newWoodCapacity)
    {
        _woodCapacity = newWoodCapacity;
        _woodText.text = _wood.ToString() + " / " + _woodCapacity.ToString(); // Met à jour l'affichage du bois
    }

    private void UpdateStoneCapacityDisplay(int newStoneCapacity)
    {
        _stoneCapacity = newStoneCapacity;
        _stoneText.text = _stone.ToString() + " / " + _stoneCapacity.ToString(); // Met à jour l'affichage de la pierre
    }
}

