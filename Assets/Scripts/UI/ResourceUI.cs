using UnityEngine;
using TMPro;
using System; // N'oublie pas d'importer TextMesh Pro

public class ResourceUI : MonoBehaviour
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

    private int _mana = 50;
    private int _gold = 10;
    private int _wood = 0;
    private int _stone = 0;

    private int _manaCapacity = 50;
    private int _goldCapacity = 50;
    private int _woodCapacity = 10;
    private int _stoneCapacity = 10;
    
    




    private void OnEnable()
    {
        // S'abonne à l'événement
        ResourceManager.OnTilesChanged += UpdateTilesDisplay;

        ResourceManager.OnResourceChanged += UpdateResourceDisplay;
        ResourceManager.OnResourcePerTurnChanged += UpdateResourcePertTurnDisplay;
        ResourceManager.OnResourceCapacityChanged += UpdateResourceCapacityDisplay;

    }

    private void OnDisable()
    {
        // Se désabonne quand l'objet est désactivé
        ResourceManager.OnTilesChanged -= UpdateTilesDisplay;

        ResourceManager.OnResourceChanged -= UpdateResourceDisplay;
        ResourceManager.OnResourcePerTurnChanged -= UpdateResourcePertTurnDisplay;
        ResourceManager.OnResourceCapacityChanged -= UpdateResourceCapacityDisplay;

    }


    private void UpdateTilesDisplay(int newTiles)
    {
        _tilesText.text = newTiles.ToString(); // Met à jour l'affichage du nombre de tuiles révélées
    }


    private void UpdateResourceDisplay(ResourceType resourceType, int newResource)
    {
        switch (resourceType)
        {
            case ResourceType.Mana:
                _mana = newResource;
                _manaText.text = _mana.ToString() + " / " + _manaCapacity.ToString(); // Met à jour l'affichage du mana
                break;
            case ResourceType.Gold:
                _gold = newResource;
                _goldText.text = _gold.ToString() + " / " + _goldCapacity.ToString();
                break;
            case ResourceType.Wood:
                _wood = newResource;
                _woodText.text = _wood.ToString() + " / " + _woodCapacity.ToString();
                break;
            case ResourceType.Stone:
                _stone = newResource;
                _stoneText.text = _stone.ToString() + " / " + _stoneCapacity.ToString();
                break;
        }
    }

    private void UpdateResourcePertTurnDisplay(ResourceType resourceType, int newResourcePerTurn)
    {
        switch (resourceType)
        {
            case (ResourceType.Mana):
                _manaPerTurnText.text = "+ " + newResourcePerTurn.ToString();
                break;
            case ResourceType.Gold:
                _goldPerTurnText.text = "+ " + newResourcePerTurn.ToString();
                break;
            case ResourceType.Wood:
                _woodPerTurnText.text = "+ " + newResourcePerTurn.ToString();
                break;
            case ResourceType.Stone:
                _stonePerTurnText.text = "+ " + newResourcePerTurn.ToString();
                break;
        }
    }

    private void UpdateResourceCapacityDisplay(ResourceType resourceType, int newResourceCapacity)
    {
        switch (resourceType)
        {
            case ResourceType.Mana:
                _manaCapacity = newResourceCapacity;
                _manaText.text = _mana.ToString() + " / " + _manaCapacity.ToString();
                break;
            case ResourceType.Gold:
                _goldCapacity = newResourceCapacity;
                _goldText.text = _gold.ToString() + " / " + _goldCapacity.ToString();
                break;
            case ResourceType.Wood:
                _woodCapacity = newResourceCapacity;
                _woodText.text = _wood.ToString() + " / " + _woodCapacity.ToString();
                break;
            case ResourceType.Stone:
                _stoneCapacity = newResourceCapacity;
                _stoneText.text = _stone.ToString() + " / " + _stoneCapacity.ToString();
                break;
        }
    }

}

