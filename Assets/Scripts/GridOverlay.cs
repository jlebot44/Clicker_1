using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GridOverlay : MonoBehaviour
{
    [SerializeField] private Color gridColor = Color.white; // Couleur de la grille
    [SerializeField] private float cellSize = 1f; // Taille des cellules
    [SerializeField] private Tilemap referenceTilemap; // Référence pour caler la grille
    [SerializeField] private int sortingOrder;

    private List<LineRenderer> lines = new List<LineRenderer>();
    private Camera cam;
    private Vector3 gridStartPosition;
    private Vector2Int gridSize;

    void Start()
    {
        cam = Camera.main;
        if (referenceTilemap != null)
        {
            CalculateGridBounds();
            CreateGrid();
        }
        else
        {
            Debug.LogError("[GridOverlay] Assigne une Tilemap de référence dans l'Inspector !");
        }
    }

    void CalculateGridBounds()
    {
        // Récupérer les limites de la Tilemap
        BoundsInt bounds = referenceTilemap.cellBounds;
        gridSize = new Vector2Int(bounds.size.x, bounds.size.y);
        gridStartPosition = referenceTilemap.CellToWorld(bounds.min); // Début de la grille en coordonnées monde
    }

    void CreateGrid()
    {
        for (int x = 0; x <= gridSize.x; x++)
            CreateLine(new Vector3(gridStartPosition.x + x * cellSize, gridStartPosition.y, 0),
                       new Vector3(gridStartPosition.x + x * cellSize, gridStartPosition.y + gridSize.y * cellSize, 0));

        for (int y = 0; y <= gridSize.y; y++)
            CreateLine(new Vector3(gridStartPosition.x, gridStartPosition.y + y * cellSize, 0),
                       new Vector3(gridStartPosition.x + gridSize.x * cellSize, gridStartPosition.y + y * cellSize, 0));
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.parent = transform;
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = gridColor;
        lineRenderer.endColor = gridColor;

        lineRenderer.sortingLayerName = "Foreground";
        lineRenderer.sortingOrder = sortingOrder;

        lines.Add(lineRenderer);
    }
}
