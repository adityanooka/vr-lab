using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Steering : NetworkBehaviour
{
    [Header("Scuba Settings")]
    public float swimSpeed = 4.0f;
    public float rotationSpeed = 100.0f;

    [Header("VR Input")]
    [SerializeField] private InputActionReference moveAction; // Left Stick
    [SerializeField] private InputActionReference turnAction; // Right Stick

    [Header("Components")]
    [SerializeField] private Transform headTransform; // Drag Main Camera here

    // Desktop Mouse Look helpers
    private float mousePitch = 0f;
    private float mouseYaw = 0f;

    void Update()
    {
        if (!IsOwner) return;

        HandleScubaMovement();
    }

    private void HandleScubaMovement()
    {
        Vector2 inputMove = Vector2.zero;
        float inputTurn = 0f;

        // ----------------------
        // 1. VR INPUT (Primary)
        // ----------------------
        if (moveAction != null && moveAction.action != null)
            inputMove = moveAction.action.ReadValue<Vector2>();

        if (turnAction != null && turnAction.action != null)
            inputTurn = turnAction.action.ReadValue<Vector2>().x;

        // ----------------------
        // 2. DESKTOP DEBUG (Secondary)
        // ----------------------
        if (Keyboard.current != null)
        {
            // WASD adds to the movement vector
            if (Keyboard.current.wKey.isPressed) inputMove.y += 1;
            if (Keyboard.current.sKey.isPressed) inputMove.y -= 1;
            if (Keyboard.current.aKey.isPressed) inputMove.x -= 1;
            if (Keyboard.current.dKey.isPressed) inputMove.x += 1;

            // Mouse Look (Hold Right Click to look around like a VR headset)
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                mouseYaw += mouseDelta.x * 0.1f;
                mousePitch -= mouseDelta.y * 0.1f;
                mousePitch = Mathf.Clamp(mousePitch, -89f, 89f);

                // On desktop, we rotate the camera manually since there is no headset
                if (headTransform != null)
                    headTransform.localRotation = Quaternion.Euler(mousePitch, 0, 0);

                // Use mouse X for body turning
                inputTurn += mouseDelta.x * 0.1f;
            }
        }

        // ----------------------
        // 3. APPLY PHYSICS
        // ----------------------

        // A. TURN Body (Left/Right)
        transform.Rotate(0, inputTurn * rotationSpeed * Time.deltaTime, 0);

        // B. SWIM (Move in the direction the CAMERA is looking)
        if (headTransform != null)
        {
            // Get the direction the head is pointing
            Vector3 swimDirection = headTransform.forward * inputMove.y + headTransform.right * inputMove.x;

            // Move there
            transform.position += swimDirection * swimSpeed * Time.deltaTime;
        }
    }
}