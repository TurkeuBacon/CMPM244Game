using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour {
    public bool grounded;

    private void Start() {
        grounded = false;
    }

    private void OnTriggerEnter(Collider other) {
        grounded = true;
    }
    private void OnTriggerExit(Collider other) {
        grounded = false;
    }
}
