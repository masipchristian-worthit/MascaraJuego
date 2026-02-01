using UnityEngine;
using UnityEngine.InputSystem; // 1. IMPORTANTE: Necesario para usar 'InputAction'

public class beginningNote : MonoBehaviour
{
    [SerializeField] private Animator noteAnimator;

    // 2. CORRECCIÓN: Cambiamos 'PlayerInputActions' por 'InputAction'
    // La variable debe ser del tipo "Acción", no del tipo "Clase de Inputs Completa".
    private InputAction _escapeAction; 

    private void Start() 
    {
        // Ahora esto funcionará porque ambos lados son del tipo 'InputAction'
        _escapeAction = InputManager.Instance.UI.EscapeUI;
        
        // Ahora 'performed' existirá porque es una propiedad de 'InputAction'
        _escapeAction.performed += ctx => closeNote();
    }

    private void closeNote()
    {
        noteAnimator.SetTrigger("Close");
    }
    
    // Opcional: Es buena práctica desuscribirse al destruir el objeto para evitar errores de memoria
    private void OnDestroy()
    {
        if (_escapeAction != null)
        {
            _escapeAction.performed -= ctx => closeNote();
        }
    }
}