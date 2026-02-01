using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Referencias")]
    
    [SerializeField] private Animator _animator; 
    [SerializeField] private GameObject _menuCanvas; 
    public void OpenDoorAndShowUI()
    {
        if (_menuCanvas != null)
        {
            _menuCanvas.SetActive(true);
        }

        if (_animator != null)
        {
            _animator.SetTrigger("OpenDoor");
        }
        else
        {
            Debug.LogWarning("El Animator no está asignado en el Inspector.");
        }
    }
}