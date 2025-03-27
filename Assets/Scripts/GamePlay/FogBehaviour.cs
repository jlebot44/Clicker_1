using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FogManager : MonoBehaviour
{
    [SerializeField] private Tilemap _fogTilemap; // Tilemap du brouillard
    [SerializeField] private Camera _mainCamera; // Cam�ra principale
    [SerializeField] private int _fogDirectionRadius = 1; // rayon de visibilit� du brouillard - 1 par defaut
    [SerializeField] private HealthBarPoolManager _healthBarPoolManager;  // R�f�rence au Pool Manager
    [SerializeField] private GameObject _vfxNewTile; // VFX de revelation d'une tuile
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _sfxNewTile;



    [SerializeField] private int clickPower = 1; // puissance du clic - cout en mana du clic

    Dictionary<Vector3Int, GameObject> healthBars = new Dictionary<Vector3Int, GameObject>();

    private void OnEnable()
    {
        TileClickHandler.OnTileSelected += HandleTileSelected;
    }


    void Start()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    private void HandleTileSelected(Vector3Int cellPosition, TileData tileData)
    {
        if (tileData != null && tileData.CurrentFog > 0) // Clic gauche pour dissiper le brouillard
        {
            if (RessourceManager.Instance.Mana < clickPower)
            {
                print("Mana inssuffisant");
                return;
            }
            
            if (IsAdjacentToClearedTile(cellPosition))
            {
                ProcessFogClick(cellPosition, tileData);
            }
            else
            {
                print("la case doit �tre adjacente a une case r�v�l�e");
            }            
        }
    }

    private void ProcessFogClick(Vector3Int cellPosition, TileData tileData)
    {
        // R�duire le brouillard (chaque clic enl�ve 1 point par clickPower)
        tileData.CurrentFog -= clickPower;

        // instanciation de la barre de vie du brouillard
        SpawnHealthBar(cellPosition);

        // mise � jour de la barre de vie du brouillard
        UpdateHealthBar(cellPosition, tileData);

        // r�duire le total de mana
        RessourceManager.Instance.Mana -= clickPower;

        // Si le brouillard est compl�tement dissip�, retirer la tuile du FogTilemap
        if (tileData.CurrentFog <= 0)
        {
            RevealTile(cellPosition, tileData);
        }
    }

    bool IsAdjacentToClearedTile(Vector3Int cellPosition)
    {
        // D�finition des directions adjacentes (haut, bas, gauche, droite)
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0), // Droite
            new Vector3Int(-1, 0, 0), // Gauche
            new Vector3Int(0, 1, 0), // Haut
            new Vector3Int(0, -1, 0) // Bas
        };

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = cellPosition + dir;
            TileData neighborTile = TileManager.Instance.GetTileData(neighborPos);

            if (neighborTile != null && neighborTile.CurrentFog == 0)
            {
                return true; // Il y a une case adjacente sans brouillard
            }
        }
        return false;
    }


    void ReduceNeighbourFog(Vector3Int cellPosition)
    {
        // D�finition des directions adjacentes (haut, bas, gauche, droite)
        Vector3Int[] directions = GenerateDirections(_fogDirectionRadius);

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = cellPosition + dir;

            if (_fogTilemap.HasTile(neighborPos))
            {
                // R�cup�rer la tuile d'origine
                Tile originalTile = _fogTilemap.GetTile<Tile>(neighborPos);

                // Cloner la tuile d'origine
                Tile modifiableTile = ScriptableObject.Instantiate(originalTile);

                // Modifier l'alpha de la couleur de la tuile clon�e
                Color newColor = modifiableTile.color;
                newColor.a = .95f; // Diviser l'alpha par 2
                modifiableTile.color = newColor;

                // Remplacer la tuile dans la Tilemap
                _fogTilemap.SetTile(neighborPos, modifiableTile);
            }
        }

        Vector3Int[] GenerateDirections(int radius)
        {
            // List pour stocker les directions g�n�r�es
            var directions = new System.Collections.Generic.List<Vector3Int>();

            // G�n�rer les directions autour de la case centrale
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    // On ignore la direction (0,0), c'est la case centrale
                    if (x == 0 && y == 0) continue;

                    // Ajouter le vecteur direction � la liste
                    directions.Add(new Vector3Int(x, y, 0));
                }
            }

            // Retourner le tableau de directions
            return directions.ToArray();
        }
    }

    void DestroyHealthBar(Vector3Int cellPosition)
    {
        if (healthBars.ContainsKey(cellPosition))
        {
            GameObject healthBar = healthBars[cellPosition];
            healthBars.Remove(cellPosition);

            // Rendre la barre au pool au lieu de la d�truire
            _healthBarPoolManager.ReturnHealthBar(healthBar);
        }
    }

    void SpawnHealthBar(Vector3Int cellPosition)
    {
        if (!healthBars.ContainsKey(cellPosition)) // V�rifie si une barre existe d�j�
        {
            Vector3 worldPosition = _fogTilemap.GetCellCenterWorld(cellPosition);
            GameObject healthBar = _healthBarPoolManager.GetHealthBar(worldPosition);
            healthBars[cellPosition] = healthBar;
        }
    }

    void UpdateHealthBar(Vector3Int cellPosition, TileData tile)
    {
        if (healthBars.ContainsKey(cellPosition))
        {
            HealthBar healthBar = healthBars[cellPosition].GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.SetHealth(tile.CurrentFog, tile.InitialFog);
            }
        }
    }
    private void RevealTile(Vector3Int cellPosition, TileData tileData)
    {
        _fogTilemap.SetTile(cellPosition, null);
        ReduceNeighbourFog(cellPosition);

        // Modifier la tuile pour la marquer comme revendiqu�e
        tileData.IsClaimed = true;

        // Si batiment fixe : l'ajouter au dictionnaire
        if (tileData.Building != BuildingType.None)
        {
            BuildingManager.Instance.AddBuilding(cellPosition, tileData.Building);
        }
            


        // Destruction de la barre de vie
        DestroyHealthBar(cellPosition);

        // Jouer l'effet sonore
        if (_audioSource != null && _sfxNewTile != null)
        {
            _audioSource.PlayOneShot(_sfxNewTile);
        }

        // Instancier l'effet de particules
        Vector3 worldPosition = _fogTilemap.GetCellCenterWorld(cellPosition);
        GameObject effect = Instantiate(_vfxNewTile, worldPosition, Quaternion.identity);

        // D�truire l'effet apr�s un court instant
        Destroy(effect, 1.5f);

        // incr�menter le nombre de tuiles decouvertes
        RessourceManager.Instance.Tiles++;

        
    }


}
