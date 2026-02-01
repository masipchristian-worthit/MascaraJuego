using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Player Settings")]
    [SerializeField] public float moveSpeed = 5f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    [Header("Interaction Colliders")]
    public BoxCollider InteractColliderSide; 

    // State Variables
    private bool isInteracting = false;
    private bool isPaused = false;

    // Private References
    Rigidbody rb;
    Vector2 movement;


    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject hintPanel;

    // -----------------------------------------------------------------------
    // REFERENCIAS A INPUT ACTIONS (Suscripción manual en Start)
    // -----------------------------------------------------------------------
    
    // Gameplay Actions
    private InputAction moveGameplayAction;
    private InputAction interactGameplayAction;
    private InputAction pauseGameplayAction;

    // UI Actions
   // private InputAction acceptUIAction;
   // private InputAction navigateUIAction;
   // private InputAction escapeUIAction; // Llamado 'CancelUI' en tu lógica anterior
//
   // // Dialogue Actions
   // private InputAction acceptDialogueAction;
   // private InputAction navigateDialogueAction;
   // private InputAction escapeDialogueAction;
//
   // // Door Actions
   // private InputAction escapeDoorAction;
   // private InputAction interactCharacterDoorAction;
   // private InputAction gamepadCursorDoorAction;
//
   // // Hint Actions
   // private InputAction acceptHintAction;
   // private InputAction navigateHintAction;
   // private InputAction escapeHintAction; // Llamado 'CancelHint' en tu lógica anterior

    private Vector3 originalScale;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; 
        }

        rb = GetComponent<Rigidbody>();
        movement = Vector2.zero;
        if (InteractColliderSide) InteractColliderSide.enabled = false;
        originalScale = transform.localScale;
    }

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (anim == null) anim = GetComponent<Animator>();

        // -----------------------------------------------------------------------
        // INICIALIZACIÓN DE REFERENCIAS (POLLING SETUP)
        // Equivalente a: action = playerInput.actions["Name"];
        // Usamos InputManager.Instance para acceso seguro y tipado.
        // -----------------------------------------------------------------------
        var inputInstance = InputManager.Instance;

        // 1. GAMEPLAY
        moveGameplayAction = inputInstance.Gameplay.MoveGameplay;
        interactGameplayAction = inputInstance.Gameplay.InteractGameplay;
        pauseGameplayAction = inputInstance.Gameplay.PauseGameplay;

        // 2. UI
       // acceptUIAction = inputInstance.UI.AcceptUI;
       // navigateUIAction = inputInstance.UI.NavigateUI;
       // escapeUIAction = inputInstance.UI.EscapeUI;
//
       // // 3. DIALOGUE
       // acceptDialogueAction = inputInstance.Dialogue.AcceptDialogue;
       // navigateDialogueAction = inputInstance.Dialogue.NavigateDialogue;
       // escapeDialogueAction = inputInstance.Dialogue.EscapeDialogue;
//
       // // 4. DOOR
       // escapeDoorAction = inputInstance.Door.EscapeDoor;
       // interactCharacterDoorAction = inputInstance.Door.InteractCharacter;
       // gamepadCursorDoorAction = inputInstance.Door.GamepadCursor;
