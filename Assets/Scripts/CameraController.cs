using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;  // Vitesse de déplacement
    [SerializeField] private float zoomSpeed = 5f;  // Vitesse du zoom
    [SerializeField] private float minZoom = 3f;    // Zoom minimum
    [SerializeField] private float maxZoom = 15f;   // Zoom maximum

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // Flèches gauche/droite ou Q/D
        float moveY = Input.GetAxis("Vertical");   // Flèches haut/bas ou Z/S

        Vector3 move = new Vector3(moveX, moveY, 0) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // Molette de la souris

        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
