using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpecialBonus")]
public class ShrineBonusData : ScriptableObject
{
    public string bonusName;
    public string description;
    public Sprite icon;
    public List<ResourceCost> activationCost;
    public BonusEffectType effectType; // Enum custom : e.g., +10 mana par tour, etc.

    [Header("Paramètres d'effet")]
    public int effectValue = 1;
}