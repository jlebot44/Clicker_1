public class ShrineBuildingData : BuildingData
{
    public ShrineBonusData bonusData;
    public bool isBonusUnlocked;

    public ShrineBuildingData(BuildingType type, ShrineBonusData bonusData) : base(type)
    {
        this.bonusData = bonusData;
        this.isBonusUnlocked = false;
    }
}
