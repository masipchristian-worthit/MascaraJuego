using UnityEngine;
using UnityEngine.Events;
using System.Collections; 

public class Interactable : MonoBehaviour
{
    [Header("Interaction Events")]
    [SerializeField] private UnityEvent OnInteracted;
    [SerializeField] private Animator animator;

    // Usamos Start para asegurarnos de que el Player (Awake) ya se inicializó
    void Start()
    {
        if (animator == null && PlayerController.Instance != null)
        {
            animator = PlayerController.Instance.GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerInteract"))
        {
            StartCoroutine(PerformInteraction());
        }
    }

    private IEnumerator PerformInteraction()
    {
        if(animator != null) 
        {
            animator.SetTrigger("Kick");
            yield return new WaitForEndOfFrame();
            
            float tiempoAnimacion = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(tiempoAnimacion);
        }
        OnInteracted?.Invoke();
    }
}