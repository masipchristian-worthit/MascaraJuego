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
            InputManager.Instance.SwitchTo(InputManager.InputMapType.Door);
        }
        else
        {
            Debug.LogWarning("El Animator no está asignado en el Inspector.");
        }
    }

    public void comeFromDialogue()
    {
        EventSystem.current.SetSelectedGameObject(_firstButton);
        
    }

    private void closeDoor()
    {
        _animator.Play("Open");
        InputManager.Instance.SwitchTo(InputManager.InputMapType.Gameplay);
    }
}