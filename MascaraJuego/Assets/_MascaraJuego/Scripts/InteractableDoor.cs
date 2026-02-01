using UnityEngine;
using UnityEngine.EventSystems;

public class DoorInteraction : MonoBehaviour
{
    [Header("Referencias")]
    
    [SerializeField] private Animator _animator; 
    [SerializeField] private GameObject _menuCanvas; 
    [SerializeField] private GameObject _firstButton;
    public void OpenDoorAndShowUI()
    {
        if (_menuCanvas != null)
        {
            _menuCanvas.SetActive(true);
        }

        if (_animator != null)
        {
            _animator.Play("Open");
            EventSystem.current.SetSelectedGameObject(_firstButton);
        }
        else
        {
            Debug.LogWarning("El Animator no está asignado en el Inspector.");
        }
    }
}