using System;
using System.Collections;
using UnityEngine;

public class RessourceManager : MonoBehaviour
{
    public static RessourceManager Instance { get; private set; }  // patern singleton

    public static event Action<int> OnManaChanged; // Événement déclenché quand le mana change
    public static event Action<int> OnTilesChanged; // Événement déclenché quand le mana change

    [SerializeField] private int _mana = 100; // Ressource de mana
    [SerializeField] private int _manaPerLevel = 1; // Combien de mana genere par noveau de ville
    [SerializeField] private float _updateInterval = 5f; // Temps entre chaque mise à jour
    [SerializeField] private int _tiles = 1;
    [SerializeField] private TileManager _tileManager; // Référence vers le gestionnaire des tuiles

    public int TotalManaGain => CalculateTotalManaGain();

    public int Mana
    { 
        get => _mana;
        set
        {
            _mana = Mathf.Max(0, value);
            OnManaChanged?.Invoke(_mana); // Notifie tous les écouteurs que le mana a changé
        }
    }
    public int ManaPerLevel { get => _manaPerLevel; set => _manaPerLevel = Mathf.Max(0, value); }
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


    private IEnumerator UpdateResources()
    {
        while (true)
        {
            yield return new WaitForSeconds(UpdateInterval);

            if (Time.timeScale > 0) // Vérifie que le jeu n'est pas en pause
            {
                GainMana();
            }
        }
    }

    // Fonction récurrente du gain de mana par cycle
    void GainMana()
    {
        
        Mana += TotalManaGain;
        Debug.Log($"Mana +{TotalManaGain}! Total : {Mana}");
    }

    private int CalculateTotalManaGain()
    {
        if (_tileManager == null) return 0;

        int totalGain = 0;

        foreach (var tile in _tileManager.GetClaimedTowns())
        {
            totalGain += tile.BuildingLevel * ManaPerLevel;
        }

        return totalGain;
    }




}