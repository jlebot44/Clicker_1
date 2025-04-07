using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    [SerializeField] private Slider _progressBar; // La barre de progression (Slider)

    private float _progressSpeed; // Vitesse de progression calcul�e
    private float _targetTime; // Temps donn� pour remplir la barre de progression

    private void OnEnable()
    {
        // Abonnement � l'�v�nement OnProgressReset pour r�initialiser la barre de progression
        ResourceManager.OnProgressReset += ResetProgressBar;
    }

    private void OnDisable()
    {
        // Se d�sabonner de l'�v�nement OnProgressReset lors de la d�sactivation
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
        // Mise � jour continue de la barre de progression
        if (_progressBar.value < 1f)
        {
            _progressBar.value += _progressSpeed * Time.deltaTime;
        }
    }

    private void ResetProgressBar(float updateInterval)
    {
        // R�initialiser la barre de progression � z�ro � la fin du cycle
        _progressBar.value = 0f;

        // Calcul de la vitesse de progression en fonction du temps donn� pour remplir la barre
        _targetTime = updateInterval;
        _progressSpeed = 1f / _targetTime; // Vitesse n�cessaire pour atteindre 100% en 'updateInterval' secondes
    }
}

