using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractorSetup : MonoBehaviour
{
    private void Start()
    {
        XRInteractionManager manager = FindObjectOfType<XRInteractionManager>();
        
        if (manager == null)
        {
            Debug.LogError("XRInteractionManager not found in scene!");
            return;
        }
        
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor[] directInteractors = GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor[] rayInteractors = GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        
        foreach (var interactor in directInteractors)
            interactor.interactionManager = manager;
            
        foreach (var interactor in rayInteractors)
            interactor.interactionManager = manager;
    }
}