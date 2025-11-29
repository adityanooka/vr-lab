using UnityEngine;

public class SimpleFloat : MonoBehaviour
{
    public float floatSpeed = 1.0f;
    public float height = 0.5f;
    public float rotateSpeed = 15f;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Position Logic (Vertical Bobbing - time-dependent function)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * height;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Rotation Logic (Tumbling)
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}