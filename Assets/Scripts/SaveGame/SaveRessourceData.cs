[System.Serializable]
public class SaveRessourceData
{

    public int mana;
    public int manaPerLevel;
    public float updateInterval;
    public int tiles;


    public SaveRessourceData(int mana, int manaPerLevel, float updateInterval, int tiles)
    {
        this.mana = mana;
        this.manaPerLevel = manaPerLevel;
        this.updateInterval = updateInterval;
        this.tiles = tiles;
    }
}