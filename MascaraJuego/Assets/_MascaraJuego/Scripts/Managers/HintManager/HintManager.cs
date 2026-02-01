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
        _hintsParent.GetComponent<CanvasGroup>().alpha = 1;

        // Eliminar los hints previos
        for (int i = 0; i < _hintsParent.childCount; i++)
        {
            Destroy(_hintsParent.GetChild(i).gameObject);
            Debug.Log("Eliminando hint");
        }

        // Crear los nuevos botones de hint
        List<HintButton> hintButtons = new List<HintButton>();
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
        
            Navigation navigation = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = (i > 0) ? hintButtons[i - 1].GetComponent<Button>() : button, // El anterior o el mismo si es el primer botón
                selectOnDown = (i < hintButtons.Count - 1) ? hintButtons[i + 1].GetComponent<Button>() : button  // El siguiente o el mismo si es el último botón
            };

            button.navigation = navigation;  // Asignamos la navegación al botón
        }

        // Seleccionar el primer botón al mostrar el menú de hints
        if (_hintsParent.childCount > 0)
        {
            EventSystem.current.SetSelectedGameObject(_hintsParent.GetChild(0).gameObject);
            Debug.Log("SELECCIONANDO EL PRIMER HINT");
        }
        else
        {
            Debug.Log("No se encontraron botones de hint.");
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
