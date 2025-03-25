using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private bool _isPaused;

    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    private void Start()
    {
        IsPaused = false;
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
        }
        _pausePanel.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0.0f : 1.0f;

    }
}
