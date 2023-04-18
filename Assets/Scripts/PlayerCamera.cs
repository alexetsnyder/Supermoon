using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MouseRotation();
    }

    private void MouseRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        yRotation += mouseX;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        orientation.rotation = Quaternion.Euler(0.0f, yRotation, 0.0f);
    }
}
