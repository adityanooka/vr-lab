using UnityEngine;

public class GlowTrigger : MonoBehaviour
{
    public Color glowColor = Color.yellow;
    private Material targetMat;
    private Color originalColor;
    private Renderer rend;

    void Start()
    {
        // SEARCH INSIDE CHILDREN instead of just the root
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            targetMat = rend.material;
            targetMat.EnableKeyword("_EMISSION");
            if (targetMat.HasProperty("_EmissionColor"))
                originalColor = targetMat.GetColor("_EmissionColor");
        }
        else
        {
            Debug.LogError("GlowTrigger: Still no Renderer found! Check if the Trap has a mesh inside it.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLLISION DETECTED with: " + other.name); // Check Console for this!

        // Check if it's the player (look for the Steering script)
        if (other.GetComponentInParent<Steering>() != null)
        {
            Debug.Log("PLAYER ENTERED TRAP ZONE!");
            if (targetMat != null)
                targetMat.SetColor("_EmissionColor", glowColor * 5.0f); // Intensity 5
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Steering>() != null)
        {
            if (targetMat != null)
                targetMat.SetColor("_EmissionColor", originalColor);
        }
    }
}