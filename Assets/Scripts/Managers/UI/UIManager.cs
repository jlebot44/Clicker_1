using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Root")]
    [SerializeField] private RectTransform _uiRoot;

    [Header("Prefabs & UI Components")]
    [SerializeField] private GameObject _floatingTextPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowFloatingText(string message, Vector3 worldPosition, Color color)
    {
        Vector3 aboveCell = worldPosition + Vector3Int.up;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(aboveCell);
        GameObject text = Instantiate(_floatingTextPrefab, _uiRoot);
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.position = screenPos;
        text.GetComponent<FloatingText>().Initialize(message, color);
    }
}
