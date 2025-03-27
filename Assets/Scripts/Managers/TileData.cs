using UnityEngine;

[System.Serializable]
public class TileData
{
    [SerializeField] private GroundType _ground;
    [SerializeField] private ReliefType _relief;
    [SerializeField] private BuildingType _building;
    [SerializeField] private int _buildingLevel;
    [SerializeField] private int _initialFog; // Niveau de brouillard : 0 = pas de brouillard, > 0 = brouillard
    [SerializeField] private int _currentFog;
    [SerializeField] private bool _isClaimed; // faux par defaut, passe à true lors de la decouverte de la tuile.
    [SerializeField] private bool _isConnectedToCapital;

    public bool IsClaimed { get => _isClaimed; set => _isClaimed = value; }
    public bool IsConnectedToCapital { get => _isConnectedToCapital; set => _isConnectedToCapital = value; }
    public BuildingType Building { get => _building; set => _building = value; }
    public GroundType Ground { get => _ground; set => _ground = value; }
    public ReliefType Relief { get => _relief; set => _relief = value; }
    public int InitialFog { get => _initialFog; set => _initialFog = value; }
    public int CurrentFog { get => _currentFog; set => _currentFog = value; }

    public TileData(GroundType ground, ReliefType relief, BuildingType building, int fog)
    {
        Ground = ground;
        Relief = relief;
        Building = building;
        InitialFog = fog;
        CurrentFog = fog;
        IsClaimed = false;
        IsConnectedToCapital = false;


    }

    
}

