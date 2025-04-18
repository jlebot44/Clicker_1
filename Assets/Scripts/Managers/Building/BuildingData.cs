using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class BuildingData
{
    [SerializeField] private BuildingType _type;
    [SerializeField] private int _productionPerTurn;
    [SerializeField] private int _level;
    [SerializeField] private int _productionBonus;

    public BuildingType Type { get => _type; set => _type = value; }
    public int ProductionPerTurn { get => _productionPerTurn; set => _productionPerTurn = value; }
    public int Level { get => _level; set => _level = value; }
    public int ProductionBonus { get => _productionBonus; set => _productionBonus = value; }


    public BuildingData(BuildingType type)
    {
        _type = type;
        _productionPerTurn = 1;
        _level = 1;
        _productionBonus = 0;
    }

}


