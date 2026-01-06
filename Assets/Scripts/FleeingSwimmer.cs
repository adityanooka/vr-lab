using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class FleeingSwimmer : NetworkBehaviour
{
    public float swimSpeed = 1f;
    public float fleeSpeed = 3f;
    public float fleeDistance = 1f;
    private Vector3 currentTarget;
    private float nextChangeTime;

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Only the server runs the AI
        {
            ChangeRandomDirection();
        }
    }

    void Update()
    {
        if (!IsServer) return; // Logic only runs on the server

        // 1. Check for nearby players to flee
        NetworkObject nearestPlayer = FindNearestPlayer();
        if (nearestPlayer != null && Vector3.Distance(transform.position, nearestPlayer.transform.position) < fleeDistance)
        {
            Flee(nearestPlayer.transform.position);
        }
        else
        {
            SwimRandomly();
        }
    }

    void SwimRandomly()
    {
        // Logic for regular random movement (Change direction every few seconds)
        if (Time.time >= nextChangeTime)
        {
            ChangeRandomDirection();
        }
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, swimSpeed * Time.deltaTime);
        transform.forward = Vector3.Lerp(transform.forward, (currentTarget - transform.position).normalized, Time.deltaTime);
    }

    void Flee(Vector3 playerPos)
    {
        // Move away from the player
        Vector3 direction = (transform.position - playerPos).normalized;
        currentTarget = transform.position + direction * 5f; // Set a new target away from player
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, fleeSpeed * Time.deltaTime);
        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);
        nextChangeTime = Time.time + 5f; // Reset random timer
    }

    void ChangeRandomDirection()
    {
        // Set a new random target in the immediate vicinity
        Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), Random.Range(-2f, 2f), Random.Range(-5f, 5f));
        currentTarget = transform.position + randomOffset;
        nextChangeTime = Time.time + Random.Range(3f, 7f);
    }

    // Needs access to all players (defined in previous steps)
    private NetworkObject FindNearestPlayer() { /* ... implementation of FindNearestPlayer ... */ return null; } 
}