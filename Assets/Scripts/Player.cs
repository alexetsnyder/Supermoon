using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player State")]
    public bool isGrounded;
    public bool isSprinting;
    public bool readyToJump;

    [Header("Mouse Settings")]
    public float xSens;
    public float ySens;

    [Header("Movment Settings")]
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpForce;

    [Header("Controlls")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    private float verticalInput;
    private float horizontalInput;

    private float mouseX;
    private float mouseY;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GetPlayerInput();
    }

    private void FixedUpdate()
    {
        MouseRotation();
    }

    private void GetPlayerInput()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKey(jumpKey))
        {

        }

        if (Input.GetKeyDown(sprintKey))
        {
            isSprinting = true;
        }

        if (Input.GetKeyUp(sprintKey))
        {
            isSprinting = false;
        }
    }

    private void MouseRotation()
    {
        xRotation -= mouseY * Time.fixedDeltaTime * ySens;
        yRotation += mouseX * Time.fixedDeltaTime * xSens;

        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
    }

    private void PlayerMovement()
    {

    }
}
