using UnityEngine;

public class StackManager : MonoBehaviour
{
    public static StackManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    
    
}
