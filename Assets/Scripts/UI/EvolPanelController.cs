using UnityEngine;
using UnityEngine.UI;

public class EvolvePanelController : MonoBehaviour
{
    [SerializeField] private Button _evolveButton;

    private Vector3Int _currentPosition;

    public void Show(Vector3Int cellPos, Transform parent, System.Action onEvolveClicked)
    {
        _currentPosition = cellPos;
        transform.SetParent(parent, false);
        transform.position = TileManager.Instance.BuildingTilemap.GetCellCenterWorld(cellPos) + Vector3.up * 0.5f;
        gameObject.SetActive(true);

        _evolveButton.onClick.RemoveAllListeners();
        _evolveButton.onClick.AddListener(() => {
            onEvolveClicked?.Invoke();
            Hide();
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