//
       // // 5. HINT
       // acceptHintAction = inputInstance.Hint.AcceptHint;
       // navigateHintAction = inputInstance.Hint.NavigateHint;
       // escapeHintAction = inputInstance.Hint.EscapeHint;
    }

    void Update()
    {
        // La lectura de botones (Pulsación única) se gestiona mejor en Update
        HandleInputPolling();
        AnimationHandle();
    }

    void FixedUpdate()
    {
        // La lectura de valores continuos (Vectores de movimiento) se hace aquí para física
        if (moveGameplayAction != null)
        {
            // Leemos el valor directamente (Polling continuo)
            movement = moveGameplayAction.ReadValue<Vector2>();
        }
        Movement();
        // Lógica de movimiento físico
       // if (!isPaused && !isInteracting)
       // {
       //     
       // }
    }

    //ANIMATIONS
    void AnimationHandle()
    {
        bool moving = movement.sqrMagnitude != 0f;
        anim.SetBool("isMoving", moving);

        if (interactGameplayAction.WasPressedThisFrame()) 
        {
            anim.SetTrigger("Kick");
        }

        if (pauseGameplayAction.WasPressedThisFrame())
        {
            anim.SetBool("Watch", true);
        }
    }

    public void cancelWatch()
    {
        anim.SetBool("Watch", false);
    } 


    // Método centralizado para verificar pulsaciones de botones
    void HandleInputPolling()
    {
        // --- GAMEPLAY INPUTS ---
        if (interactGameplayAction.WasPressedThisFrame())
        {
            StartCoroutine(Interact());
        }

        if (pauseGameplayAction.WasPressedThisFrame())
        {
            // Lógica de pausa aquí
        }

        // --- UI INPUTS ---
        // (Nota: InputManager maneja el cambio de mapas, así que estas acciones 
        // solo devolverán true si el mapa UI está activo).
     //   if (escapeUIAction.WasPressedThisFrame())
     //   {
     //       if (pauseMenuPanel != null) 
     //       {
     //           isPaused = false;
     //           Time.timeScale = 1f;
     //           pauseMenuPanel.SetActive(false);
     //       }
     //       InputManager.Instance.ReturnToPreviousMap();
     //   }
//
     //   // --- HINT INPUTS ---
     //   if (escapeHintAction.WasPressedThisFrame())
     //   {
     //       if (hintPanel != null) hintPanel.SetActive(false);
     //       InputManager.Instance.ReturnToPreviousMap();
     //   }
//
     //   // --- DIALOGUE INPUTS ---
     //   if (acceptDialogueAction.WasPressedThisFrame())
     //   {
     //       // Lógica aceptar diálogo
     //   }
//
     //   // --- DOOR INPUTS ---
     //   if (escapeDoorAction.WasPressedThisFrame())
     //   {
     //       // Lógica salir puerta
     //   }
     //   
     //   // El cursor del gamepad se lee como vector, similar al movimiento
     //   // Vector2 cursorVal = gamepadCursorDoorAction.ReadValue<Vector2>();
    }

void Movement()
    {
        Vector3 displacement = new Vector3(movement.x, 0f, movement.y) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + displacement);

        // LÓGICA DE FLIP (Volteo)
        HandleFlip();
    }

    void HandleFlip()
    {
        // Solo actuamos si hay movimiento horizontal
        if (movement.x != 0)
        {
            // Si x > 0 (derecha), usamos la escala original positiva.
            // Si x < 0 (izquierda), invertimos la X de la escala original.
            float facingDirection = (movement.x > 0) ? 1f : -1f;

            // Aplicamos el cambio manteniendo el tamaño original en Y y Z
            transform.localScale = new Vector3(
                originalScale.x * facingDirection, 
                originalScale.y, 
                originalScale.z
            );
        }
    }
    // COROUTINES
    IEnumerator Interact()
    {
        isInteracting = true;
        BoxCollider currentCollider = InteractColliderSide; 

        if (currentCollider != null)
        {
            currentCollider.enabled = true;
            yield return new WaitForSeconds(0.5f);
            currentCollider.enabled = false;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isInteracting = false;
    }

    // ----------------------------------------------------------------------------------
    // CÓDIGO DESCARTADO (EVENT-DRIVEN CALLBACKS)
    // Se mantiene comentado como referencia
    // ----------------------------------------------------------------------------------

    /*
    #region Input Callbacks

    //GAMEPLAY ACTIONS
    public void OnMoveGameplay(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void OnInteractGameplay(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(Interact());
        }
    }

    public void OnPauseGameplay(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }
    }

    //UI ACTIONS
    public void onAcceptUI(InputAction.CallbackContext context) { }
    public void onNavigateUI(InputAction.CallbackContext context) { }

    public void onCancelUI(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (pauseMenuPanel != null) 
            {
                isPaused = false;
                Time.timeScale = 1f;
                pauseMenuPanel.SetActive(false);
            }
            InputManager.Instance.ReturnToPreviousMap();
        }
    }

    // HINT ACTIONS
    public void onAcceptHint(InputAction.CallbackContext context) { }
    public void onNavigateHint(InputAction.CallbackContext context) { }

    public void onCancelHint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (hintPanel != null) hintPanel.SetActive(false);
        
            InputManager.Instance.ReturnToPreviousMap();
        }
    }

    // DIALOGUE ACTIONS
    public void onAcceptDialogue(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            
        }
    }

    public void onNavigateDialogue(InputAction.CallbackContext context) 
    {
        
    }   

    // DOORS ACTIONS
    public void onEscapeDoor(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            // Lógica para salir de la puerta
        }
    }
    #endregion
    */

    public void TogglePause()
    {
        PauseManager.Instance.pauseGame();
    }
}