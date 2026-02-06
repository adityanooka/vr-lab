using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Automatically assigns the XR Interaction Manager to all XR Interactors in this prefab.
/// This solves the issue where prefabs cannot directly reference scene objects.
/// </summary>
public class XRInteractionManagerSetup : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The XR Interaction Manager to assign to interactors. If null, will auto-search the scene.")]
    private XRInteractionManager interactionManager;

    [SerializeField]
    [Tooltip("Debug info - shows what manager was found")]
    private string debugInfo = "Not initialized";

    private void Awake()
    {
        debugInfo = "Awake called - initializing...";
        Debug.Log("[XRInteractionManagerSetup] Awake() called on " + gameObject.name, this);
    }

    private void Start()
    {
        // Ensure initialization happens even if OnEnable doesn't fire
        SetupInteractionManager();
    }

    private void OnEnable()
    {
        // Also try in OnEnable
        SetupInteractionManager();
    }

    private void SetupInteractionManager()
    {
        // Auto-find the XR Interaction Manager if not already assigned
        if (interactionManager == null)
        {
            // Search for XR Interaction Manager in scene, excluding this prefab instance
            XRInteractionManager[] managers = FindObjectsOfType<XRInteractionManager>();
            
            foreach (var manager in managers)
            {
                // Skip if the manager is inside this prefab hierarchy
                if (!manager.transform.IsChildOf(transform) && manager.gameObject.scene.isLoaded)
                {
                    interactionManager = manager;
                    break;
                }
            }
            
            if (interactionManager == null)
            {
                debugInfo = "❌ ERROR: No XR Interaction Manager found in scene!";
                Debug.LogError($"[XRInteractionManagerSetup] {debugInfo}", this);
                return;
            }
        }

        // Find all XR Interactors in this prefab and assign the manager
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] interactors = GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();

        if (interactors.Length == 0)
        {
            debugInfo = "⚠️ WARNING: No XR Interactors found in this prefab";
            Debug.LogWarning($"[XRInteractionManagerSetup] {debugInfo}", this);
            return;
        }

        foreach (var interactor in interactors)
        {
            interactor.interactionManager = interactionManager;
            Debug.Log($"✓ Connected '{interactor.gameObject.name}' to XR Interaction Manager '{interactionManager.gameObject.name}'", interactor);
        }

        debugInfo = $"✅ Connected {interactors.Length} interactor(s) to '{interactionManager.gameObject.name}'";
        Debug.Log($"[XRInteractionManagerSetup] {debugInfo}", this);
    }
}
