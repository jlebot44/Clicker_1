using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShrineBonusManager : MonoBehaviour
{
    public static ShrineBonusManager Instance { get; private set; }

    public static event Action OnBonusListChanged;

    private List<ShrineBonusData> _availableBonuses = new();
    private List<ShrineBonusData> _activatedBonuses = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public bool IsActivated(ShrineBonusData bonus)
    {
        return _activatedBonuses.Contains(bonus);
    }

    public List<ShrineBonusData> GetAllKnownBonuses()
    {
        return new List<ShrineBonusData>(_availableBonuses
            .Union(_activatedBonuses)); // pour �viter les doublons
    }



    public bool CanActivate(ShrineBonusData bonus)
    {
        return BuildingResourceService.HasEnoughResources(bonus.activationCost);
    }

    public List<ShrineBonusData> GetActivatedBonuses()
    {
        return new List<ShrineBonusData>(_activatedBonuses);
    }

    public void RegisterBonus(ShrineBonusData bonus)
    {
        if (!_availableBonuses.Contains(bonus))
        {
            _availableBonuses.Add(bonus);
            OnBonusListChanged?.Invoke(); //  d�clenche l'update de l'UI
        }
    }

    public void ActivateBonus(ShrineBonusData bonus)
    {
        if (IsActivated(bonus))
            return; // d�j� activ�, on ne fait rien
        if (CanActivate(bonus))
        {
            BuildingResourceService.DeductResources(bonus.activationCost);
            _activatedBonuses.Add(bonus);
            _availableBonuses.Remove(bonus);

            ApplyEffect(bonus);
            OnBonusListChanged?.Invoke(); //  d�clenche aussi l'update
        }
    }

    public List<ShrineBonusData> GetAvailableBonuses()
    {
        return new List<ShrineBonusData>(_availableBonuses);
    }

    public bool HasEnoughResources(List<ResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            if (!ResourceManager.Instance.HasEnoughResources(cost.resourceType, cost.amount))
            {                
                return false;
            }
        }
        return true;
    }



    private void ApplyEffect(ShrineBonusData bonus)
    {
        // Logique � adapter selon ton syst�me de ressources / stats
        switch (bonus.effectType)
        {
            case BonusEffectType.MoreManaPerTurn:
                //ResourceManager.Instance.ManaPerTurn += 10;
                break;
            // etc.
            case BonusEffectType.ExtraClickPower:
                FogManager.Instance.IncreaseClickPower(bonus.effectValue);
                break;
        }

        Debug.Log($"Bonus activ� : {bonus.bonusName}");
    }
}
