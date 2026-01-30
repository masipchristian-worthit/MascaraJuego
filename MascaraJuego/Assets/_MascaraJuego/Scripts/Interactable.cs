using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Events")]
    [SerializeField] private UnityEvent OnInteracted;

    private void OnTriggerEnter(Collider other)
    {
        // Lógica para cuando el jugador entra en el área de interacción
        if (other.CompareTag("PlayerInteract"))
        {
            OnInteracted?.Invoke();
        }
    }
}
