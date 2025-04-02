using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class RessourceManager : MonoBehaviour
{
    public static RessourceManager Instance { get; private set; }  // patern singleton

    public static event Action<int> OnManaChanged; // �v�nement d�clench� quand le mana change
    public static event Action<int> OnGoldChanged; // �v�nement d�clench� quand l'or change
    public static event Action<int> OnWoodChanged; // �v�nement d�clench� quand le bois change
    public static event Action<int> OnStoneChanged; // �v�nement d�clench� quand la pierre change

    public static event Action<int> OnManaCapacityChanged; // �v�nement d�clench� quand la capacite maximum de mana change
    public static event Action<int> OnGoldCapacityChanged; // �v�nement d�clench� quand la capacite maximum de l'or change
    public static event Action<int> OnWoodCapacityChanged; // �v�nement d�clench� quand la capacite maximum du bois change
    public static event Action<int> OnStoneCapacityChanged; // �v�nement d�clench� quand la capacite maximum de la pierre change

    public static event Action<int> OnManaPerTurnChanged; // �v�nement d�clench� quand le gain par tour de mana change
    public static event Action<int> OnGoldPerTurnChanged; // �v�nement d�clench� quand le gain par tour de l'or change
    public static event Action<int> OnWoodPerTurnChanged; // �v�nement d�clench� quand le gain par tour du bois change
    public static event Action<int> OnStonePerTurnChanged; // �v�nement d�clench� quand le gain par tour de la pierre change







    public static event Action<int> OnTilesChanged; // �v�nement d�clench� quand le nombre de tuiles r�v�l�es change

    // TODO : construire la notification de changmeent sur les g�n�rateur de ressoruce
    //public static event Action<int> OnManaGenChanged; // �v�nement d�clench� quand le nombre de tuiles g�n�ratrice de mana change

    // �v�nements pour la barre de progression
    public static event Action<float> OnProgressReset; // �v�nement pour r�initialiser la barre de progression � la fin du cycle


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





    [SerializeField] private float _updateInterval = 5f; // Temps entre chaque mise � jour

    private int _tiles = 1;

    public int Mana
    {
        get => _mana;
        set
        {
            _mana = Mathf.Max(0, value);
            OnManaChanged?.Invoke(_mana); // Notifie tous les �couteurs que le mana a chang�
        }
    }

    public int Gold
    {
        get => _gold;
        set
        {
            _gold = Mathf.Max(0, value);
            OnGoldChanged?.Invoke(_gold); // Notifie tous les �couteurs que l'or a chang�
        }
    }

    public int Wood
    {
        get => _wood;
        set
        {
            _wood = Mathf.Max(0, value);
            OnWoodChanged?.Invoke(_wood); // Notifie tous les �couteurs que le bois a chang�
        }
    }

    public int Stone
    {
        get => _stone;
        set
        {
            _stone = Mathf.Max(0, value);
            OnStoneChanged?.Invoke(_stone); // Notifie tous les �couteurs que le mana a chang�
        }
    }




    public float UpdateInterval { get => _updateInterval; set => _updateInterval = Mathf.Max(0.1f, value); }
    public int Tiles
    {
        get => _tiles;
        set
        {
            _tiles = Mathf.Max(0, value);
            OnTilesChanged?.Invoke(_tiles); // Notifie tous les �couteurs que le nombre de tuile a chang�
        }
    }

    public int ManaCapacity
    {
        get => _manaCapacity;
        set
        {
            _manaCapacity = Mathf.Max(0, value);
            OnManaCapacityChanged?.Invoke(_manaCapacity); // Notifie tous les �couteurs que le mana a chang�
        }
    }

    public int GoldCapacity
    {
        get => _goldCapacity;
        set
        {
            _goldCapacity = Mathf.Max(0, value);
            OnGoldCapacityChanged?.Invoke(_goldCapacity); // Notifie tous les �couteurs que le mana a chang�
        }
    }

    public int WoodCapacity
    {
        get => _woodCapacity;
        set
        {
            _woodCapacity = Mathf.Max(0, value);
            OnWoodCapacityChanged?.Invoke(_woodCapacity); // Notifie tous les �couteurs que le mana a chang�
        }
    }

    public int StoneCapacity
    {
        get => _stoneCapacity;
        set
        {
            _stoneCapacity = Mathf.Max(0, value);
            OnStoneCapacityChanged?.Invoke(_stoneCapacity); // Notifie tous les �couteurs que le mana a chang�
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
            Debug.LogWarning("RessourceManager d�j� existant ! Suppression de l'instance en double.");
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

            if (Time.timeScale > 0) // V�rifie que le jeu n'est pas en pause
            {
                CalculRessourcesPerTurn();
                CalculCapacity();
                GainMana();
                GainGold();
                GainWood();
                GainStone();
                OnProgressReset?.Invoke(_updateInterval); // Envoie un message pour r�initialiser la barre de progression � la fin du cycle
            }
        }
    }

    private void UpdateRessources(Vector3Int @int)
    {
        CalculRessourcesPerTurn();
        CalculCapacity();
    }

    // Fonction r�currente du gain de mana par cycle
    void GainMana()
    {
        Mana += _manaPerTurn;
        if (Mana > ManaCapacity)
            Mana = ManaCapacity;
    }

    // Fonction r�currente d'or de mana par cycle
    void GainGold()
    {
        Gold += _goldPerTurn;
        if (Gold > GoldCapacity)
            Gold = GoldCapacity;

    }
    // Fonction r�currente du gain de bois par cycle
    void GainWood()
    {
        Wood += _woodPerTurn;
        if (Wood > WoodCapacity)
            Wood = WoodCapacity;
    }

    // Fonction r�currente du gain de pierre par cycle
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
        // V�rifie si les valeurs ont chang� avant d'�mettre l'�v�nement
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
        Debug.Log($"Capacit�s de base : Mana={_manaCapacity}, Gold={_goldCapacity}, Wood={_woodCapacity}, Stone={_stoneCapacity}");
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
        // V�rifie si les valeurs ont chang� avant d'�mettre l'�v�nement
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