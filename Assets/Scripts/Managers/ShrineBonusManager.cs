using System;
using System.Collections.Generic;
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
