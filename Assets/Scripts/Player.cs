using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player State")]
    public bool isGrounded;
    public bool isSprinting;
    public bool readyToJump;

    [Header("Movment Settings")]
    public float playerRadius;
    public float playerHeight;
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpForce;
    public float jumpCooldown;
    public float gravity;

    [Header("Controlls")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    private Transform playerCamera;
    private World world;

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
        world = GameObject.Find("World").GetComponent<World>();

        velocity = new Vector3(0.0f, 0.0f, 0.0f);

        readyToJump = true;
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

        if (readyToJump && isGrounded && Input.GetKey(jumpKey))
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
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

    private void Jump()
    {
        isGrounded = false;
        verticalMomentum = jumpForce;   
    }

    private void ResetJump()
    {
        readyToJump = true;
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
        CalculateVelocity();

        CheckForCollision();

        transform.Translate(velocity, Space.World);
    }

    private void CalculateVelocity()
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
        velocity += Time.fixedDeltaTime * verticalMomentum * Vector3.up;
    }

    private void CheckForCollision()
    {
        if (velocity.y < 0.0f)
        {
            if (DownCollision(velocity.y))
            {
                isGrounded = true;
                velocity.y = 0.0f;
            }
            else
            {
                isGrounded = false;
            }
        }
        else if (velocity.y > 0.0f && UpCollision(velocity.y))
        {
            velocity.y = 0.0f; 
        }

        if ((velocity.z < 0 && BackCollision()) || 
            (velocity.z > 0.0f && FrontCollision()))
        {
            velocity.z = 0.0f;
        }

        if ((velocity.x < 0.0f && LeftCollision()) || 
            (velocity.x > 0.0f && RightCollision()))
        {
            velocity.x = 0.0f;
        }
    }

    private bool DownCollision(float speed)
    {
        Vector3 position = transform.position;
        return (
                world.HasSolidVoxel(new Vector3(position.x - playerRadius, position.y + speed, position.z - playerRadius)) ||
                world.HasSolidVoxel(new Vector3(position.x - playerRadius, position.y + speed, position.z + playerRadius)) ||
                world.HasSolidVoxel(new Vector3(position.x + playerRadius, position.y + speed, position.z + playerRadius)) ||
                world.HasSolidVoxel(new Vector3(position.x + playerRadius, position.y + speed, position.z - playerRadius)) 
               );
    }

    private bool UpCollision(float speed)
    {
        Vector3 position = transform.position;
        return (
                world.HasSolidVoxel(new Vector3(position.x - playerRadius, position.y + playerHeight + speed, position.z - playerRadius)) ||
                world.HasSolidVoxel(new Vector3(position.x - playerRadius, position.y + playerHeight + speed, position.z + playerRadius)) ||
                world.HasSolidVoxel(new Vector3(position.x + playerRadius, position.y + playerHeight + speed, position.z + playerRadius)) ||
                world.HasSolidVoxel(new Vector3(position.x + playerRadius, position.y + playerHeight + speed, position.z - playerRadius))
               );
    }

    private bool FrontCollision()
    {
        Vector3 position = transform.position;
        return (
                world.HasSolidVoxel(new Vector3(position.x, position.y, position.z + playerRadius)) ||
                world.HasSolidVoxel(new Vector3(position.x, position.y + 1.0f, position.z + playerRadius)) 
               );
    }

    private bool BackCollision()
    {
        Vector3 position = transform.position;
        return (
                world.HasSolidVoxel(new Vector3(position.x, position.y, position.z - playerRadius)) ||
                world.HasSolidVoxel(new Vector3(position.x, position.y + 1.0f, position.z - playerRadius))
               );
    }

    private bool LeftCollision()
    {
        Vector3 position = transform.position;
        return (
                world.HasSolidVoxel(new Vector3(position.x - playerRadius, position.y, position.z)) ||
                world.HasSolidVoxel(new Vector3(position.x - playerRadius, position.y + 1.0f, position.z))
               );
    }

    private bool RightCollision()
    {
        Vector3 position = transform.position;
        return (
                world.HasSolidVoxel(new Vector3(position.x + playerRadius, position.y, position.z)) ||
                world.HasSolidVoxel(new Vector3(position.x + playerRadius, position.y + 1.0f, position.z))
               );
    }
}
