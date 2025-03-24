using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private bool _isPaused;

    private void Start()
    {
        _isPaused = false;
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            _isPaused = !_isPaused;
            _pausePanel.SetActive(_isPaused);
            Time.timeScale = _isPaused ? 0.0f : 1.0f;
        }
        
    }
}
