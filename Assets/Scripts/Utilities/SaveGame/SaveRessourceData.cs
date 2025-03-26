[System.Serializable]
public class SaveRessourceData
{
    public int Mana;
    public int ManaGen;
    public int Gold;
    public int GoldPerTurn;
    public int Wood;
    public int WoodPerTurn;
    public int Stone;
    public int StonePerTurn;
    public float UpdateInterval;
    public int Tiles;

    // Constructeur
    public SaveRessourceData(int mana, int manaGen, int gold, int goldPerTurn, int wood, int woodPerTurn, int stone, int stonePerTurn, float updateInterval, int tiles)
    {
        Mana = mana;
        ManaGen = manaGen;
        Gold = gold;
        GoldPerTurn = goldPerTurn;
        Wood = wood;
        WoodPerTurn = woodPerTurn;
        Stone = stone;
        StonePerTurn = stonePerTurn;
        UpdateInterval = updateInterval;
        Tiles = tiles;
    }
}