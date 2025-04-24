using System.Collections.Generic;

[System.Serializable]
public class SaveBonusEntry
{
    public string BonusId;
    public bool IsActivated;

    public SaveBonusEntry(string bonusName, bool activated)
    {
        BonusId = bonusName;
        IsActivated = activated;
    }
}

[System.Serializable]
public class BonusSaveData
{
    public List<SaveBonusEntry> bonuses;

    public BonusSaveData(List<SaveBonusEntry> bonuses)
    {
        this.bonuses = bonuses;
    }
}