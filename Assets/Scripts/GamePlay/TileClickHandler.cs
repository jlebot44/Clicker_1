using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System;

public class TileClickHandler : MonoBehaviour
{
    public static TileClickHandler Instance { get; private set; }

    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TileClickAnimation _tileClickAnimation;


    public static event Action<Vector3Int> OnTileSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = _tilemap.WorldToCell(mouseWorldPos);

            TileData selectedTileData = TileManager.Instance.GetTileData(cellPosition);

            if (selectedTileData != null)
            {

                _tileClickAnimation.PressTile(cellPosition, _tilemap);

                // �mettre un �v�nement pour dire qu'une tuile a �t� s�lectionn�e
                OnTileSelected?.Invoke(cellPosition);
            }
        }
    }
}