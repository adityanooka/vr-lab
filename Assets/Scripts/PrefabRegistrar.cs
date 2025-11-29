using Unity.Netcode;
using UnityEngine;

public class PrefabRegistrar : MonoBehaviour
{
    public NetworkObject playerPrefab;

    void Start()
    {
        if (NetworkManager.Singleton != null && playerPrefab != null)
        {
            // Registers the prefab with the network system at startup
            NetworkManager.Singleton.AddNetworkPrefab(playerPrefab.gameObject);
        }
    }
}