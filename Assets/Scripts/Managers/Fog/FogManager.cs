using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FogManager : MonoBehaviour
{
    public static FogManager Instance { get; private set; }  // patern singleton

    [SerializeField] private Tilemap _fogTilemap; // Tilemap du brouillard
    [SerializeField] private Camera _mainCamera; // Caméra principale
    [SerializeField] private int _fogDirectionRadius = 1; // rayon de visibilité du brouillard - 1 par defaut
    [SerializeField] private CameraEffectManager _cameraEffectManager;
    [SerializeField] private FogEffectManager _fogEffectManager;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private TileClickAnimation _tileClickAnimation;
    [SerializeField] private GameObject _floatingTextPrefab;
    [SerializeField] private FogHealthBarController _healthBarController;
    [SerializeField] private ShrinePlacer _shrinePlacer;


    [SerializeField] private int clickPower = 1; // puissance du clic - cout en mana du clic

    private int _manaSpent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("FogManager déjà existant ! Suppression de l'instance en double.");
            Destroy(gameObject);
            return;
        }
    }


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

    private void HandleTileSelected(Vector3Int cellPosition)
    {
        TileData tileData = TileManager.Instance.GetTileData(cellPosition);
        if (tileData != null && tileData.CurrentFog > 0) // Clic gauche pour dissiper le brouillard
        {
            if (!ResourceManager.Instance.HasEnoughResources(ResourceType.Mana, clickPower))
            {
                // si mana insuffisant
                ShowFloatingText(cellPosition, "Mana inssuffisant", Color.red);
                return;
            }
            
            if (IsAdjacentToClearedTile(cellPosition))
            {
                HandleFogClick(cellPosition, tileData);
            }
            else
            {
                // sinon, c'est que la case n'est pas reliée aux cases révélées
                ShowFloatingText(cellPosition, "la case doit être adjacente a une case révélée", Color.red);
            }            
        }
    }


    private void HandleFogClick(Vector3Int cellPosition, TileData tileData)
    {
        if (clickPower > tileData.CurrentFog)
        {
            _manaSpent = tileData.CurrentFog;
        }
        else
        {
            _manaSpent = clickPower;
        }
            

            // animation
            _tileClickAnimation.ShakeTile(cellPosition, _fogTilemap);


        // Réduire le brouillard (chaque clic enlève 1 point par clickPower)
        tileData.CurrentFog -= _manaSpent;

        // instanciation de la barre de vie du brouillard
        _healthBarController.Spawn(cellPosition);

        // mise à jour de la barre de vie du brouillard
        _healthBarController.UpdateHealthBar(cellPosition, tileData);

        // réduire le total de mana
        
        ResourceManager.Instance.DeductResources(ResourceType.Mana, _manaSpent);

        // pop du floatingText qui indique le cout en ressource
        ShowFloatingText(cellPosition, $"-{_manaSpent} Mana", Color.cyan);

        // Si le brouillard est complètement dissipé, retirer la tuile du FogTilemap
        if (tileData.CurrentFog <= 0)
        {
            RevealTile(cellPosition, tileData);
            if (tileData.Building == BuildingType.Town)
            {
                ResourceManager.Instance.CalculResources();
            }
        }
    }

    private void ShowFloatingText(Vector3Int cellPosition, string message, Color color)
    {
        Vector3Int aboveCell = cellPosition + Vector3Int.up;
        Vector3 worldPosition = _fogTilemap.GetCellCenterWorld(aboveCell);
        GameObject floatingText = Instantiate(_floatingTextPrefab, worldPosition, Quaternion.identity);
        floatingText.GetComponent<FloatingText>().Initialize(message, color);
    }


    bool IsAdjacentToClearedTile(Vector3Int cellPosition)
    {
        // Définition des directions adjacentes (haut, bas, gauche, droite)
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

            if (neighborTile != null && neighborTile.CurrentFog <= 0)
            {
                return true; // Il y a une case adjacente sans brouillard
            }
        }
        return false;
    }


    void ReduceNeighbourFog(Vector3Int cellPosition)
    {
        // Définition des directions adjacentes (haut, bas, gauche, droite)
        Vector3Int[] directions = GenerateDirections(_fogDirectionRadius);

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = cellPosition + dir;

            if (_fogTilemap.HasTile(neighborPos))
            {
                // Récupérer la tuile d'origine
                Tile originalTile = _fogTilemap.GetTile<Tile>(neighborPos);

                // Cloner la tuile d'origine
                Tile modifiableTile = ScriptableObject.Instantiate(originalTile);

                // Modifier l'alpha de la couleur de la tuile clonée
                Color newColor = modifiableTile.color;
                newColor.a = .95f; // Diviser l'alpha par 2
                modifiableTile.color = newColor;

                // Remplacer la tuile dans la Tilemap
                _fogTilemap.SetTile(neighborPos, modifiableTile);
            }
        }
    }

    Vector3Int[] GenerateDirections(int radius)
    {
        // List pour stocker les directions générées
        var directions = new System.Collections.Generic.List<Vector3Int>();

        // Générer les directions autour de la case centrale
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                // On ignore la direction (0,0), c'est la case centrale
                if (x == 0 && y == 0) continue;

                // Ajouter le vecteur direction à la liste
                directions.Add(new Vector3Int(x, y, 0));
            }
        }

        // Retourner le tableau de directions
        return directions.ToArray();
    }
 

    private void RevealTile(Vector3Int cellPosition, TileData tileData)
    {
        ClearFogTile(cellPosition);
        ClaimTile(tileData);
        HandleBuildingReveal(cellPosition, tileData);
        HandleTileRevealEffects(cellPosition, tileData);
        _healthBarController.Destroy(cellPosition);
        UpdateTileDiscoveryCount();
    }


    private void ClearFogTile(Vector3Int cellPosition)
    {
        _fogTilemap.SetTile(cellPosition, null);
        ReduceNeighbourFog(cellPosition);
        StartCoroutine(_cameraEffectManager.Shake(_mainCamera, 0.1f, 0.06f));
    }

    private void ClaimTile(TileData tileData)
    {
        tileData.IsClaimed = true;
    }

    private void HandleBuildingReveal(Vector3Int cellPosition, TileData tileData)
    {
        if (tileData.Building == BuildingType.None)
            return;

        if (tileData.Building == BuildingType.BonusShrine)
        {
            Debug.Log("Shrine révélé !");

            // Récupérer le ShrineBonusData via le ShrinePlacer
            ShrineBonusData bonusData = _shrinePlacer.FindBonusDataAtPosition(cellPosition);
            if (bonusData != null)
            {
                BuildingManager.Instance.AddShrineBuilding(cellPosition, bonusData);
                ShrineBonusManager.Instance.RegisterBonus(bonusData);
            }
            else
            {
                Debug.LogWarning($"Aucun ShrineBonusData trouvé pour la position {cellPosition}");
                // On fallback sur une construction standard si nécessaire
                BuildingManager.Instance.AddBuilding(cellPosition, tileData.Building);
            }
        }
        else
        {
            BuildingManager.Instance.AddBuilding(cellPosition, tileData.Building);
        }

        // Actions spécifiques pour une ville révélée
        if (tileData.Building == BuildingType.Town)
        {
            _fogEffectManager.PlayTownRevealSFX();
            _fogEffectManager.PlayCaptureEffect(cellPosition, _fogTilemap);
            StartCoroutine(_cameraEffectManager.Shake(_mainCamera));
            ResourceManager.Instance.CalculResources();
        }
    }





    private void HandleTileRevealEffects(Vector3Int cellPosition, TileData tileData)
    {
        _fogEffectManager.PlayTileRevealVFX(cellPosition, _fogTilemap);
        _fogEffectManager.PlayTileRevealSFX();

    }
    private void UpdateTileDiscoveryCount()
    {
        ResourceManager.Instance.Tiles++;
    }


    public void IncreaseClickPower(int amount)
    {
        clickPower += amount;
        Debug.Log($"Click Power augmenté ! Nouveau : {clickPower}");
    }


}
