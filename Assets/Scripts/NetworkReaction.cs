using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components; // Required for NetworkTransform

public class NetworkReaction : NetworkBehaviour
{
    public float reactionDistance = 5f;
    public float runSpeed = 2f;
    public GameObject objectToMove;

    private NetworkTransform targetNetworkTransform;

    public override void OnNetworkSpawn()
    {
        // CRITICAL: This script only runs its core logic if it IS the server (Host)
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        if (objectToMove != null)
        {
            targetNetworkTransform = objectToMove.GetComponent<NetworkTransform>();
        }
    }

    void Update()
    {
        if (!IsServer || targetNetworkTransform == null) return;

        NetworkObject nearestPlayer = FindNearestPlayer();

        if (nearestPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, nearestPlayer.transform.position);

            if (distance < reactionDistance)
            {
                // Calculate the direction away from the player
                Vector3 directionAway = (transform.position - nearestPlayer.transform.position).normalized;

                // Move the object (NetworkTransform syncs this automatically)
                objectToMove.transform.position += directionAway * runSpeed * Time.deltaTime;
            }
        }
    }

    private NetworkObject FindNearestPlayer()
    {
        NetworkObject closest = null;
        float minDistance = float.MaxValue;

        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (player.PlayerObject != null)
            {
                float distance = Vector3.Distance(transform.position, player.PlayerObject.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = player.PlayerObject;
                }
            }
        }
        return closest;
    }
}