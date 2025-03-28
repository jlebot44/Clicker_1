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
    public static event Action<int> OnTilesChanged; // Événement déclenché quand le nombre de tuiles révélées change
    
    // TODO : construire la notification de changmeent sur les générateur de ressoruce
    //public static event Action<int> OnManaGenChanged; // Événement déclenché quand le nombre de tuiles génératrice de mana change

    // Événements pour la barre de progression
    public static event Action<float> OnProgressReset; // Événement pour réinitialiser la barre de progression à la fin du cycle

    
    // gestion des ressources
    [SerializeField] private int _mana = 100; // Ressource de mana
    [SerializeField] private int _gold = 10; // Ressource d'or
    [SerializeField] private int _wood = 0; // Ressource de bois
    [SerializeField] private int _stone = 0; // Ressource de pierre


    private int _manaPerTurn;
    private int _goldPerTurn;
    private int _woodPerTurn;
    private int _stonePerTurn;







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
                CalculRessources();
                GainMana();
                GainGold();
                GainWood();
                GainStone();
                OnProgressReset?.Invoke(_updateInterval); // Envoie un message pour réinitialiser la barre de progression à la fin du cycle
            }
        }
    }

    // Fonction récurrente du gain de mana par cycle
    void GainMana()
    {        
        Mana += _manaPerTurn;
    }

    // Fonction récurrente d'or de mana par cycle
    void GainGold()
    {
        Gold += _goldPerTurn;
    }
    // Fonction récurrente du gain de bois par cycle
    void GainWood()
    {
        Wood += _woodPerTurn;
    }

    // Fonction récurrente du gain de pierre par cycle
    void GainStone()
    {
        Stone += _stonePerTurn;
    }


    void CalculRessources()
    {
        _manaPerTurn = 0;
        _goldPerTurn = 0;
        _woodPerTurn = 0;
        _stonePerTurn = 0;
       
        foreach (var building in BuildingManager.Instance.BuildingsDataMap)
        {
            // Ajoute la production du bâtiment à la ressource correspondante
            switch (building.Value.Type)
            {
                case BuildingType.Temple:
                    _manaPerTurn += building.Value.ProductionPerTurn * building.Value.Level;
                    break;
                case BuildingType.Town:
                    _goldPerTurn += building.Value.ProductionPerTurn * building.Value.Level;
                    break;
                case BuildingType.Lumberjack:
                    _woodPerTurn += building.Value.ProductionPerTurn * building.Value.Level;
                    break;
                case BuildingType.StoneMine:
                    _stonePerTurn += building.Value.ProductionPerTurn * building.Value.Level;
                    break;
                    // Ajoute d'autres types de bâtiments selon le besoin
            }
            
        }
    }




}