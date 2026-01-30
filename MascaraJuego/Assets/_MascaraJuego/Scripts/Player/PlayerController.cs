using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
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
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = Vector2.zero;
        if (InteractColliderSide) InteractColliderSide.enabled = false;
    }

    void Start()
    {
        // Si no asignaste el SpriteRenderer en el inspector, lo buscamos
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    void Update()
    {
        
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
        
        // Seleccionamos qué collider activar según la dirección actual
        BoxCollider currentCollider = InteractColliderSide; // Default

        if (currentCollider != null)
        {
            currentCollider.enabled = true;
            // Debug.Log($"Interacting direction: {currentDirection}");
            yield return new WaitForSeconds(0.5f);
            currentCollider.enabled = false;
        }
        else
        {
            // Fallback por si falta asignar algo
            yield return new WaitForSeconds(0.5f);
        }

        isInteracting = false;
    }

    // Player Actions Logic
    void InteractAction()
    {
        if (isInteracting) moveSpeed = 0f;
    }

    //-------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------

    #region Input System Callbacks

    // Gameplay Actions
    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Debug.Log("Interact Pressed");
            StartCoroutine(Interact());
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
            Debug.Log("Pause Pressed");
        }
    }

    // UI Actions

    public void onAccept(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            /*
            // Si hay un diálogo activo, avanzar texto
            if (DialogueManager.Instance != null && DialogueManager.Instance.gameObject.activeInHierarchy)
            {
                DialogueManager.Instance.DisplayNextSentence();
            }
            */
        }
    }

    public void onNavigate(InputAction.CallbackContext context)
    {
        // Vector2 navigationInput = context.ReadValue<Vector2>();
    }

    public void onEscape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {  
            if (pauseMenuPanel != null && pauseMenuPanel.activeInHierarchy)
            {
                TogglePause();
            }
        }
    }

    #endregion

    // Lógica de Pausa Manual
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; 
            if(pauseMenuPanel) pauseMenuPanel.SetActive(true);
            GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        }
        else
        {
            Time.timeScale = 1f; 
            if(pauseMenuPanel) pauseMenuPanel.SetActive(false);
            GetComponent<PlayerInput>().SwitchCurrentActionMap("Gameplay");
        }
    }
}
        
