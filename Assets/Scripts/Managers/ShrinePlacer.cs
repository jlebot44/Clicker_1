using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShrinePlacement
{
    public Vector3Int position;
    public ShrineBonusData bonusData;
}

public class ShrinePlacer : MonoBehaviour
{
    public static ShrinePlacer Instance { get; private set; }

    [SerializeField] private List<ShrinePlacement> placedShrines;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        foreach (var shrine in placedShrines)
        {
            // Ajoute le shrine à BuildingManager
            BuildingManager.Instance.AddShrineBuilding(shrine.position, shrine.bonusData);

            // Si la case est déjà claim (révélée), on enregistre directement le bonus
            TileData tileData = TileManager.Instance.DataManager.GetTileData(shrine.position);
            if (tileData != null && tileData.IsClaimed)
            {
                ShrineBonusManager.Instance.RegisterBonus(shrine.bonusData);
            }
        }
    }

    public ShrineBonusData FindBonusDataAtPosition(Vector3Int position)
    {
        foreach (var shrine in placedShrines)
        {
            if (shrine.position == position)
                return shrine.bonusData;
        }
        return null;
    }
    public ShrineBonusData FindBonusDataByName(string name)
    {
        foreach (var shrine in placedShrines)
        {
            if (shrine.bonusData != null && shrine.bonusData.bonusName == name)
                return shrine.bonusData;
        }
        return null;
    }

}
