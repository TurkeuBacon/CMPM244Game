using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerGroundCheck playerGroundCheck;
    [SerializeField] private float jumpHeight, gravity, speed;

    private bool jump;
    private Vector3 moveDir;

    void Start() {
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
