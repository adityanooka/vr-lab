using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CollaborativeLift : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;
    public NetworkVariable<bool> isBeingLifted = new NetworkVariable<bool>(false);

    void Awake() => grabInteractable = GetComponent<XRGrabInteractable>();

    void Update()
    {
        if (IsServer)
        {
            // Check if two or more interactors (hands) are selecting the plate
            // In a multi-user setup, we verify if they belong to different clients
            isBeingLifted.Value = grabInteractable.interactorsSelecting.Count >= 2;
        }

        // Lock/Unlock the physics based on the lift status
        GetComponent<Rigidbody>().isKinematic = !isBeingLifted.Value;
    }
}