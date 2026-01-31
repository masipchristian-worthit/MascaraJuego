using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // --- ESTA ES LA PARTE QUE TE FALTABA (SINGLETON) ---
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
    
    void Awake()
    {
        // --- CONFIGURACIÓN DEL SINGLETON ---
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
    }

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isPaused && !isInteracting)
        {
            Movement();
        }
    }

    void Movement()
    {
        Vector3 displacement = new Vector3(movement.x, movement.y, 0f) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + displacement);
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

    // INPUT SYSTEM CALLBACKS

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

    // --------------------------------------------------------------------
    // HINT ACTIONS (Pantalla de Pistas/Notas)
    // --------------------------------------------------------------------

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

    // --------------------------------------------------------------------
    // DIALOGUE ACTIONS
    // --------------------------------------------------------------------
    public void onAcceptDialogue(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            
        }
    }

    public void onNavigateDialogue(InputAction.CallbackContext context) 
    {
        
    }   

    // --------------------------------------------------------------------
    // DOORS ACTIONS
    // --------------------------------------------------------------------
    public void onEscapeDoor(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            // Lógica para salir de la puerta
        }
    }
    #endregion

}