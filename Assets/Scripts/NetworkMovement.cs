using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Netcode.Components;

public class NetworkMovement : NetworkBehaviour
{
    public float speed = 5.0f;

    void Update()
    {
        // CRITICAL: Only run movement code if this is MY player (Host/Client)
        if (!IsOwner) return;

        Vector3 moveDirection = Vector3.zero;

        if (Keyboard.current != null)
        {
            // Reliable WASD polling fix
            if (Keyboard.current.wKey.isPressed) moveDirection.z = +1;
            if (Keyboard.current.sKey.isPressed) moveDirection.z = -1;
            if (Keyboard.current.aKey.isPressed) moveDirection.x = -1;
            if (Keyboard.current.dKey.isPressed) moveDirection.x = +1;
        }

        Vector3 move = transform.right * moveDirection.x + transform.forward * moveDirection.z;
        transform.position += move * speed * Time.deltaTime;
    }
}