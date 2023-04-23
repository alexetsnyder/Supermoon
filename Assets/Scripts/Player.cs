using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player State")]
    public bool isGrounded;
    public bool isSprinting;
    public bool readyToJump;

    [Header("Movment Settings")]
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpForce;
    public float gravity;

    [Header("Controlls")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    private Transform playerCamera;

    private float verticalInput;
    private float horizontalInput;

    private Vector3 velocity;
    private float verticalMomentum;

    private float mouseX;
    private float mouseY;

    private float xRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerCamera = GameObject.Find("PlayerCamera").transform;

        velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void Update()
    {
        GetPlayerInput();
        MouseRotation();
    }

    private void FixedUpdate()
    {     
        PlayerMovement();
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
        xRotation += -mouseY;
        if (xRotation < -90.0f || xRotation > 90.0f)
        {
            xRotation += mouseY;
            mouseY = 0.0f;
        }

        transform.Rotate(Vector3.up * mouseX);
        playerCamera.Rotate(Vector3.right * -mouseY);
    }

    private void PlayerMovement()
    {
        if (verticalMomentum > gravity)
        {
            verticalMomentum += Time.fixedDeltaTime * gravity;
        }

        float moveSpeed;
        if (isSprinting)
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        velocity = moveSpeed * Time.fixedDeltaTime * ((transform.forward * verticalInput) + (transform.right * horizontalInput));

        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        transform.Translate(velocity, Space.World);
    }
}
