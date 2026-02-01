using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions controls;
    private Stack<InputActionMap> mapHistory = new Stack<InputActionMap>();

    [Header("UI Settings")]
    [SerializeField] private InputSystemUIInputModule uiInputModule;
    private InputActionReference defaultClickAction;

    // Accesos directos
    public PlayerInputActions.GameplayActions Gameplay => controls.Gameplay;
    public PlayerInputActions.UIActions UI => controls.UI;
    public PlayerInputActions.DialogueActions Dialogue => controls.Dialogue;
    public PlayerInputActions.HintActions Hint => controls.Hint;
    public PlayerInputActions.DoorActions Door => controls.Door;

    public enum InputMapType
    {
        Gameplay,
        UI,
        Dialogue,
        Hint,
        Door
    }

    [Header("Debug Info")]
    [SerializeField] private string currentMapName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            controls = new PlayerInputActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (uiInputModule != null)
        {
            defaultClickAction = uiInputModule.leftClick;
        }
        else
        {
            Debug.LogError("[InputManager] FALTA ASIGNAR EL UiInputModule EN EL INSPECTOR");
        }

        // Arranca en Gameplay (o lo que prefieras)
        SwitchTo(InputMapType.Gameplay);
    }

    private void CursorLogic()
    {
        if (uiInputModule == null) return;

        if (currentMapName == "Door")
        {
            // MODO DOOR:
            // 1. Ocultamos el ratón de Windows (porque GamepadCursor dibuja el suyo propio)
            Cursor.visible = false; 
            
            // 2. Cursor desbloqueado para que el sistema virtual pueda moverlo
            Cursor.lockState = CursorLockMode.None;

            // 3. IMPORTANTE: Restauramos el clic para que funcionen los botones
            if (defaultClickAction != null)
            {
                uiInputModule.leftClick = defaultClickAction;
            }
        }
        else
        {
            // OTROS MODOS:
            bool showCursor = (currentMapName == "UI" || currentMapName == "Hint");
            Cursor.visible = showCursor;
            Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;

            // Restauramos clic por si acaso
            if (defaultClickAction != null)
            {
                uiInputModule.leftClick = defaultClickAction;
            }
        }
    }

    public void SwitchTo(InputMapType type)
    {
        InputActionMap mapToEnable = GetMapFromEnum(type);
        if (mapToEnable == null) return;

        if (mapHistory.Count > 0)
        {
            var activeMap = mapHistory.Peek();
            activeMap.Disable();
            UnsubscribeCancel(activeMap);
        }

        mapHistory.Push(mapToEnable);
        mapToEnable.Enable();
        SubscribeCancel(mapToEnable);

        currentMapName = mapToEnable.name;
        CursorLogic(); 
        Debug.Log(currentMapName);
    }

    public void ReturnToPreviousMap()
    {
        if (mapHistory.Count <= 1) return;

        var currentMap = mapHistory.Pop();
        currentMap.Disable();
        UnsubscribeCancel(currentMap);

        var previousMap = mapHistory.Peek();
        previousMap.Enable();
        SubscribeCancel(previousMap);

        currentMapName = previousMap.name;
        CursorLogic();
    }

    private InputActionMap GetMapFromEnum(InputMapType type)
    {
        switch (type)
        {
            case InputMapType.Gameplay: return controls.Gameplay.Get();
            case InputMapType.UI:       return controls.UI.Get();
            case InputMapType.Dialogue: return controls.Dialogue.Get();
            case InputMapType.Hint:     return controls.Hint.Get();
            case InputMapType.Door:     return controls.Door.Get();
            default: return null;
        }
    }

    private void SubscribeCancel(InputActionMap map)
    {
        if (map.name == "Gameplay" || map.name == "Dialogue") return;
        var cancelAction = map.FindAction("Cancel");
        if (cancelAction != null) cancelAction.performed += OnCancelPerformed;
    }

    private void UnsubscribeCancel(InputActionMap map)
    {
        var cancelAction = map.FindAction("Cancel");
        if (cancelAction != null) cancelAction.performed -= OnCancelPerformed;
    }

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        ReturnToPreviousMap();
    }
    
    private void OnDisable()
    {
        if (controls != null) controls.Disable();
    }
}