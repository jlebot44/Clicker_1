using System;
using System.Collections;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }  // patern singleton

    // Événements déclenchés lorsque les ressources changent
    public static event Action<ResourceType, int> OnResourceChanged; // Événement générique pour changer n'importe quelle ressource
    public static event Action<ResourceType, int> OnResourceCapacityChanged; // Événement générique pour changer les capacités
    public static event Action<ResourceType, int> OnResourcePerTurnChanged;


    public static event Action<int> OnTilesChanged;

    // Événements pour la barre de progression
    public static event Action<float> OnProgressReset;


    [SerializeField] private int _mana = 50;
    [SerializeField] private int _gold = 10;
    [SerializeField] private int _wood = 0;
    [SerializeField] private int _stone = 0;

    private int _manaPerTurn;
    private int _goldPerTurn;
    private int _woodPerTurn;
    private int _stonePerTurn;

    private int _manaCapacity = 50;
    private int _goldCapacity = 50;
    private int _woodCapacity = 10;
    private int _stoneCapacity = 10;

    [SerializeField] private int _capacityPerManaPile = 10;
    [SerializeField] private int _capacityPerWoodPile = 10;
    [SerializeField] private int _capacityPerStonePile = 10;
    [SerializeField] private int _capacityPerGoldPile = 10;

    [SerializeField] private float _updateInterval = 5f;

    private int _tiles = 1;

    // Accesseurs pour les ressources
    public int GetResource(ResourceType type)
    {
        return type switch
        {
            ResourceType.Mana => _mana,
            ResourceType.Gold => _gold,
            ResourceType.Wood => _wood,
            ResourceType.Stone => _stone,
            _ => 0
        };
    }

    public void SetResource(ResourceType type, int value)
    {
        switch (type)
        {
            case ResourceType.Mana:
                value = Mathf.Clamp(value, 0, _manaCapacity);
                SetResourceValue(ref _mana, value, ResourceType.Mana);
                break;

            case ResourceType.Gold:
                value = Mathf.Clamp(value, 0, _goldCapacity);
                SetResourceValue(ref _gold, value, ResourceType.Gold);
                break;

            case ResourceType.Wood:
                value = Mathf.Clamp(value, 0, _woodCapacity);
                SetResourceValue(ref _wood, value, ResourceType.Wood);
                break;

            case ResourceType.Stone:
                value = Mathf.Clamp(value, 0, _stoneCapacity);
                SetResourceValue(ref _stone, value, ResourceType.Stone);
                break;
        }
    }

    private void SetResourceValue(ref int resourceField, int value, ResourceType type)
    {
        if (resourceField == value)
            return;

        resourceField = value;
        OnResourceChanged?.Invoke(type, value);
    }

    public float UpdateInterval { get => _updateInterval; set => _updateInterval = Mathf.Max(0.1f, value); }
    public int Tiles
    {
        get => _tiles;
        set
        {
            _tiles = Mathf.Max(0, value);
            OnTilesChanged?.Invoke(_tiles);
        }
    }

    // Capacités des ressources
    public int GetCapacity(ResourceType type)
    {
        return type switch
        {
            ResourceType.Mana => _manaCapacity,
            ResourceType.Gold => _goldCapacity,
            ResourceType.Wood => _woodCapacity,
            ResourceType.Stone => _stoneCapacity,
            _ => 0
        };
    }

    public void SetCapacity(ResourceType type, int value)
    {
        value = Mathf.Max(0, value);
        switch (type)
        {
            case ResourceType.Mana:
                _manaCapacity = value;
                OnResourceCapacityChanged?.Invoke(ResourceType.Mana, _manaCapacity);
                break;
            case ResourceType.Gold:
                _goldCapacity = value;
                OnResourceCapacityChanged?.Invoke(ResourceType.Gold, _goldCapacity);
                break;
            case ResourceType.Wood:
                _woodCapacity = value;
                OnResourceCapacityChanged?.Invoke(ResourceType.Wood, _woodCapacity);
                break;
            case ResourceType.Stone:
                _stoneCapacity = value;
                OnResourceCapacityChanged?.Invoke(ResourceType.Stone, _stoneCapacity);
                break;
        }
    }

    public void OnEnable()
    {
        BuildingManager.OnBuildingConstructed += CalculResources;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("RessourceManager déjà existant ! Suppression de l'instance en double.");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        CalculRessourcesPerTurn();
        StartCoroutine(UpdateResources());
    }

    // boucle principale du jeu
    private IEnumerator UpdateResources()
    {
        while (true)
        {
            yield return new WaitForSeconds(UpdateInterval);

            if (Time.timeScale > 0)
            {
                CalculRessourcesPerTurn();
                CalculCapacity();
                GainResources();
                OnProgressReset?.Invoke(_updateInterval);
            }
        }
    }

    public void CalculResources()
    {
        CalculRessourcesPerTurn();
        CalculCapacity();
    }

    // Fonction récurrente pour gagner des ressources par cycle
    void GainResources()
    {
        SetResource(ResourceType.Mana, _mana + _manaPerTurn);
        SetResource(ResourceType.Gold, _gold + _goldPerTurn);
        SetResource(ResourceType.Wood, _wood + _woodPerTurn);
        SetResource(ResourceType.Stone, _stone + _stonePerTurn);
    }

    public void CalculRessourcesPerTurn()
    {
        int newManaPerTurn = 0;
        int newGoldPerTurn = 0;
        int newWoodPerTurn = 0;
        int newStonePerTurn = 0;

        if (BuildingManager.Instance == null)
        {
            Debug.LogWarning("BuildingManager n'est pas disponible !");
            return;
        }

        foreach (var building in BuildingManager.Instance.BuildingsDataMap)
        {
            switch (building.Value.Type)
            {
                case BuildingType.Temple:
                    newManaPerTurn += building.Value.ProductionPerTurn;
                    break;
                case BuildingType.Town:
                    newGoldPerTurn += building.Value.ProductionPerTurn * building.Value.Level;
                    break;
                case BuildingType.Capital:
                    newGoldPerTurn += building.Value.ProductionPerTurn * building.Value.Level;
                    break;
                case BuildingType.Lumberjack:
                    newWoodPerTurn += building.Value.ProductionPerTurn;
                    break;
                case BuildingType.StoneMine:
                    newStonePerTurn += building.Value.ProductionPerTurn;
                    break;
            }
        }

        if (newManaPerTurn != _manaPerTurn)
        {
            _manaPerTurn = newManaPerTurn;
            OnResourcePerTurnChanged?.Invoke(ResourceType.Mana, _manaPerTurn);
        }

        if (newGoldPerTurn != _goldPerTurn)
        {
            _goldPerTurn = newGoldPerTurn;
            OnResourcePerTurnChanged?.Invoke(ResourceType.Gold, _goldPerTurn);
        }

        if (newWoodPerTurn != _woodPerTurn)
        {
            _woodPerTurn = newWoodPerTurn;
            OnResourcePerTurnChanged?.Invoke(ResourceType.Wood, _woodPerTurn);
        }

        if (newStonePerTurn != _stonePerTurn)
        {
            _stonePerTurn = newStonePerTurn;
            OnResourcePerTurnChanged?.Invoke(ResourceType.Stone, _stonePerTurn);
        }
    }

    public void CalculCapacity()
    {
        int newManaCapacity = 50;
        int newGoldCapacity = 50;
        int newWoodCapacity = 10;
        int newStoneCapacity = 10;

        if (BuildingManager.Instance == null)
        {
            Debug.LogWarning("BuildingManager n'est pas disponible !");
            return;
        }

        foreach (var building in BuildingManager.Instance.BuildingsDataMap)
        {
            switch (building.Value.Type)
            {
                case BuildingType.ManaPile:
                    newManaCapacity += _capacityPerManaPile;
                    break;
                case BuildingType.Town:
                    newGoldCapacity += _capacityPerGoldPile;
                    break;
                case BuildingType.Capital:
                    newGoldCapacity += _capacityPerGoldPile;
                    break;
                case BuildingType.WoodPile:
                    newWoodCapacity += _capacityPerWoodPile;
                    break;
                case BuildingType.StonePile:
                    newStoneCapacity += _capacityPerStonePile;
                    break;
            }
        }

        if (newManaCapacity != _manaCapacity)
        {
            _manaCapacity = newManaCapacity;
            OnResourceCapacityChanged?.Invoke(ResourceType.Mana, _manaCapacity);
        }

        if (newGoldCapacity != _goldCapacity)
        {
            _goldCapacity = newGoldCapacity;
            OnResourceCapacityChanged?.Invoke(ResourceType.Gold, _goldCapacity);
        }

        if (newWoodCapacity != _woodCapacity)
        {
            _woodCapacity = newWoodCapacity;
            OnResourceCapacityChanged?.Invoke(ResourceType.Wood, _woodCapacity);
        }

        if (newStoneCapacity != _stoneCapacity)
        {
            _stoneCapacity = newStoneCapacity;
            OnResourceCapacityChanged?.Invoke(ResourceType.Stone, _stoneCapacity);
        }
    }

    internal void DeductResources(ResourceType resource, int amount)
    {
        switch (resource)
        {
            case ResourceType.Mana:
                SetResource(ResourceType.Mana, _mana - amount);
                break;

            case ResourceType.Gold:
                SetResource(ResourceType.Gold, _gold - amount);
                break;

            case ResourceType.Wood:
                SetResource(ResourceType.Wood, _wood - amount);
                break;

            case ResourceType.Stone:
                SetResource(ResourceType.Stone, _stone - amount);
                break;

            default:
                Debug.LogWarning($"Ressource inconnue : {resource}. Aucune déduction appliquée.");
                break;
        }
    }

    internal bool HasEnoughResources(ResourceType resource, int amount)
    {
        return resource switch
        {
            ResourceType.Mana => _mana >= amount,
            ResourceType.Gold => _gold >= amount,
            ResourceType.Wood => _wood >= amount,
            ResourceType.Stone => _stone >= amount,
            _ => false
        };
    }
}
