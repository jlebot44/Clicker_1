using UnityEngine;
using UnityEngine.UI; // Nécessaire pour l'UI

public class HealthBar : MonoBehaviour
{
    public Image healthFill; // L'image de la barre de vie

    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthFill.fillAmount = Mathf.Clamp01(1 - (currentHealth / maxHealth));
    }
}