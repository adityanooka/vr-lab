using UnityEngine;
using Unity.Netcode.Components;

public class OrbitMotion : MonoBehaviour
{
    public Transform targetToOrbit;
    public float orbitSpeed = 20.0f;
    public float orbitRadius = 5.0f;

    private Vector3 initialOffset;

    void Start()
    {
        if (targetToOrbit != null)
        {
            initialOffset = transform.position - targetToOrbit.position;
            initialOffset = initialOffset.normalized * orbitRadius;
        }
    }

    void Update()
    {
        if (targetToOrbit != null)
        {
            // Calculate new position based on rotation and radius
            float angle = Time.time * orbitSpeed;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);

            Vector3 newPosition = targetToOrbit.position + rotation * initialOffset;

            // 3. Face the direction of movement (Rotation towards orientation)
            if (newPosition != transform.position)
            {
                // Smoothly interpolate rotation to prevent sudden snapping
                Quaternion targetRotation = Quaternion.LookRotation(newPosition - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // Set position last
            transform.position = newPosition;
        }
    }
}

