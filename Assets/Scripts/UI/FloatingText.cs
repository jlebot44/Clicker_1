using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float _duration = 1.5f;
    [SerializeField] private float _speed = 1.5f;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialize(string message, Color color)
    {
        _text.text = message;
        _text.color = color;
        Destroy(gameObject, _duration);
    }

    private void Update()
    {
        transform.position += Vector3.up * _speed * Time.deltaTime;
    }
}
