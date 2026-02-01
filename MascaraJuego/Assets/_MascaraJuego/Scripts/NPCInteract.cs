using UnityEngine;
using UnityEngine.InputSystem; 
using DS.ScriptableObjects;    

public class NPCInteract : MonoBehaviour
{
    [Header("Diálogos")]
    [SerializeField] DSDialogueContainerSO DSDialogue; 
    [SerializeField] GameObject eKeyIcon;

    // IMPORTANTE: Asegúrate de inicializar estas acciones en el Start o desde el Inspector
    // si no, te dará error de "NullReference" al jugar.
    private InputAction moveGameplayAction;
    private InputAction interactGameplayAction; 
    private InputAction pauseGameplayAction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerInteract")) 
        {
            eKeyIcon.SetActive(true); 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // CORRECCIÓN AQUÍ: Añadimos () al final de WasPressedThisFrame
        if (other.CompareTag("PlayerInteract") && 
            interactGameplayAction != null && 
            interactGameplayAction.WasPressedThisFrame()) 
        {
            Debug.Log("Iniciando Diálogo...");
            // Aquí llamarías a tu corrutina cuando la crees:
            // StartCoroutine(DSDialogueCoroutine());
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("PlayerInteract")) 
        {
            eKeyIcon.SetActive(false);
        }
    }
}