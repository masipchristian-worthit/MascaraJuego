using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions controls; // Aquí usamos tu clase real
    private Stack<InputActionMap> mapHistory = new Stack<InputActionMap>();

    // Definimos los tipos para evitar errores de escritura (magic strings)
    public enum InputMapType
    {
        Gameplay,
        UI,
        Dialogue,
        Hint
    }

    [Header("Debug Info")]
    [SerializeField] private string currentMapName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Inicializamos TU clase generada
            controls = new PlayerInputActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Iniciamos activando el Gameplay
        SwitchTo(InputMapType.Gameplay);
    }

    // --- API PÚBLICA (Lo que usarás desde otros scripts) ---

    public void SwitchTo(InputMapType type)
    {
        // Convertimos el Enum al Mapa real
        InputActionMap mapToEnable = GetMapFromEnum(type);

        if (mapToEnable == null)
        {
            Debug.LogError($"El mapa {type} no existe en PlayerInputActions.");
            return;
        }

        // Lógica de la Pila (Stack)
        if (mapHistory.Count > 0)
        {
            var activeMap = mapHistory.Peek();
            activeMap.Disable();
            UnsubscribeToken(activeMap);
        }

        mapHistory.Push(mapToEnable);
        
        mapToEnable.Enable();
        SubscribeToken(mapToEnable);

        currentMapName = mapToEnable.name;
        Debug.Log($"[InputManager] Cambiado a: {currentMapName}");
    }

    public void ReturnToPreviousMap()
    {
        if (mapHistory.Count <= 1) return; // No regresamos si es el último (Gameplay)

        // 1. Apagar actual
        var currentMap = mapHistory.Pop();
        currentMap.Disable();
        UnsubscribeToken(currentMap);

        // 2. Encender anterior
        var previousMap = mapHistory.Peek();
        previousMap.Enable();
        SubscribeToken(previousMap);

        currentMapName = previousMap.name;
        Debug.Log($"[InputManager] Regresado a: {currentMapName}");
    }

    // --- LÓGICA INTERNA ---

    private InputActionMap GetMapFromEnum(InputMapType type)
    {
        // Asegúrate de que los nombres aquí coinciden EXACTAMENTE con tu .inputactions
        switch (type)
        {
            case InputMapType.Gameplay: return controls.Gameplay;
            case InputMapType.UI:       return controls.UI;
            case InputMapType.Dialogue: return controls.Dialogue;
            case InputMapType.Hint:     return controls.Hint;
            default: return null;
        }
    }

    private void SubscribeToken(InputActionMap map)
    {
        if (map.name == "Gameplay") return; 

        var cancelAction = map.FindAction("Cancel");
        if (cancelAction != null)
            cancelAction.performed += OnCancelPerformed;
    }

    private void UnsubscribeToken(InputActionMap map)
    {
        var cancelAction = map.FindAction("Cancel");
        if (cancelAction != null)
            cancelAction.performed -= OnCancelPerformed;
    }

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        ReturnToPreviousMap();
    }
    
    // Getter por si necesitas acceder a una acción específica desde fuera (ej. Moverse)
    public PlayerInputActions GetControls()
    {
        return controls;
    }
}