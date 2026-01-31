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
        Cursor.visible = false;
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
    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(Interact());
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
        }
    }

    public void onAccept(InputAction.CallbackContext context) { }
    public void onNavigate(InputAction.CallbackContext context) { }
    
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

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; 
            Cursor.visible = true;
            if(pauseMenuPanel) pauseMenuPanel.SetActive(true);
            GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");

        }
        else
        {
            Time.timeScale = 1f; 
            Cursor.visible = false;
            if(pauseMenuPanel) pauseMenuPanel.SetActive(false);
            GetComponent<PlayerInput>().SwitchCurrentActionMap("Gameplay");
        }
    }
}