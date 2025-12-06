using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Netcode.Components;

public class NetworkMovement : NetworkBehaviour
{
    public float speed = 5.0f;
    public float verticalSpeed = 3.0f;
    public float rotationSpeed = 100.0f;

    void Update()
    {
        // CRITICAL: Only run movement code if this is MY player (Host/Client)
        if (!IsOwner) return;

        Vector3 moveDirection = Vector3.zero;
        float rotateInput = 0f;

        if (Keyboard.current != null)
        {
            // Reliable WASD polling fix
            if (Keyboard.current.wKey.isPressed) moveDirection.z = +1;
            if (Keyboard.current.sKey.isPressed) moveDirection.z = -1;
            if (Keyboard.current.aKey.isPressed) moveDirection.x = -1;
            if (Keyboard.current.dKey.isPressed) moveDirection.x = +1;
            if (Keyboard.current.upArrowKey.isPressed) moveDirection.y = +1;
            if (Keyboard.current.downArrowKey.isPressed) moveDirection.y = -1;
            if (Keyboard.current.leftArrowKey.isPressed) rotateInput = -1f;
            if (Keyboard.current.rightArrowKey.isPressed) rotateInput = +1f;


        }
        transform.Rotate(Vector3.up, rotateInput * rotationSpeed * Time.deltaTime);
        Vector3 moveHorizontal = transform.right * moveDirection.x + transform.forward * moveDirection.z;
        Vector3 moveVertical = transform.up * moveDirection.y * verticalSpeed;

        //Vector3 move = transform.right * moveDirection.x + transform.forward * moveDirection.z;
        transform.position += (moveHorizontal * speed * Time.deltaTime) + (moveVertical * Time.deltaTime);
    }
}