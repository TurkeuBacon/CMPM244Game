using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerGroundCheck playerGroundCheck;
    [SerializeField] private float jumpHeight, gravity, speed, cameraSensitivity, verticalRange;

    private bool jump;
    private Vector3 moveDir;

    void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        jump = false;
    }

    void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            jump = true;
        }
        moveDir = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) {
            moveDir += Vector3.forward;
        }
        if (Keyboard.current.sKey.isPressed) {
            moveDir += Vector3.back;
        }
        if (Keyboard.current.aKey.isPressed) {
            moveDir += Vector3.left;
        }
        if (Keyboard.current.dKey.isPressed) {
            moveDir += Vector3.right;
        }

        Vector2 mouseMovement = Mouse.current.delta.ReadValue();
        Vector3 rotationDelta = new Vector3(-mouseMovement.y, mouseMovement.x, 0f) * cameraSensitivity * Time.deltaTime;

        float currentPlayerRotationY = transform.localRotation.eulerAngles.y;
        currentPlayerRotationY += rotationDelta.y;
        transform.localRotation = Quaternion.Euler(Vector3.up * currentPlayerRotationY);

        float currentCameraRotationX = playerCamera.transform.localRotation.eulerAngles.x;
        if (currentCameraRotationX >= 180) currentCameraRotationX -= 360;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX + rotationDelta.x, -verticalRange / 2, verticalRange / 2);
        playerCamera.transform.localRotation = Quaternion.Euler(Vector3.right * currentCameraRotationX);
    }

    void FixedUpdate() {
        if(!playerGroundCheck.grounded) {
            rb.linearVelocity += Vector3.down * gravity * Time.fixedDeltaTime;
        } else {
            if(jump) {
                rb.linearVelocity += Vector3.up * Mathf.Sqrt(2 * gravity * jumpHeight);
            }
        }
        Vector3 moveVelocity = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f) * moveDir * speed;
        rb.linearVelocity = moveVelocity + Vector3.up * rb.linearVelocity.y;
        //Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f) * inputDirection
        jump = false;
    }
}
