using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    [SerializeField] private Slider _progressBar; // La barre de progression (Slider)

    private float _progressSpeed; // Vitesse de progression calculée
    private float _targetTime; // Temps donné pour remplir la barre de progression

    private void OnEnable()
    {
        // Abonnement à l'événement OnProgressReset pour réinitialiser la barre de progression
        ResourceManager.OnProgressReset += ResetProgressBar;
    }

    private void OnDisable()
    {
        // Se désabonner de l'événement OnProgressReset lors de la désactivation
        ResourceManager.OnProgressReset -= ResetProgressBar;
    }

    private void Start()
    {
        // Initialisation de la barre de progression
        _progressBar.value = 0f;
        ResetProgressBar(ResourceManager.Instance.UpdateInterval);
    }

    private void Update()
    {
        // Mise à jour continue de la barre de progression
        if (_progressBar.value < 1f)
        {
            _progressBar.value += _progressSpeed * Time.deltaTime;
        }
    }

    private void ResetProgressBar(float updateInterval)
    {
        // Réinitialiser la barre de progression à zéro à la fin du cycle
        _progressBar.value = 0f;

        // Calcul de la vitesse de progression en fonction du temps donné pour remplir la barre
        _targetTime = updateInterval;
        _progressSpeed = 1f / _targetTime; // Vitesse nécessaire pour atteindre 100% en 'updateInterval' secondes
    }
}

