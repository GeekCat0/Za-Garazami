using UnityEngine;
using UnityEngine.InputSystem;
public class CameraMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float edgeSize = 10f;

    [SerializeField] float minX = 0f;
    [SerializeField] float maxX = 15f;

    void Update()
    {
        if (Mouse.current == null)
            return;
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 currentPosition = transform.position;
        if (mousePosition.x <= edgeSize)
        {
            currentPosition.x -= moveSpeed * Time.deltaTime;
        }
        else if (mousePosition.x >= Screen.width - edgeSize)
        {
            currentPosition.x += moveSpeed * Time.deltaTime;
        }
        currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
        transform.position = currentPosition;
    }
}
