using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    
    void Awake()
    {
        // Keep the first instance
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        // Self destruct any duplicate instances
        Destroy(gameObject);
    }
}
