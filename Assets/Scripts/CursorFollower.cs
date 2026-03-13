using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    [SerializeField] private float zDepth = 0f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        transform.position = mainCamera.ScreenToWorldPoint(screenPos);
        transform.position = new Vector3(transform.position.x, transform.position.y, zDepth);
    }
}
