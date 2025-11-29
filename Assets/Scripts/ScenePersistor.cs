using UnityEngine;

public class ScenePersistor : MonoBehaviour
{
    void Awake()
    {
        // Forces this GameObject to survive any scene loading event
        DontDestroyOnLoad(this.gameObject);
    }
}