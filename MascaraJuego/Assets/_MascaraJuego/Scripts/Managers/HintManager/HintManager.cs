using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public List<HintSO> hints;
    [SerializeField] private HintButton _hintButton;
    [SerializeField] private Transform _hintsParent;
    [SerializeField] private HintUI _hintMenu;
    [SerializeField] private HintUI _hintDiscover;
    public static HintManager Instance;
    private PlayerInputActions _playerInputActions;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void registerHint(HintSO hint)
    {
        hints.Add(hint);
        _hintDiscover.SetHintDiscover(hint);
    }

    public void ShowHintsInMenu()
{
    var _hintParentCanvas = _hintsParent.GetComponent<CanvasGroup>().alpha = 1;

    // Eliminar los hints previos
    for (int i = 0; i < _hintsParent.childCount; i++)
    {
        Destroy(_hintsParent.GetChild(i).gameObject);
        Debug.Log("Eliminando hint");
    }

    // Crear los nuevos hint buttons
    List<HintButton> hintButtons = new List<HintButton>(); // Para almacenar referencias a los botones instanciados
    for (int i = 0; i < hints.Count; i++)
    {
        HintButton hintButtonPrefab = Instantiate(_hintButton, _hintsParent);
        hintButtonPrefab.GetComponent<HintButton>().SetTextButton(hints[i]);
        hintButtons.Add(hintButtonPrefab);  // Guardamos las referencias
        Debug.Log("Añadiendo hint ");
    }

    // Configurar la navegación entre los botones
    for (int i = 0; i < hintButtons.Count; i++)
    {
        Button button = hintButtons[i].GetComponent<Button>();
        Navigation navigation = button.navigation;  // Crea una referencia a la variable Navigation

        // Configurar la navegación hacia abajo
        if (i < hintButtons.Count - 1)
        {
            // Si no es el último botón, navegamos hacia abajo al siguiente botón
            navigation.selectOnUp = hintButtons[i + 1].GetComponent<Button>();  // El siguiente botón
            navigation.selectOnDown = hintButtons[i + 1].GetComponent<Button>();  // El siguiente botón
        }
        else
        {
            // Si es el último botón, no hay un botón hacia abajo (se queda en el último)
            navigation.selectOnUp = hintButtons[i - 1].GetComponent<Button>(); // El anterior
            navigation.selectOnDown = button;  // Se queda en el mismo
        }

        // Configuración hacia arriba (para todos los botones)
        if (i > 0)
        {
            navigation.selectOnUp = hintButtons[i - 1].GetComponent<Button>();  // El anterior
        }

        // Si es el primer botón, no navega hacia arriba, se queda en el mismo
        if (i == 0)
        {
            navigation.selectOnUp = button;
        }

        // Asignamos la configuración de navegación de vuelta al botón
        button.navigation = navigation;
    }

    // Seleccionar el primer botón para comenzar la navegación
    if (_hintsParent.childCount > 0)
    {
        EventSystem.current.SetSelectedGameObject(_hintsParent.GetChild(0).gameObject);
        Debug.Log("SELECCIONANDO EL PRIMER HINT");
    }
}




    public void HideHintMenu()
    {
        _hintMenu.CloseHint();
        _hintsParent.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void ShowHintInMenu(HintSO hint)
    {
        _hintMenu.SetHint(hint);
    }

    public void ShowHintDialogue(HintSO hint)
    {
        _hintDiscover.SetHint(hint);
        InputManager.Instance.SwitchTo(InputManager.InputMapType.Hint);
        
    }
}
