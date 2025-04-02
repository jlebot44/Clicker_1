using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class RessourceManager : MonoBehaviour
{
    public static RessourceManager Instance { get; private set; }  // patern singleton

    public static event Action<int> OnManaChanged; // Événement déclenché quand le mana change
    public static event Action<int> OnGoldChanged; // Événement déclenché quand l'or change
    public static event Action<int> OnWoodChanged; // Événement déclenché quand le bois change
    public static event Action<int> OnStoneChanged; // Événement déclenché quand la pierre change

    public static event Action<int> OnManaCapacityChanged; // Événement déclenché quand la capacite maximum de mana change
    public static event Action<int> OnGoldCapacityChanged; // Événement déclenché quand la capacite maximum de l'or change
    public static event Action<int> OnWoodCapacityChanged; // Événement déclenché quand la capacite maximum du bois change
    public static event Action<int> OnStoneCapacityChanged; // Événement déclenché quand la capacite maximum de la pierre change

    public static event Action<int> OnManaPerTurnChanged; // Événement déclenché quand le gain par tour de mana change
    public static event Action<int> OnGoldPerTurnChanged; // Événement déclenché quand le gain par tour de l'or change
    public static event Action<int> OnWoodPerTurnChanged; // Événement déclenché quand le gain par tour du bois change
    public static event Action<int> OnStonePerTurnChanged; // Événement déclenché quand le gain par tour de la pierre change







    public static event Action<int> OnTilesChanged; // Événement déclenché quand le nombre de tuiles révélées change

    // TODO : construire la notification de changmeent sur les générateur de ressoruce
    //public static event Action<int> OnManaGenChanged; // Événement déclenché quand le nombre de tuiles génératrice de mana change

    // Événements pour la barre de progression
    public static event Action<float> OnProgressReset; // Événement pour réinitialiser la barre de progression à la fin du cycle


    // gestion des ressources
    [SerializeField] private int _mana = 50; // Ressource de mana
    [SerializeField] private int _gold = 10; // Ressource d'or
    [SerializeField] private int _wood = 0; // Ressource de bois
    [SerializeField] private int _stone = 0; // Ressource de pierre


    private int _manaPerTurn;
    private int _goldPerTurn;
    private int _woodPerTurn;
    private int _stonePerTurn;

    private int _manaCapacity = 50;
    private int _goldCapacity = 50;
    private int _woodCapacity = 10;
    private int _stoneCapacity = 10;


    //TODO -> ajouter ces info dans les buildingData
    [SerializeField] private int _capacityPerManaPile = 10;
    [SerializeField] private int _capacityPerWoodPile = 10;
    [SerializeField] private int _capacityPerStonePile = 10;
    [SerializeField] private int _capacityPerGoldPile = 10;





    [SerializeField] private float _updateInterval = 5f; // Temps entre chaque mise à jour

    private int _tiles = 1;

    public int Mana
    {
        get => _mana;
        set
        {
            _mana = Mathf.Max(0, value);
            OnManaChanged?.Invoke(_mana); // Notifie tous les écouteurs que le mana a changé
        }
    }

    public int Gold
    {
        get => _gold;
        set
        {
            _gold = Mathf.Max(0, value);
            OnGoldChanged?.Invoke(_gold); // Notifie tous les écouteurs que l'or a changé
        }
    }

    public int Wood
    {
        get => _wood;
        set
        {
            _wood = Mathf.Max(0, value);
            OnWoodChanged?.Invoke(_wood); // Notifie tous les écouteurs que le bois a changé
        }
    }

    public int Stone
    {
        get => _stone;
        set
        {
            _stone = Mathf.Max(0, value);
            OnStoneChanged?.Invoke(_stone); // Notifie tous les écouteurs que le mana a changé
        }
    }




    public float UpdateInterval { get => _updateInterval; set => _updateInterval = Mathf.Max(0.1f, value); }
    public int Tiles
    {
        get => _tiles;
        set
        {
            _tiles = Mathf.Max(0, value);
            OnTilesChanged?.Invoke(_tiles); // Notifie tous les écouteurs que le nombre de tuile a changé
        }
    }

    public int ManaCapacity
    {
        get => _manaCapacity;
        set
        {
            _manaCapacity = Mathf.Max(0, value);
            OnManaCapacityChanged?.Invoke(_manaCapacity); // Notifie tous les écouteurs que le mana a changé
        }
    }

    public int GoldCapacity
    {
        get => _goldCapacity;
        set
        {
            _goldCapacity = Mathf.Max(0, value);
            OnGoldCapacityChanged?.Invoke(_goldCapacity); // Notifie tous les écouteurs que le mana a changé
        }
    }

    public int WoodCapacity
    {
        get => _woodCapacity;
        set
        {
            _woodCapacity = Mathf.Max(0, value);
            OnWoodCapacityChanged?.Invoke(_woodCapacity); // Notifie tous les écouteurs que le mana a changé
        }
    }

    public int StoneCapacity
    {
        get => _stoneCapacity;
        set
        {
            _stoneCapacity = Mathf.Max(0, value);
            OnStoneCapacityChanged?.Invoke(_stoneCapacity); // Notifie tous les écouteurs que le mana a changé
        }
    }

    public void OnEnable()
    {
        BuildingManager.OnBuildingConstructed += UpdateRessources;
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

            if (Time.timeScale > 0) // Vérifie que le jeu n'est pas en pause
            {
                CalculRessourcesPerTurn();
                CalculCapacity();
                GainMana();
                GainGold();
                GainWood();
                GainStone();
                OnProgressReset?.Invoke(_updateInterval); // Envoie un message pour réinitialiser la barre de progression à la fin du cycle
            }
        }
    }

    private void UpdateRessources(Vector3Int @int)
    {
        CalculRessourcesPerTurn();
        CalculCapacity();
    }

    // Fonction récurrente du gain de mana par cycle
    void GainMana()
    {
        Mana += _manaPerTurn;
        if (Mana > ManaCapacity)
            Mana = ManaCapacity;
    }

    // Fonction récurrente d'or de mana par cycle
    void GainGold()
    {
        Gold += _goldPerTurn;
        if (Gold > GoldCapacity)
            Gold = GoldCapacity;

    }
    // Fonction récurrente du gain de bois par cycle
    void GainWood()
    {
        Wood += _woodPerTurn;
        if (Wood > WoodCapacity)
            Wood = WoodCapacity;
    }

    // Fonction récurrente du gain de pierre par cycle
    void GainStone()
    {
        Stone += _stonePerTurn;
        if (Stone > StoneCapacity)
            Stone = StoneCapacity;
    }


    void CalculRessourcesPerTurn()
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
                    newGoldPerTurn += building.Value.ProductionPerTurn;
                    break;
                case BuildingType.Lumberjack:
                    newWoodPerTurn += building.Value.ProductionPerTurn;
                    break;
                case BuildingType.StoneMine:
                    newStonePerTurn += building.Value.ProductionPerTurn;
                    break;
            }
        }
        // Vérifie si les valeurs ont changé avant d'émettre l'événement
        if (newManaPerTurn != _manaPerTurn)
        {
            _manaPerTurn = newManaPerTurn;
            OnManaPerTurnChanged?.Invoke(_manaPerTurn);
        }

        if (newGoldPerTurn != _goldPerTurn)
        {
            _goldPerTurn = newGoldPerTurn;
            OnGoldPerTurnChanged?.Invoke(_goldPerTurn);
        }

        if (newWoodPerTurn != _woodPerTurn)
        {
            _woodPerTurn = newWoodPerTurn;
            OnWoodPerTurnChanged?.Invoke(_woodPerTurn);
        }

        if (newStonePerTurn != _stonePerTurn)
        {
            _stonePerTurn = newStonePerTurn;
            OnStonePerTurnChanged?.Invoke(_stonePerTurn);
        }
    }

    void CalculCapacity()
    {
        Debug.Log($"Capacités de base : Mana={_manaCapacity}, Gold={_goldCapacity}, Wood={_woodCapacity}, Stone={_stoneCapacity}");
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
                case BuildingType.WoodPile:
                    newWoodCapacity += _capacityPerWoodPile;
                    break;
                case BuildingType.StonePile:
                    newStoneCapacity += _capacityPerStonePile;
                    break;
            }
        }
        // Vérifie si les valeurs ont changé avant d'émettre l'événement
        if (newManaCapacity != _manaCapacity)
        {
            _manaCapacity = newManaCapacity;
            OnManaCapacityChanged?.Invoke(_manaCapacity);
        }

        if (newGoldCapacity != _goldCapacity)
        {
            _goldCapacity = newGoldCapacity;
            OnGoldCapacityChanged?.Invoke(_goldCapacity);
        }

        if (newWoodCapacity != _woodCapacity)
        {
            _woodCapacity = newWoodCapacity;
            OnWoodCapacityChanged?.Invoke(_woodCapacity);
        }

        if (newStoneCapacity != _stoneCapacity)
        {
            _stoneCapacity = newStoneCapacity;
            OnStoneCapacityChanged?.Invoke(_stoneCapacity);
        }
    }
}