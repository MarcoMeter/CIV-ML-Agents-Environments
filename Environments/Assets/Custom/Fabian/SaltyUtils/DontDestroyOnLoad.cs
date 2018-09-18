using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
            Debug.Log("DontDestroyOnLoad: " + gameObject);
        }
    }
}