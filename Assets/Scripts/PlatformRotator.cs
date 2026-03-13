using UnityEngine;

public class PlatformRotator : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeedDegreesPerSecond = 12f;
    [SerializeField] private bool rotateChildren = true;

    private void Update()
    {
        float angleStep = rotationSpeedDegreesPerSecond * Time.deltaTime;

        if (rotateChildren && transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).Rotate(0f, 0f, angleStep, Space.Self);
            }

            return;
        }

        transform.Rotate(0f, 0f, angleStep, Space.Self);
    }
}
