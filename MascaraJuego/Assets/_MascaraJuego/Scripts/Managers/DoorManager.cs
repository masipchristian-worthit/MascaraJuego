using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance;

    [Header("Checks")] public bool isAtDoor = false;
    
    public DoorInteraction currentDoor;
    private InputAction _inputAction;
    private void Start()
    {
        _inputAction = InputManager.Instance.Door.EscapeDoor;
        _inputAction.performed += ctx => closeDoor(); 
    }

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
        Debug.Log("MANGEL TE ODIIOOOOOOOOO");
        currentDoor.closeDoor();
        isAtDoor = false;
        currentDoor = null;
    }
    
}
