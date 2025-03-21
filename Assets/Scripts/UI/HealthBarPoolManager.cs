using System.Collections.Generic;
using UnityEngine;

public class HealthBarPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject _healthBarPrefab;  // Le prefab de la barre de vie
    [SerializeField] private int _poolSize = 10;  // Nombre d'objets à conserver dans le pool

    private Queue<GameObject> _healthBarPool = new Queue<GameObject>();

    private void Start()
    {
        // Remplir le pool avec des barres de vie à l'avance
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject healthBar = Instantiate(_healthBarPrefab);
            healthBar.SetActive(false);  // Les barres sont inactives au départ
            _healthBarPool.Enqueue(healthBar);
        }
    }

    // Récupérer une barre de vie depuis le pool
    public GameObject GetHealthBar(Vector3 position)
    {
        GameObject healthBar;

        // Si le pool est vide, on crée un nouvel objet
        if (_healthBarPool.Count == 0)
        {
            healthBar = Instantiate(_healthBarPrefab);
        }
        else
        {
            healthBar = _healthBarPool.Dequeue();  // Prendre une barre de vie inactive du pool
        }

        healthBar.transform.position = position;
        healthBar.SetActive(true);  // Réactiver l'objet
        return healthBar;
    }

    // Rendre une barre de vie au pool
    public void ReturnHealthBar(GameObject healthBar)
    {
        healthBar.SetActive(false);  // Désactiver l'objet
        _healthBarPool.Enqueue(healthBar);  // Le remettre dans le pool
    }
}