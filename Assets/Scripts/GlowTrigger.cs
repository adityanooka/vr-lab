using UnityEngine;

public class GlowTrigger : MonoBehaviour
{
    public Color glowColor = Color.yellow;
    private Material targetMat;
    private Color originalColor;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            targetMat = rend.material;
            targetMat.EnableKeyword("_EMISSION");
            if (targetMat.HasProperty("_EmissionColor"))
                originalColor = targetMat.GetColor("_EmissionColor");
        }
        else { Debug.LogError("GlowTrigger: No Renderer found on " + gameObject.name); }
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