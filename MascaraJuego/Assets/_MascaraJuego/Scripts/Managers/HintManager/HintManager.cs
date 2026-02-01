using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
        for (int i = 0; i < _hintsParent.childCount; i++)
        {
            Destroy(_hintsParent.GetChild(i).gameObject);
            Debug.Log("Eliminando hint");
        }

        for (int i = 0; i < hints.Count; i++)
        {
            HintButton hintButtonPrefab = Instantiate(_hintButton, _hintsParent);
            hintButtonPrefab.GetComponent<HintButton>().SetTextButton(hints[i]);
            Debug.Log("Añadiendo hint ");
        }

        if (_hintsParent.GetChild(0).transform.GetChild(0).gameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(_hintsParent.GetChild(0).transform.GetChild(0).gameObject);
        }
    }

    public void HideHintMenu()
    {
        var _hintParentCanvas = _hintsParent.GetComponent<CanvasGroup>().alpha = 0;
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
