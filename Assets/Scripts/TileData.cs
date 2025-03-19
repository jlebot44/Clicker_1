using UnityEngine;

public class TileData
{
    public GroundType Ground;
    public ReliefType Relief;
    public BuildingType Building;
    public int BuildingLevel;
    public int InitialFog; // Niveau de brouillard : 0 = pas de brouillard, > 0 = brouillard
    public int CurrentFog;
    public bool isClaimed; // faux par defaut, passe à true lors de la decouverte de la tuile.
    public bool isConnectedToCapital;

    public TileData(GroundType ground, ReliefType relief, BuildingType building, int fog)
    {
        Ground = ground;
        Relief = relief;
        Building = building;
        InitialFog = fog;
        CurrentFog = fog;
        // Si c'est une ville, on commence au niveau 1
        BuildingLevel = (building == BuildingType.Town) ? 1 : 0;
        isClaimed = false;
        isConnectedToCapital = false;


    }
}

