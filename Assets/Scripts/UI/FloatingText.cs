using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float _duration = 1.5f;
    [SerializeField] private float _speed = 50f; // en pixels/s
    private TextMeshProUGUI _text;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(string message, Color color)
    {
        _text.text = message;
        _text.color = color;
        Destroy(gameObject, _duration);
    }

    private void Update()
    {
        // Monter dans l'UI
        _rectTransform.anchoredPosition += Vector2.up * _speed * Time.deltaTime;
    }
}