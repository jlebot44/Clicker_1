[System.Serializable]
public class SaveRessourceData
{

    public int mana;
    public float updateInterval;
    public int tiles;


    public SaveRessourceData(int mana, float updateInterval, int tiles)
    {
        this.mana = mana;
        this.updateInterval = updateInterval;
        this.tiles = tiles;
    }
}