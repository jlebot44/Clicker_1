using TMPro;
using UnityEngine;

public class CityLevelMarker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetLevel(int level)
    {
        _text.text = level.ToString();
    }
}
