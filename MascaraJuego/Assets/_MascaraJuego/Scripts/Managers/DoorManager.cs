using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance;

    [Header("Checks")] public bool isAtDoor = false;
    
    public DoorInteraction currentDoor;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        DontDestroyOnLoad(this.gameObject);
    }

    public void openDoor()
    {
        isAtDoor = true;
    }

    public void closeDoor()
    {
        isAtDoor = false;
        DoorManager.Instance.currentDoor = null;
    }
    
}
