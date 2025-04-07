[System.Serializable]
public class SaveResourceData
{
    public int Mana;
    public int Gold;
    public int Wood;
    public int Stone;
    public float UpdateInterval;
    public int Tiles;

    // Constructeur
    public SaveResourceData(int mana, int gold, int wood, int stone, float updateInterval, int tiles)
    {
        Mana = mana;
        Gold = gold;
        Wood = wood;
        Stone = stone;
        UpdateInterval = updateInterval;
        Tiles = tiles;
    }
}