using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpActon;
    [SerializeField] private InputActionReference runAction;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float runningMultiplier;

    [Header("Camera")]
    [SerializeField] private InputActionReference cameraAction;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float cameraSensitivity;

    private Rigidbody rb;

    private Vector3 inputDir;
    private Vector3 cameraDir;
    private float xRotation;
    private float yRotation;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        inputDir = new Vector3(moveAction.action.ReadValue<Vector2>().x, 0, moveAction.action.ReadValue<Vector2>().y);
        cameraDir = cameraAction.action.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        MovementSys(inputDir);
    }
    private void LateUpdate()
    {
        Rotation(cameraDir);
        //CameraMovement(cameraDir);
    }
    private void MovementSys(Vector3 dir)
    {
        Vector3 camForward = mainCamera.forward;
        Vector3 camRight = mainCamera.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 relativeMovement = (camForward * dir.z) + (camRight * dir.x);
        Vector3 targetVelocity = relativeMovement * playerSpeed;

        if(runAction.action.IsPressed())
            targetVelocity = targetVelocity * runningMultiplier;

        Vector3 currentVelocity = rb.linearVelocity;
        float currentY = currentVelocity.y;
        currentVelocity.y = 0f;

        Vector3 smoothVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 10f);
        smoothVelocity.y = currentY;
        rb.linearVelocity = smoothVelocity;
    }
    private void CameraMovement(Vector2 dir)
    {
        float mouseY = dir.y * cameraSensitivity;
        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (mainCamera != null)
        {
            mainCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
    private void Rotation(Vector2 dir)
    {
        float mouseX = dir.x * cameraSensitivity;
        yRotation += mouseX;

        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        rb.MoveRotation(targetRotation);
    }
    private void Jump(InputAction.CallbackContext obj)
    {
        if (Physics.CheckSphere(transform.position, 1f, LayerMask.GetMask("Ground")))
        {
            rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }
    }
    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpActon.action.performed += Jump;
    }
    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpActon.action.Disable();
    }
}
