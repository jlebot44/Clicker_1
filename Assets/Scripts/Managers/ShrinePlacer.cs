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
    [SerializeField] private List<ShrinePlacement> placedShrines;

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
}
